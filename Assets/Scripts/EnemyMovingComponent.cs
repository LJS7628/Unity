using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyMovingComponent : MonoBehaviour
{

    private Animator animator;
    private HealthPointComponent healthPoint;
    private GameObject player;

    private float chaseRadius = 5.0f;

    private Vector3 direction;
    private Vector3 randomPoint;
    private float speed = 2.0f;

    private float stopDistance = 0.5f;
    private float waitTime = 2.0f;

    private bool isMoving=true;
    void Start()
    {
        animator = GetComponent<Animator>();
        healthPoint = GetComponent<HealthPointComponent>();
        player = GameObject.Find("Player");

        GetRandomPosition();
    }



    void Update()
    {
        animator.SetFloat("SpeedY", speed);

        if (healthPoint.IsDead == false)
        {
            if (isMoving)
            {
                Moving();

                if (Vector3.Distance(transform.position, randomPoint) < stopDistance)
                {
                    StartCoroutine(WaitAndMoveAgain());
                }
            }

            else if (CheckNearPlayer(chaseRadius))
            {
                Chasing();
            }


        }
    }


    private Collider[] colliders;
    bool CheckNearPlayer(float radius)
    {
        colliders = Physics.OverlapSphere(transform.position, radius);

       
        foreach (Collider collider in colliders)
        {
            if (collider.name == "Player")
                return true;
        }

        return false;
    }

    bool CheckNearWall()
    {
        colliders = Physics.OverlapSphere(transform.position, 0.5f);


        foreach (Collider collider in colliders)
        {
            if (collider.tag == "wall")
                return true;
        }

        return false;
    }

    void GetDirection(Vector3 target)
    {
        direction = (target - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;
    }

    void GetRandomPosition() 
    {
        if (CheckNearWall())
            GetRandomPosition();

        float x = Random.Range(-5.0f, 5.0f);
        float y = Random.Range(-5.0f, 5.0f);
        randomPoint = new Vector3(transform.position.x+x, transform.position.y, transform.position.z+y);

    }
    void Moving()
    {
        GetDirection(randomPoint);
        transform.position = Vector3.MoveTowards(transform.position, randomPoint, speed * Time.deltaTime);
    }

    void Chasing()
    {
        speed = 2.0f;
        GetDirection(player.transform.position);
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    IEnumerator WaitAndMoveAgain()
    {
        speed = 0.0f;
        isMoving = false;
        yield return new WaitForSeconds(waitTime);

        GetRandomPosition();
        isMoving = true;
        speed = 2.0f;

    }

}
