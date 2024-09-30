using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyMovingComponent : MonoBehaviour
{

    private Animator animator;
    private HealthPointComponent healthPoint;
    private GameObject player;

    private float chaseRadius = 3.0f;

    private Vector3 direction;
    private Vector3 randomPoint;
    private float animationSpeed = 2.0f;

    private float speed = 2.0f;

    private float stopDistance = 0.5f;
    private float waitTime = 2.0f;

    private bool bCanMove = true;
    private bool chased = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        healthPoint = GetComponent<HealthPointComponent>();
        player = GameObject.Find("Player");

        GetRandomPosition();
    }



    void Update()
    {
        animator.SetFloat("SpeedY", animationSpeed);

        if (healthPoint.IsDead == false)
        {
            if (bCanMove)
            {
                Moving();

                if (Vector3.Distance(transform.position, randomPoint) < stopDistance)
                {
                    StartCoroutine(WaitAndMoveAgain());
                }

                if (CheckNearPlayer(chaseRadius))
                {
                    chased = true;
                    Chasing();
                }

                if (CheckNearPlayer(chaseRadius) == false & chased)
                {
                    StartCoroutine(StopChase());
                }
            }

        }

        if (Player.isPlayerDead) 
        {
            bCanMove = false;
            animationSpeed = 0.0f;
        }
            
    }


    private Collider[] colliders;
    public bool CheckNearPlayer(float radius)
    {
        colliders = Physics.OverlapSphere(transform.position, radius);


        foreach (Collider collider in colliders)
        {
            if (collider.name == "Player")
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
        float x = Random.Range(-13.0f, +13.0f);
        float y = Random.Range(-13.0f, +13.0f);
        randomPoint = new Vector3(x, 0.0f, y);

    }
    void Moving()
    {
        if (chased)
            return;
        GetDirection(randomPoint);
        speed = 2.0f;
        transform.position = Vector3.MoveTowards(transform.position, randomPoint, speed * Time.deltaTime);
    }

    void Chasing()
    {
        speed = 2.0f;
        animationSpeed = 2.0f;
        GetDirection(player.transform.position);
        if (Vector3.Distance(transform.position, player.transform.position) >= 1.5f)
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    IEnumerator WaitAndMoveAgain()
    {
        animationSpeed = 0.0f;
        bCanMove = false;
        yield return new WaitForSeconds(waitTime);

        GetRandomPosition();
        bCanMove = true;
        animationSpeed = 2.0f;

    }

    IEnumerator StopChase()
    {
        animationSpeed = 0.0f;
        yield return new WaitForSeconds(waitTime);
        chased = false;
        animationSpeed = 2.0f;
    }

    public void Move()
    {
        bCanMove = true;
    }
    public void Stop()
    {
        bCanMove = false;
    }

}