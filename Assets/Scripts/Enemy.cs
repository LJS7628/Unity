using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HealthPointComponent))]
public partial class Enemy : MonoBehaviour, IDamagable
{
    private Animator animator;
    private new Rigidbody rigidbody;
    private HealthPointComponent healthPoint;
    private EnemyMovingComponent moving;

    private List<Material> materialList;
    private List<Color> originColorList;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        healthPoint = GetComponent<HealthPointComponent>();
        moving = GetComponent<EnemyMovingComponent>();

        materialList = new List<Material>();
        originColorList = new List<Color>();

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            foreach(Material material in renderer.materials)
            {
                materialList.Add(material);
                originColorList.Add(material.color);
            }
        }
    }

    private void Start ()
	{
        
	}

    private void Update()
    {

    }

    public void Damage(GameObject attacker, Sword causer, Vector3 hitPoint, DoActionData data)
    {
        healthPoint.Damage(data.Power);


        foreach (Material material in materialList)
            material.color = Color.red;

        Invoke("RestoreColor", 0.15f);

        if(healthPoint.IsDead == false)
            transform.LookAt(attacker.transform, Vector3.up);


        StopFrameComponent.Instance.Delay(data.StopFrame);


        if (data.Particle != null)
        {
            GameObject obj = Instantiate<GameObject>(data.Particle, transform, false);
            obj.transform.localPosition = hitPoint + data.ParticleOffset;
            obj.transform.localScale = data.ParticleScale;
        }

        if (healthPoint.IsDead == false)
        {
            animator.SetTrigger("Damaged");

            
            rigidbody.isKinematic = false;

            float launch = rigidbody.drag * data.Distance * 10.0f;
            rigidbody.AddForce(-transform.forward * launch);

            StartCoroutine(Change_IsKinemetics(5));

            return;
        }

        
        animator.SetTrigger("Death");
        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 5.0f);
    }

    private void RestoreColor()
    {
        for (int i = 0; i < materialList.Count; i++)
            materialList[i].color = originColorList[i];
    }

    private IEnumerator Change_IsKinemetics(int frame)
    {
        for (int i = 0; i < frame; i++)
            yield return new WaitForFixedUpdate();

        rigidbody.isKinematic = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        int segments = 50;
        float radius = 3f;

        // 이전 점과 현재 점을 저장할 변수
        Vector3 prevPoint = Vector3.zero;
        Vector3 firstPoint = Vector3.zero;

        // 각 세그먼트마다 그릴 각도
        float angleStep = 360f / segments;

        // 시작 각도 설정
        float angle = 0f;

        // 원의 각 점을 순차적으로 그리기
        for (int i = 0; i <= segments; i++)
        {
            // 현재 각도에 따른 x, z 좌표 계산 (2D 평면에서 원을 그리므로 x, z를 사용)
            float rad = Mathf.Deg2Rad * angle;
            float x = Mathf.Cos(rad) * radius;
            float z = Mathf.Sin(rad) * radius;

            // 현재 점의 위치
            Vector3 currentPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

            // 첫 번째 점 저장
            if (i == 0)
            {
                firstPoint = currentPoint;
            }
            else
            {
                // 이전 점과 현재 점을 선으로 연결
                Gizmos.DrawLine(prevPoint, currentPoint);
            }

            // 이전 점을 현재 점으로 갱신
            prevPoint = currentPoint;

            // 각도를 증가시켜 다음 점으로 이동
            angle += angleStep;
        }

        // 마지막 점과 첫 번째 점을 연결하여 원을 닫음
        Gizmos.DrawLine(prevPoint, firstPoint);
    }
}