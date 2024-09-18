using System.Collections;
using UnityEngine;

public class EnemyMovingComponent : MonoBehaviour
{

    public float moveRadius = 1f;  // 이동 반경
    public float stopDistance = 0.5f;  // 도착 후 정지 거리
    public float moveSpeed = 3f;  // 몬스터 이동 속도
    public float waitTime = 1f;  // 도착 후 멈추는 시간
    private Vector3 targetPosition;  // 목표 위치
    private Vector3 moveDirection;  // 이동 방향 벡터
    private bool isMoving = true;  // 이동 상태 확인

    private Animator animator;
    private HealthPointComponent healthPoint;
    private float animatorSpeed=2.0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        healthPoint = GetComponent<HealthPointComponent>();

        GetNewRandomPosition(transform.position);  // 첫 랜덤 위치 생성
        animator.SetFloat("SpeedY", animatorSpeed);

    }

    void Update()
    {
        
        animator.SetFloat("SpeedY", animatorSpeed);
        if (isMoving & healthPoint.IsDead == false)
        {
            
            // 목표 위치로 이동
            MoveTowardsTarget();
            // 몬스터가 목표 지점에 도착했는지 체크
            if (Vector3.Distance(transform.position, targetPosition) < stopDistance)
            {
                // 도착했으면 잠시 멈추고 다시 움직이기 위한 Coroutine 실행
                StartCoroutine(WaitAndMoveAgain());
            }

        }
    }

    // 일정 반경 내의 랜덤 위치 구하기
    void GetNewRandomPosition(Vector3 currentPosition)
    {
        // 2D 랜덤 좌표 구한 후, 이를 3D로 변환
        Vector2 randomPoint2D = Random.insideUnitCircle * moveRadius;
        targetPosition = new Vector3(currentPosition.x + randomPoint2D.x, currentPosition.y, currentPosition.z + randomPoint2D.y);
        if(isMoveRange() == false)
            GetNewRandomPosition(transform.position);
        // 목표 지점까지의 방향 계산
        moveDirection = (targetPosition - transform.position).normalized;  // 방향 벡터 정규화
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = targetRotation;
    }

    // 목표 지점으로 이동
    void MoveTowardsTarget()
    {
        // 일정 속도로 목표 방향으로 이동
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    // 도착 후 잠깐 멈췄다가 다시 움직이는 Coroutine
    IEnumerator WaitAndMoveAgain()
    {
        animatorSpeed = 0.0f;
        isMoving = false;  // 일단 멈춤
        yield return new WaitForSeconds(waitTime);  // waitTime 만큼 기다림

        GetNewRandomPosition(transform.position);  // 새로운 랜덤 목표 위치 설정
        isMoving = true;  // 다시 움직임
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
