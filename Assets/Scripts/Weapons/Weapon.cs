using Cinemachine;
using System;
using UnityEngine;

[Serializable]
public class DoActionData //���� ������
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
    //���� ��� ����
    public void Equip() 
    {
        state.SetEquipMode();
    }
    //���� ��
    public virtual void Begin_Equip()
    {
        bEquipping = true;
    }
    //���� �Ϸ�
    public virtual void End_Equip() 
    {
        bEquipping = false;
        bEquipped = true;

        state.SetIdleMode();
    }
    //���� ����
    public virtual void Unequip() 
    {
        bEquipped = false;
    }
    //���� ���� ���� �Ǵ� (������ ����)
    public virtual bool CanDoAction() 
    {
        return true;
    }

    //����
    public virtual void DoAction() 
    {
        state.SetActionMode();
        PlayerMoving(0);
  
    }
    //���� �� ó��
    public virtual void Begin_DoAction() 
    {
        weaponAttacked = true;  
    }

    //���� �Ϸ� �� ó��
    public virtual void End_DoAction() 
    {
        state.SetIdleMode();


        if (moving != null)
            moving.Move();
    }

    //��ų ó��
    public virtual void Begin_DoSkill() 
    {
        weaponAttacked = false; 
    }

    //��ƼŬ ���
    public virtual void Play_Particle()
    {
        
    }

    //�÷��̾� ������
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

    //Ÿ�ݽ� or ��ų ȿ���� ���
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
