using UnityEngine;

public class SkillParticleTrigger : MonoBehaviour
{
    private GameObject player;
    private SphereCollider particleCollider;

    private void Awake()
    {
        player = GameObject.Find("Player");
        particleCollider = GetComponent<SphereCollider>();
    }
    private void Update()
    {
        if(Weapon.weaponAttacked == true) 
            particleCollider.center += new Vector3(0, 0, 0.1f);
    }

    //스킬 피해 처리
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            HealthPointComponent hp = other.GetComponent<HealthPointComponent>();
            Animator animator = other.GetComponent<Animator>();
            Collider collider = other.GetComponent<Collider>();
            AIController controller = other.GetComponent<AIController>();

            other.transform.LookAt(player.transform, Vector3.up);

            if (hp.Dead == false)
            {
                controller.SetDamageMode();

                animator.SetInteger("ImpactType", 2);
                animator.SetInteger("ImpactIndex", 1);
                animator.SetTrigger("Impact");

                hp.Damage(35.0f);

                return;
            }

            collider.enabled = false;
            controller.SetWaitMode();
            animator.SetTrigger("Dead");
        }


        if (other.CompareTag("Boss"))
        {
            HealthPointComponent hp = other.GetComponent<HealthPointComponent>();
            Animator animator = other.GetComponent<Animator>();
            Collider collider = other.GetComponent<Collider>();
            AIController_Boss controller = other.GetComponent<AIController_Boss>();

            other.transform.LookAt(player.transform, Vector3.up);

            if (hp.Dead == false)
            {
                controller.SetDamageMode();

                animator.SetInteger("ImpactType", 2);
                animator.SetInteger("ImpactIndex", 1);
                animator.SetTrigger("Impact");

                hp.Damage(35.0f);

                return;
            }

            collider.enabled = false;
            controller.SetWaitMode();
            animator.SetTrigger("Dead");
        }
    }
}
