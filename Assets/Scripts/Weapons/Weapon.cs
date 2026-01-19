using Cinemachine;
using System;
using UnityEngine;

[Serializable]
public class DoActionData //무기 데이터
{
    public bool bCanMove;

    public float Power;
    public float Distance;
    public int StopFrame;

    public int ImpactIndex;

    public GameObject HitParticle;
    public Vector3 HitParticlePositionOffset;
    public Vector3 HitParticleScaleOffset = Vector3.zero;

    public float ShakeDuration;
    public Vector3 ShakeDirection;
    public Vector3 ShakeDirectionDeviation;
}


public class Weapon : MonoBehaviour
{
    [SerializeField]
    protected WeaponType type;
    public WeaponType Type => type;

    [SerializeField]
    protected AudioClip weaponClip;
    protected AudioSource audioSource;

    [SerializeField]
    protected DoActionData[] doActionDatas;

    protected Animator animator;
    protected StateComponent state;
    protected PlayerMovingComponent moving;
    protected CinemachineImpulseSource impulseSource;

    protected GameObject rootObject;

    protected bool bEquipping;
    public bool Equipping => bEquipping;

    protected bool bEquipped;

    public static bool weaponAttacked = false;
    protected virtual void Reset() 
    { 
    
    }
    protected virtual void Awake() 
    {
        rootObject = transform.root.gameObject;
        Debug.Assert(rootObject != null);

        audioSource = rootObject.GetComponent<AudioSource>();

        animator = rootObject.GetComponent<Animator>();
        state = rootObject.GetComponent<StateComponent>();
        moving = rootObject.GetComponent<PlayerMovingComponent>();

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    protected virtual void Start() 
    { 
    
    }

    protected virtual void Update() 
    {

    }
    //장착 모드 설정
    public void Equip() 
    {
        state.SetEquipMode();
    }
    //장착 중
    public virtual void Begin_Equip()
    {
        bEquipping = true;
    }
    //장착 완료
    public virtual void End_Equip() 
    {
        bEquipping = false;
        bEquipped = true;

        state.SetIdleMode();
    }
    //장착 해제
    public virtual void Unequip() 
    {
        bEquipped = false;
    }
    //공격 가능 여부 판단 (재정의 가능)
    public virtual bool CanDoAction() 
    {
        return true;
    }

    //공격
    public virtual void DoAction() 
    {
        state.SetActionMode();
        PlayerMoving(0);
  
    }
    //공격 시 처리
    public virtual void Begin_DoAction() 
    {
        weaponAttacked = true;  
    }

    //공격 완료 후 처리
    public virtual void End_DoAction() 
    {
        state.SetIdleMode();


        if (moving != null)
            moving.Move();
    }

    //스킬 처리
    public virtual void Begin_DoSkill() 
    {
        weaponAttacked = false; 
    }

    //파티클 재생
    public virtual void Play_Particle()
    {
        
    }

    //플레이어 움직임
    protected void PlayerMoving(int index) 
    { 
        if(moving == null)
            return;

        if(doActionDatas == null)
            return;

        if (doActionDatas.Length<1)
            return;

        if (doActionDatas[index].bCanMove) 
        {
            moving.Move();
            return;
        }

        moving.Stop();
    }

    //타격시 or 스킬 효과음 재생
    protected void PlaySound(AudioClip clip) 
    {
        if( audioSource == null )
            return;

        if (weaponAttacked == true) 
        {
            audioSource.clip = weaponClip;
            audioSource.Play();
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();
    }
}
