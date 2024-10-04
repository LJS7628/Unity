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

    [SerializeField]
    private GameObject shieldPrefab;
    private GameObject shieldObject;

    private Transform holsterTransform2;
    private Transform handTransform2;

    private Animator animator;
    private new Rigidbody rigidbody;
    private HealthPointComponent healthPoint;
    private EnemyMovingComponent moving;
    private GameObject player;
    private Sword sword;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        healthPoint = GetComponent<HealthPointComponent>();
        moving = GetComponent<EnemyMovingComponent>();
        player = GameObject.Find("Player");


        
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

        if (shieldPrefab != null) 
        {
            holsterTransform2 = transform.FindChildByName("Holster_Shield");
            handTransform2 = transform.FindChildByName("Hand_Shield");


            shieldObject = Instantiate<GameObject>(shieldPrefab, holsterTransform2);
        }
        Equip();
    }

    private void Update()
    {
        if (moving.CheckNearPlayer(1.5f) & Player.attackCount < 2)
            Update_Attacking();

        else if (Player.attackCount >= 2) 
        {
            Update_Blocking();
        }
            
    }

    public void Damage(GameObject attacker, Sword causer, Vector3 hitPoint, DoActionData data)
    {
        Player.attackCount++;
        if (bBlocking)
        {
            
            if (data.Particle != null)
            {
                GameObject obj = Instantiate<GameObject>(data.Particle, transform, false);
                obj.transform.localPosition = hitPoint + data.ParticleOffset;
                obj.transform.localScale = data.ParticleScale;
            }

            return;
        }

        healthPoint.Damage(data.Power);

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
    private IEnumerator Change_IsKinemetics(int frame)
    {
        for (int i = 0; i < frame; i++)
            yield return new WaitForFixedUpdate();

        rigidbody.isKinematic = true;
    }

    private void Equip() 
    {
        swordObject.transform.parent.DetachChildren();
        swordObject.transform.position = Vector3.zero;
        swordObject.transform.rotation = Quaternion.identity;
        swordObject.transform.localScale = Vector3.one;
        swordObject.transform.SetParent(handTransform, false);


        shieldObject.transform.parent.DetachChildren();
        shieldObject.transform.position = Vector3.zero;
        shieldObject.transform.rotation = Quaternion.identity;
        shieldObject.transform.localScale = Vector3.one;
        shieldObject.transform.SetParent(handTransform2, false);
    }

}