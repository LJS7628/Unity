using System.Collections;
using UnityEngine;

public class EnemyMovingComponent : MonoBehaviour
{

    public float moveRadius = 1f;  // �̵� �ݰ�
    public float stopDistance = 0.5f;  // ���� �� ���� �Ÿ�
    public float moveSpeed = 3f;  // ���� �̵� �ӵ�
    public float waitTime = 1f;  // ���� �� ���ߴ� �ð�
    private Vector3 targetPosition;  // ��ǥ ��ġ
    private Vector3 moveDirection;  // �̵� ���� ����
    private bool isMoving = true;  // �̵� ���� Ȯ��

    private Animator animator;
    private HealthPointComponent healthPoint;
    private float animatorSpeed=2.0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        healthPoint = GetComponent<HealthPointComponent>();

        GetNewRandomPosition(transform.position);  // ù ���� ��ġ ����
        animator.SetFloat("SpeedY", animatorSpeed);

    }

    void Update()
    {
        
        animator.SetFloat("SpeedY", animatorSpeed);
        if (isMoving & healthPoint.IsDead == false)
        {
            
            // ��ǥ ��ġ�� �̵�
            MoveTowardsTarget();
            // ���Ͱ� ��ǥ ������ �����ߴ��� üũ
            if (Vector3.Distance(transform.position, targetPosition) < stopDistance)
            {
                // ���������� ��� ���߰� �ٽ� �����̱� ���� Coroutine ����
                StartCoroutine(WaitAndMoveAgain());
            }

        }
    }

    // ���� �ݰ� ���� ���� ��ġ ���ϱ�
    void GetNewRandomPosition(Vector3 currentPosition)
    {
        // 2D ���� ��ǥ ���� ��, �̸� 3D�� ��ȯ
        Vector2 randomPoint2D = Random.insideUnitCircle * moveRadius;
        targetPosition = new Vector3(currentPosition.x + randomPoint2D.x, currentPosition.y, currentPosition.z + randomPoint2D.y);
        if(isMoveRange() == false)
            GetNewRandomPosition(transform.position);
        // ��ǥ ���������� ���� ���
        moveDirection = (targetPosition - transform.position).normalized;  // ���� ���� ����ȭ
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = targetRotation;
    }

    // ��ǥ �������� �̵�
    void MoveTowardsTarget()
    {
        // ���� �ӵ��� ��ǥ �������� �̵�
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    // ���� �� ��� ����ٰ� �ٽ� �����̴� Coroutine
    IEnumerator WaitAndMoveAgain()
    {
        animatorSpeed = 0.0f;
        isMoving = false;  // �ϴ� ����
        yield return new WaitForSeconds(waitTime);  // waitTime ��ŭ ��ٸ�

        GetNewRandomPosition(transform.position);  // ���ο� ���� ��ǥ ��ġ ����
        isMoving = true;  // �ٽ� ������
        animatorSpeed = 2.0f;

    }

    bool isMoveRange() 
    {
        Vector3 upRange = new Vector3(15, 0, 0);
        Vector3 rightRange = new Vector3(0, 0, 15);

        if (targetPosition.x > -upRange.x & targetPosition.x < upRange.x) 
        {
            if (targetPosition.z > -rightRange.z & targetPosition.z < rightRange.z)
                return true;
        }
           
        return false;
    }

}
