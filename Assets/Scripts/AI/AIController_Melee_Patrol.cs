using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController_Melee_Patrol : AIController_Melee
{
    private PatrolComponent patrol;

    protected override void Awake()
    {
        base.Awake();

        patrol = GetComponent<PatrolComponent>();
    }

    protected override void FixedUpdate()
    {
        if (Check_FixedUpdate() == false)
            return;

        GameObject player = perception.GetPercievedPlayer();

        if (player == null)
        {
            if (weapon.UnarmedMode == false)
                weapon.SetUnarmedMode();

            if (patrol == null)
            {
                SetWaitMode();

                return;
            }

            SetPatrolMode();

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

        if (state.DeadMode == false)
            SetApproachMode();
    }

    private void SetPatrolMode()
    {
        if(state.DeadMode==true)
            return;

        if (PatrolMode == true)
            return;

        ChangeType(Type.Patrol);

        if (patrol != null)
            navMeshAgent.speed = patrol.Speed;
        navMeshAgent.isStopped = false;

        patrol.StartMoving();
    }
}
