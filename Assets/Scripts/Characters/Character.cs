using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(StateComponent))] 
[RequireComponent(typeof(HealthPointComponent))] 

public class Character : MonoBehaviour
{
    protected Animator animator;
    protected new Rigidbody rigidbody;

    protected StateComponent state;
    protected HealthPointComponent healthPoint;
    protected virtual void Awake() 
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        healthPoint = GetComponent<HealthPointComponent>();
        state = GetComponent<StateComponent>();
    }

    protected virtual void Start()
    {

    }

    protected void OnAimatorMove()
    {
        transform.position += animator.deltaPosition; //Ä³¸¯ÅÍ ÀÌµ¿
    }

    protected virtual void End_Damaged()
    {

    }
    protected virtual void End_Dead() 
    {
        Destroy(gameObject, 2.0f);  //»ç¸Á½Ã °´Ã¼ ÆÄ±«
    }
}
