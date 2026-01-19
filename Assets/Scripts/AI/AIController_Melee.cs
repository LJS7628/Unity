using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController_Melee : AIController
{
    [Header(" - Approach")]
    [SerializeField]
    private float approachSpeed = 1.5f;

    [Header(" - Attack Range")]
    [SerializeField]
    protected float attackRange = 1.5f;

    protected override void FixedUpdate()
    {
        if (Check_FixedUpdate() == false)
            return;


        GameObject player = perception.GetPercievedPlayer();

        if (player == null)
        {
            if (weapon.UnarmedMode == false)
                weapon.SetUnarmedMode();

            return;
        }

        if (weapon.UnarmedMode)
        {
            SetEquipMode(WeaponType.Sword);
            return;
        }

        float temp = Vector3.Distance(transform.position, player.transform.position);
        if (temp < attackRange)
        {
            SetActionMode();

            return;
        }

        if (state.DeadMode == true) 
        {
            navMeshAgent.speed = 0.0f;
            navMeshAgent.isStopped = true;
        }

        if (state.DeadMode == false)
            SetApproachMode();
    }

    protected void SetApproachMode()
    {
        if (ApproachMode == true)
            return;

        ChangeType(Type.Approach);

        navMeshAgent.speed = approachSpeed;
        navMeshAgent.isStopped = false;
    }
}
