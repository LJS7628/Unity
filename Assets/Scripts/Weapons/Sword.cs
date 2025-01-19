using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Sword : Melee
{

    [SerializeField]
    private string holsterName = "Holster_Sword";

    [SerializeField]
    private string handName = "Hand_Sword";

    [SerializeField]
    private string coverName = "Odachi_cover";

    [SerializeField]
    private GameObject slashPrefab;

    [SerializeField]
    private AudioClip skillClip;

    private Transform holsterTransform;
    private Transform handTransform;

    private SkinnedMeshRenderer skinnedMesh;

    protected override void Reset()
    {
        base.Reset();

        type = WeaponType.Sword;
    }
    protected override void Awake()
    {
        base.Awake();

        holsterTransform = rootObject.transform.FindChildByName(holsterName);
        Debug.Assert(holsterTransform != null);

        handTransform = rootObject.transform.FindChildByName(handName);
        Debug.Assert(handTransform != null);


        transform.SetParent(holsterTransform, false);

        skinnedMesh = rootObject.transform.FindChildByName(coverName).GetComponent<SkinnedMeshRenderer>();


    }

    //장착 처리
    public override void Begin_Equip()
    {
        base.Begin_Equip();

        transform.parent.DetachChildren();
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        transform.SetParent(handTransform, false);
        transform.localScale = Vector3.one;
        skinnedMesh.enabled = false;

        weaponAttacked = true;
    }

    //해제 처리
    public override void Unequip()
    {
        base.Unequip();

        
        transform.parent.DetachChildren();
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        transform.SetParent(holsterTransform, false);
        transform.localScale = Vector3.one;
        skinnedMesh.enabled = true;

        weaponAttacked = false;
    }

    //스킬 시작
    public override void Begin_DoSkill()
    {
        base.Begin_DoSkill();

        GameObject obj = Instantiate<GameObject>(slashPrefab, rootObject.transform.position, rootObject.transform.rotation);
        obj.transform.position += rootObject.transform.transform.forward * 0.5f;
        obj.transform.position += rootObject.transform.transform.up * 0.9f;
        obj.transform.SetParent(rootObject.transform);

        PlaySound(skillClip);
    }

}