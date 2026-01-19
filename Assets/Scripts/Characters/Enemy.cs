using System.Collections;
using UnityEngine;

public class Enemy : Character, IDamagable // 피해를 받을 수 있는 캐릭터
{

    private AIController aiController;  //적 AI 스크립트

    protected override void Awake() 
    {
        base.Awake();

        aiController = GetComponent<AIController>();
    }

    //Enemy 데미지 처리
    public void OnDamage(GameObject attacker, Weapon causer, Vector3 hitPoint, DoActionData data)
    {
        healthPoint.Damage(data.Power); 

        StartCoroutine(Start_FrameDelay(attacker,data.StopFrame));

        if (data.HitParticle != null)  
        { 
            GameObject obj = Instantiate<GameObject>(data.HitParticle,transform,false);
            obj.transform.localPosition = hitPoint + data.HitParticlePositionOffset;
            obj.transform.localScale = data.HitParticleScaleOffset;

            Destroy(obj,1);
        }

        if (healthPoint.Dead == false) 
        {
            aiController?.SetDamageMode();  // AI Contorller의 Damage 처리함수 
            state.SetDamagedMode();
            

            transform.LookAt(attacker.transform, Vector3.up);

            
            animator.SetInteger("ImpactType", (int)causer.Type);
            animator.SetInteger("ImpactIndex",(int)data.ImpactIndex);
            animator.SetTrigger("Impact");
            
            if (data.Distance > 0.0f )
                StartCoroutine(Start_Lanch(30, data.Distance)); 

            return;
           
        }


        state.SetDeadMode();
        
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        animator.SetTrigger("Dead");


        
    }

    //경직  처리
    private IEnumerator Start_FrameDelay(GameObject attacker, int frame) 
    {
        Animator attackAnimator =  attacker.GetComponent<Animator>();

        animator.speed = 0.0f;
        attackAnimator.speed = 0.0f;

        for (int i = 0; i < frame; i++)
            yield return new WaitForFixedUpdate();

        animator.speed = 1.0f;
        attackAnimator.speed = 1.0f;
    }

    // 런치 : 뒤로 밀림
    private IEnumerator Start_Lanch(int frame, float distance) 
    {
        rigidbody.isKinematic = false;
        float launch = rigidbody.drag * 10.0f;
        rigidbody.AddForce(-transform.forward * launch);

        for (int i = 0; i < frame; i++)
            yield return new WaitForFixedUpdate();

        rigidbody.isKinematic = true;
    }

    // 데미지 처리 후 IDLE 상태로 복구
    protected override void End_Damaged()
    {
        base.End_Damaged();

        state.SetIdleMode();
    }
}
