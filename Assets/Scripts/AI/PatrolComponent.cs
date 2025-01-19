using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PatrolComponent : MonoBehaviour
{
    [SerializeField]
    private float speed = 2.0f;
    public float Speed =>speed;

    [SerializeField]
    private float radius = 10; //순찰반경

    [SerializeField]
    private float goalDelay = 2; //목표지점 도달시 대기시간

    [SerializeField]
    private float goalDelayDeviation = 0.5f; //랜덤 편차

    [SerializeField]
    private PatrolPoints patrolPoints;

    public bool HasPatroPoints => patrolPoints != null;



    private NavMeshAgent navMeshAgent;
    private NavMeshPath navMeshPath;

    private Vector3 initPosition;
    private Vector3 goalPosition;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

    }
    private void Start()
    {
        initPosition = goalPosition = transform.position;
    }

    public void StartMoving() 
    {
        if(navMeshPath == null)
            navMeshPath = CreateNavMeshPath();

        navMeshAgent.SetPath(navMeshPath);
    }

    private NavMeshPath CreateNavMeshPath() 
    {
        NavMeshPath path = null;
        

        if (HasPatroPoints) 
        {
            goalPosition = patrolPoints.GetMoveToPosition();

            path = new NavMeshPath();

            bool bCheck =navMeshAgent.CalculatePath(goalPosition, path);
            Debug.Assert(bCheck);

            patrolPoints.UpdateNextIndex();

            return path;
        }


        Vector3 prevGoalPosition = goalPosition;

        while (true) 
        {

            while (true)
            {
                float x = Random.Range(-radius * 0.5f, +radius * 0.5f);
                float z = Random.Range(-radius * 0.5f, +radius * 0.5f);

                goalPosition = new Vector3(x, 0, z) + initPosition;

                if (Vector3.Distance(goalPosition, prevGoalPosition) > radius * 0.25f)
                    break;
            }

             path = new NavMeshPath();

            if(navMeshAgent.CalculatePath(goalPosition, path))
                return path;
        }
    }

    private bool bArrived;
    private void Update()
    {
        
        if(navMeshPath == null)
            return;

        if(bArrived == true)
            return;

        float distance = Vector3.Distance(transform.position, goalPosition);

        if(distance >= navMeshAgent.stoppingDistance)
            return;

        bArrived = true;

        float waitTime = goalDelay + Random.Range(-goalDelayDeviation,+goalDelayDeviation);

        IEnumerator waitRoutine = WaitDelay(waitTime);
        StartCoroutine(waitRoutine);
    }

    private IEnumerator WaitDelay(float time) 
    {
        yield return new WaitForSeconds(time);

        bArrived  = false;
        navMeshPath = CreateNavMeshPath();
        navMeshAgent.SetPath(navMeshPath);
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if(Application.isPlaying == false)
            return;

        Vector3 from = transform.position + new Vector3(0.0f,0.1f,0.0f);
        Vector3 to = goalPosition + new Vector3(0.0f, 0.1f, 0.0f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(from, to);

        Gizmos.color= new Color(0,1,0,0.75f);
        Gizmos.DrawSphere(goalPosition, 0.5f);

        Gizmos.color = new Color(0, 0, 1, 0.75f);
        Gizmos.DrawSphere(initPosition, 0.5f);
    }
#endif
}
