using System;
using UnityEngine;

public class StateComponent : MonoBehaviour
{
    // ���� enum���� ���� 
    public enum StateType
    { 
        Idle=0, Equip,Action,Evade, Damaged,Dead, Boss,
    }

    private StateType type = StateType.Idle;

    public event Action<StateType, StateType> OnStateTypeChanged;

    // �� ���� üũ
    public bool IdleMode { get => type == StateType.Idle; }
    public bool EquipMode { get => type == StateType.Equip; }
    public bool ActionMode { get => type == StateType.Action; }

    public bool EvadeMode { get => type == StateType.Evade; }
    public bool DamagedMode { get => type == StateType.Damaged; }
    public bool DeadMode { get => type == StateType.Dead; }
    public bool BossMode { get => type == StateType.Boss; }

    // ���� ����
    public void SetIdleMode() => ChangeType(StateType.Idle);
    public void SetEquipMode() => ChangeType(StateType.Equip);
    public void SetActionMode() => ChangeType(StateType.Action);

    public void SetEvadeMode() => ChangeType(StateType.Evade);
    public void SetDamagedMode() => ChangeType(StateType.Damaged);
    public void SetDeadMode() => ChangeType(StateType.Dead);
    public void SetBossMode() => ChangeType(StateType.Boss);

    // ���� �ٲٱ� 
    private void ChangeType(StateType newType) 
    {
        if (type == newType)
            return;

        StateType prevType = type;
        type = newType;

        OnStateTypeChanged?.Invoke(prevType, newType);
    }
}
