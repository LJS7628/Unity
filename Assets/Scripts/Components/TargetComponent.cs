using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetComponent : MonoBehaviour
{
    [Header(" - Trace")]
    [SerializeField]
    private bool bDebugLine; //Test 용

    [SerializeField]
    private float radius = 10; // 타겟 가능 반경

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private float rotateSpeed = 1.0f; //카메라 회전속도


    private PlayerMovingComponent moving;

    private GameObject targetObject;
    private Vector3? drawSpherePosition;

    private GameObject cursorPrefab;
    private GameObject cursorObject;

    private void Reset()
    {
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
    }

    private void Awake()
    {
        Awake_BindInput();

        moving = GetComponent<PlayerMovingComponent>();

        cursorPrefab = Resources.Load<GameObject>("TargetPoint");
    }

    //타겟팅 키 등록
    private void Awake_BindInput()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        InputActionMap actionMap = input.actions.FindActionMap("PlayerAction");


        InputAction action;

        action = actionMap.FindAction("Targeting");
        action.started += ToggleTarget;

        action = actionMap.FindAction("Targeting_Left");
        action.started += context => ChangeFocus(false);

        action = actionMap.FindAction("Targeting_Right");
        action.started += context => ChangeFocus(true);
    }

    private void Start()
    {

    }

    private void Update()
    {
        Update_Cursor();
        Update_Rotation();
    }

    //타겟팅 한 적에 마킹
    private void Update_Cursor()
    {
        if (cursorObject != null && targetObject != null)
        {
            Vector3 direction = transform.position - targetObject.transform.position;
            Vector3 position = targetObject.transform.position + direction.normalized * 0.3f;

            position.y = 1.25f;
            cursorObject.transform.position = position;
        }
    }


    private float deltaRotation;
    private bool bMovingFocus;

    //플레이어를 타켓팅한 적으로 시선 돌려주기
    private void Update_Rotation()
    {
        if (targetObject == null)
            return;


        HealthPointComponent healthPoint = targetObject.GetComponent<HealthPointComponent>();
        if (healthPoint != null)
        {
            if (healthPoint.Dead)
            {
                End_Targeting();

                return;
            }
        }

        if (Vector3.Distance(targetObject.transform.position, transform.position) > radius)
        {
            End_Targeting();

            return;
        }

        Vector3 forward = transform.forward;
        Vector3 position = transform.position;
        Vector3 targetPosition = targetObject.transform.position;

        Vector3 direction = targetPosition - position;


        Quaternion from = transform.rotation;
        Quaternion to = Quaternion.LookRotation(direction.normalized, Vector3.up);

        if (Quaternion.Angle(from, to) < 1.0f) //적과 플레이어가 거의 일직선에 있을 경우
        {
            bMovingFocus = false;

            deltaRotation = 0.0f;
            transform.localRotation = to;

            return;
        }

        deltaRotation += rotateSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.RotateTowards(from, to, deltaRotation);
    }

    private void ToggleTarget(InputAction.CallbackContext context)
    {
        if (targetObject == null)
        {
            Begin_Targeting();

            return;
        }

        End_Targeting();
    }

    //타겟팅 시작
    private void Begin_Targeting()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);

        if (IsInvoking("End_DrawSphere"))
            CancelInvoke("End_DrawSphere");

        drawSpherePosition = transform.position;
        Invoke("End_DrawSphere", 5);


        List<GameObject> candidateList = new List<GameObject>();
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<Enemy>() != null)
                candidateList.Add(collider.gameObject);
        }


        if (bDebugLine)
        {
            Vector3 position = transform.position;
            Debug.DrawLine(position, position + transform.forward * radius, Color.red, 5);

            foreach (GameObject candidate in candidateList)
            {
                Vector3 direction = candidate.transform.position - position;
                Debug.DrawLine(position, position + direction, Color.blue, 5);
            }
        }

        GameObject nearlyObject = GetNearlyFrontAngle(candidateList.ToArray());
        //Destroy(nearlyObject);

        ChangeTarget(nearlyObject);
    }

    private GameObject GetNearlyFrontAngle(GameObject[] candidates)
    {
        Vector3 position = transform.position;


        GameObject candidate = null;
        float maxAngle = float.MinValue;

        foreach (GameObject obj in candidates)
        {
            Vector3 enemyPosition = obj.transform.position;
            Vector3 direction = enemyPosition - position;

            float angle = Vector3.Dot(transform.forward, direction.normalized);
            if (angle < 1.0f - 0.5f)
                continue;


            if (maxAngle <= angle)
            {
                maxAngle = angle;
                candidate = obj;
            }
        }

        return candidate;
    }

    //타겟 변경
    private void ChangeTarget(GameObject candidate)
    {
        if (candidate == null)
        {
            End_Targeting();

            return;
        }

        if (cursorPrefab != null)
        {
            if (cursorObject != null)
                Destroy(cursorObject);

            cursorObject = Instantiate<GameObject>(cursorPrefab, candidate.transform);
        }


        targetObject = candidate;
        moving.SetLock();
    }

    //타겟팅 끝
    private void End_Targeting()
    {
        moving.Unlock(transform.forward);

        deltaRotation = 0.0f;
        targetObject = null;

        if (cursorObject != null)
            Destroy(cursorObject);

        if (IsInvoking("End_DrawSphere"))
            CancelInvoke("End_DrawSphere");
    }

    //타겟 변경을 위한 처리
    private void ChangeFocus(bool bRight)
    {
        if (targetObject == null)
            return;

        if (bMovingFocus == true)
            return;

        bMovingFocus = true;


        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);

        Dictionary<float, GameObject> candidateTable = new Dictionary<float, GameObject>();
        foreach (Collider collider in colliders)
        {
            if (targetObject == collider.gameObject)
                continue;


            Vector3 vec1 = collider.transform.position;
            Vector3 vec2 = transform.position;
            Vector3 direction = vec1 - vec2;

            Vector3 cross = Vector3.Cross(transform.forward, direction.normalized);
            float distance = Vector3.Dot(cross, Vector3.up);

            candidateTable.Add(distance, collider.gameObject);
        }


        float minimum = float.MaxValue;
        GameObject candidate = null;

        foreach (float distance in candidateTable.Keys)
        {
            if (Mathf.Abs(distance) >= minimum)
                continue;

            if (bRight && distance > 0.0f)
            {
                minimum = Mathf.Abs(distance);
                candidate = candidateTable[distance];
            }

            if (bRight == false && distance < 0.0f)
            {
                minimum = Mathf.Abs(distance);
                candidate = candidateTable[distance];
            }
        }

        ChangeTarget(candidate); //타겟 변경
    }

    //Gizmo에서 그리는 것 종료
    private void End_DrawSphere()
    {
        drawSpherePosition = null;
    }

    // Scene 테스트 용
    private void OnDrawGizmos()
    {
        if (drawSpherePosition == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(drawSpherePosition.Value, radius);
    }
}