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
    private GameObject[] originPrefab;  //모든 무기 프리팹 저장 

    private WeaponType current = WeaponType.Unarmed;  //무기 상태
    public string CurrentName =>current.ToString(); // 무기 상태 이름

    public event Action OnEndEquip;  //장착 완료 액션 이벤트
    public event Action OnEndDoAction; //공격 완료 액션 이벤트

    private Animator animator;
    private StateComponent state;
    

    // 각 무기 모드 체크용
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

    // Dictionary로 무기 데이터 관리 
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

    //무기 장착
    public bool IsEquippingMode() 
    {
        if (UnarmedMode)
            return false;

        Weapon weapon = weaponTable[current];
        if(weapon == null) 
            return false;

        return weapon.Equipping;
    }

    // 무기 해제 상태 : 기본 
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

    // 상태 변화를 시켜줌 
    private void SetMode(WeaponType newType) 
    {
        if (current == newType) //장착할 무기와 현재 무기가 같은 경우
        {
            SetUnarmedMode();  // 무기 해제

            return;
        }
        else if (UnarmedMode == false) //무기가 장착되어 있으면
        {
            weaponTable[current].Unequip(); //무기 해제
        }

        if (weaponTable[newType] == null) //등록된 무기가 없으면 
        {
            SetUnarmedMode(); //무기 해제
        }

       
        animator.SetInteger("WeaponType", (int)newType);
        animator.SetBool("IsEquipping", true);

        weaponTable[newType].Equip(); //해당 무기 장착

        current = newType; //현재 타입 갱신
        
        
    }

    //무기 장착
    public void Begin_Equip()  
    {
        weaponTable[current].Begin_Equip(); 
    }
    
    // 장착 완료
    public void End_Equip() 
    {
        animator.SetBool("IsEquipping", false); 
        weaponTable[current].End_Equip(); 
        OnEndEquip?.Invoke(); //OnEndEquip 액션 이벤트 실행
    }

    //공격
    public void DoAction() 
    {

        if (weaponTable[current] == null)  //무기가 없다면
            return;

        if (weaponTable[current].CanDoAction() == false)  //공격 가능 상태 판단
            return;

        if (animator.name == "Boss") //Boss만 따로 처리
        {
            int rand = UnityEngine.Random.Range(0, 3);
            animator.SetInteger("Pattern", rand);
        }

        animator.SetBool("IsAction", true);
        weaponTable[current].DoAction(); //공격 실행
        
    }


   //공격 시작
    private void Begin_DoAction() 
    {
        weaponTable[current].Begin_DoAction();
    }

    //공격 완료
    public void End_DoAction() 
    {
        animator.SetBool("IsAction",false);
        weaponTable[current].End_DoAction();

        OnEndDoAction?.Invoke();
    }

    // 스킬 실행
    public void DoSkill()  
    {
        if(weaponTable[current] == null) 
            return;

        animator.SetBool("IsSkill", true);
    }

    //스킬 시작
    private  void Begin_DoSkill()  
    {
        weaponTable[current].Begin_DoSkill();
    }
    
    //스킬 완료
    public void End_DoSkill() 
    {
        animator.SetBool("IsSkill", false);
    }

    // 콤보 입력 시작
    private void Begin_Combo()  
    {
        Melee melee = weaponTable[current] as Melee;
        melee?.Begin_Combo();
    }

    // 콤보 입력 종료
    private void End_Combo()  
    {
        Melee melee = weaponTable[current] as Melee;
        melee?.End_Combo();
    }

    //충돌 시작
    private void Begin_Collision(AnimationEvent e) 
    {
        Melee melee = weaponTable[current] as Melee;
        melee?.Begin_Collision(e);
    }

    //충돌 종료
    private void End_Collision() 
    {
        Melee melee = weaponTable[current] as Melee;
        melee?.End_Collision();
    }

    //파티클 재생
    private void Play_DoAction_Particle() 
    {
        weaponTable[current].Play_Particle();
    }
}
