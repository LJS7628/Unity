using UnityEngine;

public class DualBlade : Melee
{
    [SerializeField]
    private string[] holsterTransformName = new string[2] { "Holster_Dual1", "Holster_Dual2" };

    [SerializeField]
    private string[] dualHandName = new string[2] { "Hand_DualBlade_L", "Hand_DualBlade_R" };

    protected override void Reset()
    {
        base.Reset();

        type = WeaponType.DualBlade;
    }

    private enum PartType
    {
        holsterTransformL = 0, holsterTransformR, Max,
    }

    protected override void Awake()
    {
        base.Awake();

        
        for (int i = 0; i < (int)PartType.Max; i++)
        {
            Transform t = colliders[i].transform;

            Weapon_Trigger trigger = t.GetComponent<Weapon_Trigger>();
            trigger.OnTrigger += OnTriggerEnter;


            string partName = holsterTransformName[i];
            Transform parent = rootObject.transform.FindChildByName(partName);
            Debug.Assert(parent != null);

            t.SetParent(parent, false);
        }
        
    }


    public override void Begin_Equip()
    {
        base.Begin_Equip();
        for (int i = 0; i < (int)PartType.Max; i++)
        {
            Transform t = colliders[i].transform;

            string partName = dualHandName[i];
            Transform parent = rootObject.transform.FindChildByName(partName);

            t.SetParent(parent, false);
        }
    }

    public override void Unequip()
    {
        base.Unequip();
        for (int i = 0; i < (int)PartType.Max; i++)
        {

            Transform t = colliders[i].transform;

            string partName = holsterTransformName[i];
            Transform parent = rootObject.transform.FindChildByName(partName);

            t.SetParent(parent, false);
        }
    }
}
