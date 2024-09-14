using System;
using UnityEngine;

using System.Linq;

public partial class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject swordPrefab;
    private GameObject swordObject;

    private Transform holsterTransform;
    private Transform handTransform;


    private Animator animator;
    private PlayerMovingComponent moving;

    private Sword sword;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        moving = GetComponent<PlayerMovingComponent>();
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
        Update_Drawing();
        Update_Attacking();
	}

    private void Update_Moving()
    {
        
    }
}