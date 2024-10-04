using System;
using UnityEngine;
using System.Linq;
using TMPro;


public partial class Player : MonoBehaviour, IDamagable
{
    [SerializeField]
    private GameObject swordPrefab;
    private GameObject swordObject;

    private Transform holsterTransform;
    private Transform handTransform;


    private Animator animator;
    private PlayerMovingComponent moving;

    private Sword sword;
    private HealthPointComponent healthPoint;
    private new Rigidbody rigidbody;

    public static bool isPlayerDead=false;
    public static int attackCount = 0;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        moving = GetComponent<PlayerMovingComponent>();
        healthPoint = GetComponent<HealthPointComponent>();
        rigidbody = GetComponent<Rigidbody>();


    }

    private void Start ()
	{
		if(swordPrefab != null)
        {
            holsterTransform = transform.FindChildByName("Holster_Sword");
            handTransform = transform.FindChildByName("Hand_Sword");

            swordObject = Instantiate<GameObject>(swordPrefab, holsterTransform);
            sword = swordObject.GetComponent<Sword>();
        }
	}

    private void Update()
    {
        Debug.Log(attackCount);
        Update_Drawing();
        Update_Attacking();
        Update_Blocking();
	}


    public void Damage(GameObject attacker, Sword causer, Vector3 hitPoint, DoActionData data)
    {
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

        if (healthPoint.IsDead == false)
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

            if(bEquipped)
                animator.SetTrigger("Sword");
            else
                animator.SetTrigger("Unarmed");

            float launch = rigidbody.drag * data.Distance * 10.0f;
            rigidbody.AddForce(-transform.forward * launch);
            return;
        }

        if (healthPoint.IsDead) 
        {
            animator.SetTrigger("Death");
            Destroy(gameObject, 5.0f);
            GetComponent<Collider>().enabled = false;
            isPlayerDead = true;
        }


    }
}