using System;
using UnityEngine;
using UnityEngine.AI;
public partial class AIController
{
    public enum Type
    {
        Wait = 0, Patrol, Approach, Equip, Action, Damaged,
    }

    private Type type = Type.Wait;

    public event Action<Type, Type> OnAIStateTypeChanged;

    public bool WaitMode => type == Type.Wait;
    public bool PatrolMode => type == Type.Patrol;
    public bool ApproachMode => type == Type.Approach;
    public bool EquipMode => type == Type.Equip;
    public bool ActionMode => type == Type.Action;
    public bool DamagedMode => type == Type.Damaged;

    //쿨타임
    protected bool DecreaseCoolTime()
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
    //쿨타임 또는 상태를 바꿀 때 조건 검사
    protected bool CheckMode()
    {
        bool bCheck = false;
        bCheck |= EquipMode == true;
        bCheck |= ActionMode == true;
        bCheck |= DamagedMode == true;


        return bCheck;
    }
    //대기 상태
    public void SetWaitMode()
    {
        if (WaitMode == true)
            return;

        ChangeType(Type.Wait);

        navMeshAgent.speed = 0.0f;
        navMeshAgent.isStopped = true;
    }
    //무기 장착
    protected void SetEquipMode(WeaponType type)
    {
        if (EquipMode == true)
            return;

        ChangeType(Type.Equip);

        navMeshAgent.isStopped = false;

        switch (type)
        {
            case WeaponType.Sword: weapon.SetSwordMode(); break;
            case WeaponType.FireBall: weapon.SetFireBallMode(); break;
            case WeaponType.Warp: weapon.SetWarpMode(); break;
        }
    }

    //공격 모드
    protected void SetActionMode()
    {
        if (ActionMode == true)
            return;

        ChangeType(Type.Action);

        navMeshAgent.speed = 0.0f;
        navMeshAgent.isStopped = true;

        weapon.DoAction();

    }

    //피해 받았을 때
    public void SetDamageMode() 
    {

        if (ActionMode == true)
        {
            animator.Play(weapon.CurrentName, 0);

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

    //상태 변경
    protected void ChangeType(Type newType)
    {
        Type prevType = type;
        type = newType;

        OnAIStateTypeChanged?.Invoke(prevType, type);
    }

}
