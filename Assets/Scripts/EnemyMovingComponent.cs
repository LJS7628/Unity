using System.Collections;
using UnityEngine;

public class EnemyMovingComponent : MonoBehaviour
{

    private float moveRadius = 8f;
    private float stopDistance = 0.5f;
    private float chaseDistance = 4.0f;
    private float moveSpeed = 3f;
    private float waitTime = 1f;
    private Vector3 targetPosition;
    private Vector3 moveDirection;
    private bool isMoving = true;
    private bool bChase = false;
    private bool chased = false;

    private Animator animator;
    private HealthPointComponent healthPoint;
    private GameObject player;
    private float animatorSpeed = 2.0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        healthPoint = GetComponent<HealthPointComponent>();
        player = GameObject.Find("Player");
    }

    void Update()
    {

        animator.SetFloat("SpeedY", animatorSpeed);
        bChase = isChasing(player);
        if (healthPoint.IsDead == false)
        {

            if (isMoving & isChasing(player) == false)
            {
                GetDirection(targetPosition);
                MoveTowardsTarget();

                if (Vector3.Distance(transform.position, targetPosition) < stopDistance)
                {
                    StartCoroutine(WaitAndMoveAgain());
                }
            }

            else if (bChase)
            {
                animatorSpeed = 2.0f;
                GetDirection(player.transform.position);
                MoveTowardsTarget();
                chased = true;
            }

            if (bChase == false & chased)
            {
                StartCoroutine(WaitAndMoveAgain());
                chased = false;
            }

        }
    }


    void GetNewRandomPosition(Vector3 currentPosition)
    {

        Vector2 randomPoint2D = Random.insideUnitCircle * moveRadius;
        targetPosition = new Vector3(currentPosition.x + randomPoint2D.x, currentPosition.y, currentPosition.z + randomPoint2D.y);
        if (isMoveRange() == false)
            GetNewRandomPosition(transform.position);

        GetDirection(targetPosition);
    }


    void MoveTowardsTarget()
    {

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void GetDirection(Vector3 target)
    {
        moveDirection = (target - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = targetRotation;

    }


    IEnumerator WaitAndMoveAgain()
    {
        animatorSpeed = 0.0f;
        Stop();
        yield return new WaitForSeconds(waitTime);

        GetNewRandomPosition(transform.position);
        Move();
        animatorSpeed = 2.0f;

    }

    public void Move() 
    {
        isMoving = true;
    }

    public void Stop()
    {
        isMoving =false;
    }

    public void StopChase() 
    {
        bChase = false;
    }

    public bool isChasing(GameObject player)
    {
        if (Vector3.Distance(transform.position, player.transform.position) < chaseDistance)
            return true;

        return false;
    }

    bool isMoveRange()
    {
        Vector3 upRange = new Vector3(13.5f, 0, 0);
        Vector3 rightRange = new Vector3(0, 0, 13.5f);

        if (targetPosition.x > -upRange.x & targetPosition.x < upRange.x)
        {
            if (targetPosition.z > -rightRange.z & targetPosition.z < rightRange.z)
                return true;
        }

        return false;
    }

}
