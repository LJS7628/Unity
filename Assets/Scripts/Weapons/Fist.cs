using UnityEngine;

public class Fist : Melee
{
    protected override void Reset()
    {
        base.Reset();

        type = WeaponType.Fist;
    }

    private enum PartType 
    {
        LeftHand =0,RightHand,LeftFoot,RightFoot,Max,
    }

    //阿 何困俊 公扁 何馒
    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < (int)PartType.Max; i++) 
        { 
            Transform t = colliders[i].transform;

            t.DetachChildren();
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;

            Weapon_Trigger trigger = t.GetComponent<Weapon_Trigger>();
            trigger.OnTrigger += OnTriggerEnter;

            string partName = ((PartType)i).ToString();
            Transform parnet =rootObject.transform.FindChildByName(partName);
            Debug.Assert(parnet != null);

            t.SetParent(parnet,false);
        }
    }

    //面倒 贸府
    public override void Begin_Collision(AnimationEvent e)
    {
        colliders[e.intParameter].enabled = true;
    }
}
