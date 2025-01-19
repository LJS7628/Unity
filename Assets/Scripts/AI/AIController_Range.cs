using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController_Range : AIController
{
    [Header(" - Avoid")]
    [SerializeField]
    private float aviodRange = 5.0f;

    [SerializeField]
    private Vector2 backwardDistacne = new Vector2(3, 10);

    [SerializeField]
    private float backwardRange = 3.0f;

    [SerializeField]
    private int vaildationCount = 5;
    protected override void FixedUpdate()
    {
        if(Check_FixedUpdate()==false)
            return;

        GameObject player = perception.GetPercievedPlayer();

        if (player == null)
        {
            if (weapon.UnarmedMode == false)
                weapon.SetUnarmedMode();

            SetWaitMode();

            return;
        }

        transform.LookAt(player.transform);

        float temp = Vector3.Distance(transform.position, player.transform.position);
        if (temp >= aviodRange) 
        {
            if (weapon.FireBallMode == false) 
            {
                SetEquipMode(WeaponType.FireBall);
                return;
            }

            if(weapon.FireBallMode)
                SetActionMode();

            return;
        }

        if (weapon.WarpMode == false) 
        {
            SetEquipMode(WeaponType.Warp);
            return;
        }

        if (weapon.WarpMode) 
            DoAction_Warp(player.transform);
        
    }


    protected override void End_Damaged()
    {
        if (weapon.WarpMode)
            currentCoolTime = -1.0f;

        base.End_Damaged();
    }

    private Vector3 avoidPosition;
    public Vector3 AvoidPosition=>avoidPosition;

    private void DoAction_Warp(Transform avoidTransform) 
    {
        avoidPosition = GetAvoidPosition(avoidTransform);

        transform.LookAt(avoidPosition);
        SetActionMode();
    }

    //플레이어로 부터 도망칠 장소 찾기
    private Vector3 GetAvoidPosition(Transform avoidTransform) 
    {
        Vector3 range = new Vector3();
        range.x = Random.Range(-backwardRange,+backwardRange);
        range.z = Random.Range(-backwardRange, +backwardRange);

        float distance = 0.0f;
        Vector3 direction = Vector3.zero;
        Vector3 position = Vector3.zero;

        NavMeshPath path = new NavMeshPath();
        for (int i = 1; i < vaildationCount; i++) 
        {
            distance = Random.Range(backwardDistacne.x, backwardDistacne.y);
            direction = avoidTransform.position - transform.position;

            position = avoidTransform.position + (direction.normalized * distance);
            position += range;

            if (navMeshAgent.CalculatePath(position,path))
                return position;
        }

        distance = Random.Range(backwardDistacne.x, backwardDistacne.y);
        direction = transform.position - avoidTransform.position;

        position = avoidTransform.position + (direction.normalized * distance);
        position += range;

        return position;    
    }

}
