using UnityEngine;
using UnityEngine.InputSystem;

using StateType = StateComponent.StateType;
public class PlayerMovingComponent : MonoBehaviour
{
    [SerializeField]
    private bool bLockCursor = true;


    [SerializeField, Header("Moving")]
    private float walkSpeed = 2.0f;

    [SerializeField]
    private float runSpeed = 4.0f;

    [SerializeField]
    private float sensitivity = 10.0f;

    [SerializeField]
    private float deadZone = 0.1f;

    [SerializeField, Header("Rotation")]
    private Transform followTargetTransform;

    [SerializeField]
    private float mouseSpeed = 0.25f;

    [SerializeField]
    private Vector2 limitPitchAngle = new Vector2(20, 340);

    //�÷��̾�, ī�޶� ����
    private void Reset()
    {
        GameObject obj = GameObject.Find("Player");
        Debug.Assert(obj != null);

        followTargetTransform = obj.transform.FindChildByName("CameraTarget");
        Debug.Assert(followTargetTransform != null);
    }

    //�̵� ���� ���� üũ
    #region CanMove
    private bool bCanMove = true;
    public void Move() => bCanMove = true;
    public void Stop() => bCanMove = false;
    #endregion


    private bool bRun;
    private Vector2 inputMove;
    private Vector2 inputLook;

    private Animator animator;
    private StateComponent state;
    private WeaponComponent weapon;
    private void Awake()
    {
        Awake_BindInput();
        Awake_GetComponents();
    }

    private void Awake_GetComponents()
    {
        animator = GetComponent<Animator>();
        state = GetComponent<StateComponent>();
        weapon = GetComponent<WeaponComponent>();
    }

    private void Awake_BindInput()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        InputActionMap actionMap = input.actions.FindActionMap("PlayerAction");

        //Move
        {
            InputAction action = actionMap.FindAction("Move");
            action.performed += context =>
            {
                inputMove = context.ReadValue<Vector2>();
            };
            action.canceled += context => inputMove = Vector2.zero;
        }
        //Run
        {
            InputAction action = actionMap.FindAction("Run");
            action.started += context => bRun = true;
            action.canceled += context => bRun = false;
        }

        //Look
        {
            InputAction action = actionMap.FindAction("Look");
            action.performed += context =>
            {
                inputLook = context.ReadValue<Vector2>();
            };
            action.canceled += context => inputLook = Vector2.zero;
        }

        //Evade
        {
            actionMap.FindAction("Evade").started += context =>
            {
                if (weapon.UnarmedMode == false)
                    return;

                if (state.IdleMode == false)
                    return;

                state.SetEvadeMode();
            };
        }
    }


    private void Start()
    {
        Start_LockCursor();
        Start_BindEvent();
    }

    //Ŀ�� �Ⱥ��̰�
    private void Start_LockCursor()
    {
        if (bLockCursor == false)
            return;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //���º�ȭ �̺�Ʈ ����
    private void Start_BindEvent()
    {
        state.OnStateTypeChanged += OnStateTypeChanged;
    }

    private Vector2 velocity;
    private Vector2 currInputMove;

    private Quaternion rotation;
    public Quaternion CameraRotation  //ī�޶� ȸ�� ���� ���� ����
    {
        set
        {
            rotation = Quaternion.Euler(rotation.eulerAngles.x, value.eulerAngles.y, 0.0f);
        }
    }

    [SerializeField]
    private bool bLock = false;

    // Ŀ�� �� ����
    public void SetLock()
    {
        bLock = true;
    }

    // Ŀ�� �� ����
    public void Unlock(Vector3 direction)
    {
        bLock = false;

        Quaternion q1 = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion q2 = Quaternion.Euler(rotation.eulerAngles.x, q1.eulerAngles.y, 0.0f);

        rotation = q2;
    }

    private void Update()
    {
        //�̵� ���� ó��
        currInputMove = Vector2.SmoothDamp(currInputMove, inputMove, ref velocity, 1.0f / sensitivity);

        if (bCanMove == false)
            return;

        //1. ���콺 ȸ�� ó�� 
        if(bLock == false)
            rotation *= Quaternion.AngleAxis(inputLook.x, Vector3.up);

        rotation *= Quaternion.AngleAxis(-inputLook.y, Vector3.right);
        followTargetTransform.rotation = rotation;

        Vector3 angle = followTargetTransform.localEulerAngles;
        angle.z = 0.0f;

        float angleX = followTargetTransform.localEulerAngles.x;

        if (angleX < 100.0f && angleX > limitPitchAngle.x)
            angle.x = limitPitchAngle.x;
        else if (angleX > 180.0f && angleX < limitPitchAngle.y)
            angle.x = limitPitchAngle.y;

        followTargetTransform.localEulerAngles = angle;

        if (bLock == false) 
        {
            rotation = Quaternion.Lerp(followTargetTransform.rotation, rotation, mouseSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(0.0f, rotation.eulerAngles.y, 0.0f);
        }

        followTargetTransform.localEulerAngles = new Vector3(angle.x, 0.0f, 0.0f);

        //2. Ű���� �̵� ó��
        Vector3 direction = Vector3.zero;

        float speed = bRun ? runSpeed : walkSpeed;
        if (currInputMove.magnitude > deadZone)
        {
            direction = (Vector3.right * currInputMove.x) + (Vector3.forward * currInputMove.y);
            direction = direction.normalized * speed;
        }

        transform.Translate(direction * Time.deltaTime);

        animator.SetFloat("SpeedX", currInputMove.x * speed);
        animator.SetFloat("SpeedY", currInputMove.y * speed);
        animator.SetFloat("SpeedZ", direction.magnitude);
    }

    //ȸ�� ���� ó��
    private void OnStateTypeChanged(StateType prevType, StateType newType)
    {
        switch (newType)
        {
            case StateType.Evade: Execute_Evade(); break;
        }
    }
    private enum EvadeDirection
    {
        Forward = 0, Backward, Left, Right,
    }

    //���⿡ �´� ȸ�� ó��
    private void Execute_Evade()
    {
        EvadeDirection direction = EvadeDirection.Forward;
        if (inputMove.y == 0.0f)
        {
            direction = EvadeDirection.Forward;

            if (inputMove.x < 0.0f)
                direction = EvadeDirection.Left;
            else if (inputMove.x > 0.0f)
                direction = EvadeDirection.Right;
        }
        else if (inputMove.y > 0.0f)
        {
            direction = EvadeDirection.Forward;

            if (inputMove.x < 0.0f)
                transform.Rotate(Vector3.up, -45.0f);
            else if (inputMove.x > 0.0f)
                transform.Rotate(Vector3.up, +45.0f);
        }
        else
        {
            direction = EvadeDirection.Backward;
        }

        animator.SetInteger("Direction", (int)direction);
        animator.SetTrigger("Evade");
    }

    //ȸ�� �� ó��
    private void End_Evade()
    {
        state.SetIdleMode();
    }
}