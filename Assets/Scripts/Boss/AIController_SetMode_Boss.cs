using System;
using UnityEngine;
using UnityEngine.AI;
public partial class AIController_Boss
{
    public enum Type
    {
        Wait = 0, Patrol, Approach, Equip, Action, Damaged,Boss
    }

    private Type type = Type.Wait;

    public event Action<Type, Type> OnAIStateTypeChanged;

    public bool WaitMode => type == Type.Wait;
    public bool PatrolMode => type == Type.Patrol;
    public bool ApproachMode => type == Type.Approach;
    public bool EquipMode => type == Type.Equip;
    public bool ActionMode => type == Type.Action;
    public bool DamagedMode => type == Type.Damaged;
    public bool BossMode => type == Type.Boss;


    private bool DecreaseCoolTime()
    {
        if (WaitMode == false)
            return false;

        if (currentCoolTime <= 0.0f)
            return false;

        currentCoolTime -= Time.fixedDeltaTime;

        bool bFinish = false;
        bFinish |= (currentCoolTime <= 0.0f);
        bFinish |= (perception.GetPercievedPlayer() == null);

        if (bFinish)
        {
            currentCoolTime = 0.0f;

            return false;
        }

        return true;
    }
    private bool CheckMode()
    {
        bool bCheck = false;
        bCheck |= EquipMode == true;
        bCheck |= ActionMode == true;
        bCheck |= DamagedMode == true;

        return bCheck;
    }
    public void SetWaitMode()
    {
        if (WaitMode == true)
            return;

        ChangeType(Type.Wait);

        
        navMeshAgent.speed = 0.0f;
        navMeshAgent.isStopped = true;
    }

    private void SetPatrolMode()
    {
        if (PatrolMode == true)
            return;

        ChangeType(Type.Patrol);

        navMeshAgent.speed = partolSpeed;
        navMeshAgent.isStopped = false;

        patrol.StartMoving();
    }
    private void SetApproachMode()
    {
        if (ApproachMode == true)
            return;

        ChangeType(Type.Approach);

        navMeshAgent.speed = approachSpeed;
        navMeshAgent.isStopped = false;
    }

    private void SetEquipMode(WeaponType type)
    {
        if (EquipMode == true)
            return;

        ChangeType(Type.Equip);

        navMeshAgent.isStopped = false;

        switch (type)
        {
            case WeaponType.Sword: weapon.SetSwordMode(); break;
        }
    }
    private void SetActionMode()
    {
        if (ActionMode == true)
            return;

        ChangeType(Type.Action);

        navMeshAgent.speed = 0.0f;
        navMeshAgent.isStopped = true;

        weapon.DoAction();
        
    }

    private void SetBossMode()
    {
        if (BossMode == true)
            return;

        ChangeType(Type.Boss);

        navMeshAgent.speed = 0.0f;
        navMeshAgent.isStopped = true;

        rigidbody.isKinematic = false;
        weapon.DoAction();

    }


    public void SetDamageMode() 
    {

        if (ActionMode == true)
        {
            animator.Play("Blend Tree", 0);

            if (animator.GetBool("IsAction") == true)
                weapon.End_DoAction();
        }
        
        bool bCancledCoolTime = false;
        bCancledCoolTime |= ApproachMode;
        bCancledCoolTime |= EquipMode;
        bCancledCoolTime |= (perception.GetPercievedPlayer() == null);

        if (bCancledCoolTime)
            currentCoolTime = -1.0f;

        if (EquipMode == true) 
        {
            int layerIndex = animator.GetLayerIndex("DrawLayer");
            animator.Play("Unarmed_Draw", layerIndex);

            if(weapon.IsEquippingMode()==false)
                weapon.Begin_Equip();

            weapon.End_Equip();
        }

        if(DamagedMode == true) 
            return;

        ChangeType(Type.Damaged);

        navMeshAgent.speed = 0.0f;
        navMeshAgent.isStopped = true;
    }
    private void ChangeType(Type newType)
    {
        Type prevType = type;
        type = newType;

        OnAIStateTypeChanged?.Invoke(prevType, type);
    }

}
