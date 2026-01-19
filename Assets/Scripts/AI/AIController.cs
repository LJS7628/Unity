using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PerceptionComponent))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract partial class AIController : MonoBehaviour
{
    [Header(" - Attack")]
    [SerializeField]
    protected float attackDelay = 3.0f;

    [SerializeField]
    protected float attackDelayDeviation = 0.5f;

    [Header(" - Damaged")]
    [SerializeField]
    protected float damagedDelay = 1.5f;

    [SerializeField]
    protected float damagedDelayDeviation = 0.5f;

    [Header(" - UserInterface")]
    [SerializeField]
    private string uiPrefabName = "Enemy_AI_State";

    protected PerceptionComponent perception;
    protected NavMeshAgent navMeshAgent;
    private Animator animator;

    protected WeaponComponent weapon;
    protected StateComponent state;

    private Canvas uiCanvas;
    private TextMeshProUGUI uiText;
    protected virtual void Awake()
    {
        perception = GetComponent<PerceptionComponent>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
  
        state = GetComponent<StateComponent>();

        weapon = GetComponent<WeaponComponent>();
        weapon.OnEndEquip += OnEndEquip;
        weapon.OnEndDoAction += OnEndDoAction;

    }

    protected virtual void Start()
    {
        SetWaitMode();

        GameObject prefab = Resources.Load<GameObject>(uiPrefabName);
        GameObject obj = GameObject.Instantiate<GameObject>(prefab, transform);

        uiCanvas = obj.GetComponent<Canvas>();
        uiCanvas.worldCamera = Camera.main;

        uiText = obj.transform.FindChildByName("Text").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (uiCanvas != null)
        {
            if (uiText != null)
                uiText.text = $"{type.ToString()}({currentCoolTime.ToString("F3")})";

            uiCanvas.transform.rotation = Camera.main.transform.rotation;
        }
    }

    protected bool Check_FixedUpdate() 
    {
        if (DecreaseCoolTime() == true)
            return false;

        if (CheckMode() == true)
            return false;

        return true;
    }
    protected abstract void FixedUpdate();

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

    protected virtual void End_Damaged() 
    {
        SetCoolTime(damagedDelay,damagedDelayDeviation);
        SetWaitMode();
    }

    protected float currentCoolTime = 0.0f;
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
