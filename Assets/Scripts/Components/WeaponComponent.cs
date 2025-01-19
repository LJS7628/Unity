using System;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType  
{ 
    Unarmed=0, Fist,Sword,DualBlade,Warp,FireBall,Max,
}
public class WeaponComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject[] originPrefab;  //��� ���� ������ ���� 

    private WeaponType current = WeaponType.Unarmed;  //���� ����
    public string CurrentName =>current.ToString(); // ���� ���� �̸�

    public event Action OnEndEquip;  //���� �Ϸ� �׼� �̺�Ʈ
    public event Action OnEndDoAction; //���� �Ϸ� �׼� �̺�Ʈ

    private Animator animator;
    private StateComponent state;
    

    // �� ���� ��� üũ��
    public bool UnarmedMode { get => current == WeaponType.Unarmed; }
    public bool FistMode { get => current == WeaponType.Fist; }
    public bool SwordMode { get => current == WeaponType.Sword; }
    public bool DualBladeMode { get => current == WeaponType.DualBlade; }
    public bool WarpMode { get => current == WeaponType.Warp; }
    public bool FireBallMode { get => current == WeaponType.FireBall; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        state = GetComponent<StateComponent>();
    }

    // Dictionary�� ���� ������ ���� 
    private Dictionary<WeaponType, Weapon> weaponTable;
    private void Start()
    {
        weaponTable = new Dictionary<WeaponType, Weapon>();

        for (int i = 0; i < (int)WeaponType.Max; i++)
            weaponTable.Add((WeaponType)i, null);

        for(int i = 0; i < originPrefab.Length; i++) 
        {
            GameObject obj =Instantiate<GameObject>(originPrefab[i],transform);
            Weapon weapon = obj.GetComponent<Weapon>();
            obj.name = weapon.Type.ToString();

            weaponTable[weapon.Type] = weapon;
        }
    }

    //���� ����
    public bool IsEquippingMode() 
    {
        if (UnarmedMode)
            return false;

        Weapon weapon = weaponTable[current];
        if(weapon == null) 
            return false;

        return weapon.Equipping;
    }

    // ���� ���� ���� : �⺻ 
    public void SetUnarmedMode() 
    {
        if (state.IdleMode == false)
            return;

        animator.SetInteger("WeaponType", (int)WeaponType.Unarmed);

        if (weaponTable[current] != null)
            weaponTable[current].Unequip();

        current = WeaponType.Unarmed;
    }

    //Fist
    public void SetFistMode()
    {
        if (state.IdleMode == false)
            return;

        SetMode(WeaponType.Fist);
    }

    //Sword
    public void SetSwordMode() 
    {
        if (state.IdleMode == false)
            return;

        SetMode(WeaponType.Sword);
    }

    //DualBlade
    public void SetDualBladeMode()
    {
        if (state.IdleMode == false)
            return;

        SetMode(WeaponType.DualBlade);
    }

    //Warp
    public void SetWarpMode()
    {
        if (state.IdleMode == false)
            return;

        SetMode(WeaponType.Warp);
    }

    //FireBall
    public void SetFireBallMode()
    {
        if (state.IdleMode == false)
            return;

        SetMode(WeaponType.FireBall);
    }

    // ���� ��ȭ�� ������ 
    private void SetMode(WeaponType newType) 
    {
        if (current == newType) //������ ����� ���� ���Ⱑ ���� ���
        {
            SetUnarmedMode();  // ���� ����

            return;
        }
        else if (UnarmedMode == false) //���Ⱑ �����Ǿ� ������
        {
            weaponTable[current].Unequip(); //���� ����
        }

        if (weaponTable[newType] == null) //��ϵ� ���Ⱑ ������ 
        {
            SetUnarmedMode(); //���� ����
        }

       
        animator.SetInteger("WeaponType", (int)newType);
        animator.SetBool("IsEquipping", true);

        weaponTable[newType].Equip(); //�ش� ���� ����

        current = newType; //���� Ÿ�� ����
        
        
    }

    //���� ����
    public void Begin_Equip()  
    {
        weaponTable[current].Begin_Equip(); 
    }
    
    // ���� �Ϸ�
    public void End_Equip() 
    {
        animator.SetBool("IsEquipping", false); 
        weaponTable[current].End_Equip(); 
        OnEndEquip?.Invoke(); //OnEndEquip �׼� �̺�Ʈ ����
    }

    //����
    public void DoAction() 
    {

        if (weaponTable[current] == null)  //���Ⱑ ���ٸ�
            return;

        if (weaponTable[current].CanDoAction() == false)  //���� ���� ���� �Ǵ�
            return;

        if (animator.name == "Boss") //Boss�� ���� ó��
        {
            int rand = UnityEngine.Random.Range(0, 3);
            animator.SetInteger("Pattern", rand);
        }

        animator.SetBool("IsAction", true);
        weaponTable[current].DoAction(); //���� ����
        
    }


   //���� ����
    private void Begin_DoAction() 
    {
        weaponTable[current].Begin_DoAction();
    }

    //���� �Ϸ�
    public void End_DoAction() 
    {
        animator.SetBool("IsAction",false);
        weaponTable[current].End_DoAction();

        OnEndDoAction?.Invoke();
    }

    // ��ų ����
    public void DoSkill()  
    {
        if(weaponTable[current] == null) 
            return;

        animator.SetBool("IsSkill", true);
    }

    //��ų ����
    private  void Begin_DoSkill()  
    {
        weaponTable[current].Begin_DoSkill();
    }
    
    //��ų �Ϸ�
    public void End_DoSkill() 
    {
        animator.SetBool("IsSkill", false);
    }

    // �޺� �Է� ����
    private void Begin_Combo()  
    {
        Melee melee = weaponTable[current] as Melee;
        melee?.Begin_Combo();
    }

    // �޺� �Է� ����
    private void End_Combo()  
    {
        Melee melee = weaponTable[current] as Melee;
        melee?.End_Combo();
    }

    //�浹 ����
    private void Begin_Collision(AnimationEvent e) 
    {
        Melee melee = weaponTable[current] as Melee;
        melee?.Begin_Collision(e);
    }

    //�浹 ����
    private void End_Collision() 
    {
        Melee melee = weaponTable[current] as Melee;
        melee?.End_Collision();
    }

    //��ƼŬ ���
    private void Play_DoAction_Particle() 
    {
        weaponTable[current].Play_Particle();
    }
}
