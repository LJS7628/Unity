using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Warp : Weapon
{
    [SerializeField]
    private GameObject particlePrefab;

    [SerializeField]
    private float traceDistance = 100;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private GameObject cursorPrefab;
    private GameObject cursorObject;

    private AIController_Range aiRange;
    protected override void Reset()
    {
        base.Reset();

        type = WeaponType.Warp;
    }

    protected override void Awake()
    {
        base.Awake();

        aiRange = rootObject.GetComponent<AIController_Range>();
    }
    protected override void Start()
    {
        base.Start();

        if (aiRange != null) //워프 사용자가 AI이면 커서 생성 막기
            return;
            

        CursorObject_Instance();
    }

    public override void Play_Particle() 
    {
        base.Play_Particle();

        if (particlePrefab == null)
            return;

        Instantiate<GameObject>(particlePrefab,rootObject.transform);
    }

    protected override void Update()
    {
        base.Update();


        bool bCheck = true;
        bCheck &= (bEquipped == true);
        bCheck &= (cursorObject != null);

        if (bCheck == false)
            return;

        bCheck &= CameraHelpers.GetCursorLocation(traceDistance, layerMask);
        bCheck &= state.ActionMode == false;

        if (cursorObject.transform.position.y > 0.06f)  //Decal 안그리기
            cursorObject.GetComponent<DecalProjector>().enabled = false;
        else
            cursorObject.GetComponent<DecalProjector>().enabled = true;



        cursorObject.SetActive(bCheck);
        
        
    }

    public override void End_Equip()
    {
        base.End_Equip();

        cursorObject?.SetActive(true);
    }

    public override void Unequip()
    {
        base.Unequip();

        cursorObject?.SetActive(false);
    }

    private Vector3 moveToPosition;
    public override bool CanDoAction()
    {
        if(base.CanDoAction()==false) 
            return false;

        if (aiRange == null) 
        {
            if (cursorObject == null)
                return false;

            bool bCheck = CameraHelpers.GetCursorLocation(out moveToPosition, traceDistance, layerMask);
            bCheck &= (cursorObject.transform.position.y <= 0.06f);

            if (bCheck == false)
                return false;
        }
        return true;
    }

    public override void DoAction()
    {
        base.DoAction();
    }

    public override void Begin_DoAction()
    {
        base.Begin_DoAction();

        PlaySound(null);

        if (aiRange != null) 
            moveToPosition = aiRange.AvoidPosition;
            
        rootObject.transform.position = moveToPosition;
    }

    public override void End_DoAction()
    {
        base.End_DoAction();

        moveToPosition = Vector3.zero;
    }

    //데칼 커서 생성
    private void CursorObject_Instance() 
    {
        if (cursorPrefab != null)
        {
            cursorObject = Instantiate<GameObject>(cursorPrefab);

            Warp_Cursor cursor = cursorObject.GetComponent<Warp_Cursor>();
            cursor.TraceDistance = traceDistance;
            cursor.Mask = layerMask;
            cursorObject.SetActive(false);
        }
    }
}
