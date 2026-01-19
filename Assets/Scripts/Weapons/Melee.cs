using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Melee : Weapon
{
    private bool bEnable;
    private bool bExist;
    private int index;

    protected Collider[] colliders;
    private List<GameObject> hittedList;
    protected override void Awake()
    {
        base.Awake();

        colliders = GetComponentsInChildren<Collider>();
        hittedList = new List<GameObject>();
    }


    protected override void Start()
    {
        base.Start();

        End_Collision();
    }

    public virtual void Begin_Collision(AnimationEvent e)
    {
        foreach (Collider collider in colliders)
            collider.enabled = true;

    }
    public virtual void End_Collision()
    {
        foreach (Collider collider in colliders)
            collider.enabled = false;

        hittedList.Clear();
    }
    public void Begin_Combo()
    {

        bEnable = true;
    }

    public void End_Combo()
    {

        bEnable = false;
    }

    public override void DoAction()
    {

        if (bEnable)
        {
            bEnable = false;
            bExist = true;

            return;
        }

        if (state.IdleMode == false)
            return;

        
        base.DoAction();
    }

    public override void Begin_DoAction()
    {
        base.Begin_DoAction();
        if (state.name != "Boss") 
        {
            if (bExist == false)
                return;

            bExist = false;
        }
        index++;
        animator.SetTrigger("NextCombo"); //ÄÞº¸ ½ÇÇà
        PlayerMoving(index);
        
    }


    public override void End_DoAction()
    {
        base.End_DoAction();
        index = 0;

        bExist = false;
        bEnable = false;
    }

    protected  void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == rootObject)
            return;

        if (other.CompareTag(rootObject.tag))
            return;

        IDamagable damage = other.GetComponent<IDamagable>();

        if (damage == null)
            return;

        if (hittedList.Find(hitted => hitted == other.gameObject))
            return;

        hittedList.Add(other.gameObject);

        if (impulseSource != null && doActionDatas[index].ShakeDuration > 0.0f)
        {
            impulseSource.m_ImpulseDefinition.m_ImpulseDuration = doActionDatas[index].ShakeDuration;

            Vector3 direction = doActionDatas[index].ShakeDirection;
            Vector3 deviation = doActionDatas[index].ShakeDirectionDeviation;
            direction.x += Random.Range(-deviation.x, +deviation.x);
            direction.y += Random.Range(-deviation.y, +deviation.y);
            direction.z += Random.Range(-deviation.z, +deviation.z);

            impulseSource.m_DefaultVelocity = direction;

            impulseSource.GenerateImpulse();
        }

        Collider enableCollider = colliders.ToList().Find(col => col.enabled);

        Vector3 hitPoint = enableCollider.ClosestPoint(other.transform.position);
        hitPoint = other.transform.InverseTransformPoint(hitPoint);

        damage.OnDamage(rootObject, this, hitPoint, doActionDatas[index]);
        
        PlaySound(weaponClip);
    }

}
