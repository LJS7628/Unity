using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PerceptionComponent_Boss))]
[RequireComponent(typeof(NavMeshAgent))]
public partial class AIController_Boss : MonoBehaviour
{
    [Header(" - Speed")]
    [SerializeField]
    private float partolSpeed = 1.0f;

    [SerializeField]
    private float approachSpeed = 5.0f;   

    [Header(" - Attack")]
    [SerializeField]
    private float attackRange = 2.5f;

    [SerializeField]
    private float attackDelay = 1.0f;

    [SerializeField]
    private float attackDelayDeviation = 0.5f;

    [Header(" - Damaged")]
    [SerializeField]
    private float damagedDelay = 1.0f;

    [SerializeField]
    private float damagedDelayDeviation = 0.5f;

    private PerceptionComponent_Boss perception;
    private PatrolComponent patrol;
    private WeaponComponent weapon;
    private StateComponent state;

    private Animator animator;
    private NavMeshAgent navMeshAgent;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        perception = GetComponent<PerceptionComponent_Boss>();
        weapon = GetComponent<WeaponComponent>();
        state = GetComponent<StateComponent>();

        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        rigidbody = GetComponent<Rigidbody>();  

        weapon.OnEndEquip += OnEndEquip;
        weapon.OnEndDoAction += OnEndDoAction;
    }

    private void Start()
    {
        SetWaitMode();

    }

    private void Update()
    {

    }


    private void FixedUpdate()
    {
        if(DecreaseCoolTime() == true)
            return;

        if(CheckMode() == true)
            return;

        GameObject player = perception.GetPercievedPlayer();

        if (player == null)
        {
            if (weapon.UnarmedMode == false)
                weapon.SetUnarmedMode();

            if (patrol == null)
            {
                SetWaitMode();

                return;
            }

            SetPatrolMode();

            return;


        }

        if (weapon.UnarmedMode) 
        {
            SetEquipMode(WeaponType.Sword);
            return;
        }

        float temp = Vector3.Distance(transform.position, player.transform.position);
        if(temp < attackRange)
        {
           SetBossMode();

            return;
        }

        if(state.DeadMode == false) 
            SetApproachMode();
    }

    private void LateUpdate()
    {
        LateUpdate_Approach();

        animator.SetFloat("SpeedY", navMeshAgent.velocity.magnitude);
        animator.SetFloat("SpeedZ", navMeshAgent.velocity.magnitude);
    }

    private void LateUpdate_Approach() 
    { 
        if(ApproachMode == false)
            return;

        GameObject player = perception.GetPercievedPlayer();

        if(player == null)  
            return;

        navMeshAgent.SetDestination(player.transform.position);
    }
    
    private void OnEndEquip() 
    { 
        SetWaitMode();
    }

    private void OnEndDoAction() 
    {
        SetCoolTime(attackDelay, attackDelayDeviation);
        SetWaitMode();
    }

    private void End_Damaged() 
    {
        SetCoolTime(damagedDelay,damagedDelayDeviation);
        SetWaitMode();
    }

    private float currentCoolTime = 0.0f;
    private void SetCoolTime(float delayTime, float delayTimeDeviation) 
    {
        if (currentCoolTime < 0.0f) 
        {
            currentCoolTime = 0.0f;
            return;
        }

        float time = 0.0f;
        time += delayTime;
        time += UnityEngine.Random.Range(-delayTimeDeviation, +delayTimeDeviation);

        currentCoolTime = time;
    }
}
