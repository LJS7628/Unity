using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HealthPointComponent))]
public partial class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField]
    private GameObject swordPrefab;
    private GameObject swordObject;

    private Transform holsterTransform;
    private Transform handTransform;

    private Animator animator;
    private new Rigidbody rigidbody;
    private HealthPointComponent healthPoint;
    private EnemyMovingComponent moving;
    private GameObject player;
    private Sword sword;

    private List<Material> materialList;
    private List<Color> originColorList;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        healthPoint = GetComponent<HealthPointComponent>();
        moving = GetComponent<EnemyMovingComponent>();
        player = GameObject.Find("Player");

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

    private void Start()
    {
        if (swordPrefab != null)
        {
            holsterTransform = transform.FindChildByName("Holster_Sword");
            handTransform = transform.FindChildByName("Hand_Sword");

            swordObject = Instantiate<GameObject>(swordPrefab, holsterTransform);
            sword = swordObject.GetComponent<Sword>();
        }
    }

    private void Update()
    {

        Update_Attacking();
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

}