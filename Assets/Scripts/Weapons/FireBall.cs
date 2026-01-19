using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Weapon
{
    [SerializeField]
    private string staffName = "Hand_FireBall_Staff";

    [SerializeField]
    private string flameName = "Hand_FireBall_Flame";

    [SerializeField]
    private string muzzleName = "Hand_FireBall_Muzzle";

    [SerializeField]
    private GameObject flamePrefab;
    private GameObject flameObject;

    [SerializeField]
    private GameObject muzzlePrefab;

    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private GameObject explosionPrefab;

    private Transform staffTransform;
    private Transform flameTransform;
    private Transform muzzleTransform;

    [SerializeField]
    private AudioClip skillClip;
    protected override void Reset()
    {
        base.Reset();

        type = WeaponType.FireBall;
    }

    protected override void Awake()
    {
        base.Awake();

        staffTransform = rootObject.transform.FindChildByName(staffName);
        Debug.Assert(staffTransform != null);
        transform.SetParent(staffTransform, false);

        flameTransform = rootObject.transform.FindChildByName(flameName);
        Debug.Assert(flameTransform != null);

        muzzleTransform = rootObject.transform.FindChildByName(muzzleName);
        Debug.Assert(muzzleTransform != null);

        if (flamePrefab != null)
        {
            flameObject = Instantiate<GameObject>(flamePrefab, flameTransform);
            flameObject.SetActive(false);
        }


        gameObject.SetActive(false);
    }

    public override void Begin_Equip()
    {
        base.Begin_Equip();

        gameObject.SetActive(true);
        flameObject?.SetActive(true);
    }

    public override void Unequip()
    {
        base.Unequip();

        gameObject.SetActive(false);
        flameObject?.SetActive(false);
    }

    public override void Play_Particle()
    {
        base.Play_Particle();

        if (muzzlePrefab == null)
            return;

        Vector3 position = muzzleTransform.position;
        Quaternion rotation = rootObject.transform.rotation;

        Instantiate<GameObject>(muzzlePrefab, position, rotation);

    }

    public override void Begin_DoAction()
    {
        base.Begin_DoAction();

        if (projectilePrefab == null)
            return;

        PlaySound(weaponClip);

        Vector3 position = muzzleTransform.position;
        position += rootObject.transform.forward * 0.5f;

        GameObject obj = Instantiate<GameObject>(projectilePrefab, position, rootObject.transform.rotation);
        Projectile projectile = obj.GetComponent<Projectile>();
        {
            Vector3 direction = Camera.main.transform.forward;

            if (rootObject.CompareTag("Enemy"))
                direction = rootObject.transform.forward;

            projectile.Direction = direction;
            projectile.OnProjectileHit += OnProjectileHit;
        }
        projectile.enabled = true;
    }

    //스킬 처리
    public override void Begin_DoSkill()
    {
        base.Begin_DoSkill();

        GameObject obj = Instantiate<GameObject>(explosionPrefab, rootObject.transform.position, rootObject.transform.rotation);
        obj.transform.position += rootObject.transform.transform.forward * 8.0f;
        obj.transform.position += rootObject.transform.transform.up * 0.5f;
        obj.transform.SetParent(rootObject.transform);

        PlaySound(skillClip);
    }

    //발사체 맞췄을 때 처리
    private void OnProjectileHit(Collider self, Collider other, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (other.CompareTag(rootObject.tag))
            return;

        IDamagable damage = other.GetComponent<IDamagable>();

        if (damage != null)
        {
            Vector3 point = self.ClosestPoint(other.transform.position);
            point = other.transform.InverseTransformPoint(hitPoint);

            damage?.OnDamage(rootObject, this, point, doActionDatas[0]);

            return;
        }


        Quaternion q = Quaternion.LookRotation(hitNormal);

        if (doActionDatas[0].HitParticle)
            Instantiate<GameObject>(doActionDatas[0].HitParticle, hitPoint, q);


    }


}
