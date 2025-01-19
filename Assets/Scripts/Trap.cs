using UnityEngine;

public class Trap : MonoBehaviour
{

    //��Ʈ ��
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Player") 
        {
            HealthPointComponent hp = other.GetComponent<HealthPointComponent>();
            StateComponent state = other.GetComponent<StateComponent>();
            Animator animator = other.GetComponent<Animator>();
            hp.Damage(1.0f);

            if (hp.Dead == true) 
            {
                state.SetDeadMode();

                Collider collider = other.GetComponent<Collider>();
                collider.enabled = false;

                animator.SetTrigger("Dead");
            }
                
        }
            
    }

  
}
