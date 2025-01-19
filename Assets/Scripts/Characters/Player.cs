using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(WeaponComponent))]
[RequireComponent(typeof(PlayerInput))]  
[RequireComponent(typeof(PlayerMovingComponent))] 
public class Player : Character, IDamagable // 피해를 받을 수 있는 캐릭터
{
    private WeaponComponent weapon; //캐릭터와 무기,스킬 상태를 통신하기 위한 컴포넌트
    protected override sealed void Awake()
    {
        base.Awake();
        
        weapon = GetComponent<WeaponComponent>();
        Awake_BindInput();
    }

    private void Awake_BindInput() 
    {
        PlayerInput input = GetComponent<PlayerInput>();
        InputActionMap actionMap = input.actions.FindActionMap("PlayerAction");

        //Fist
        {
            actionMap.FindAction("Fist").started += context =>
            {
                weapon.SetFistMode();
            };
        }

        //Sword
        {
            actionMap.FindAction("Sword").started += context =>
            {
                weapon.SetSwordMode();
            };
        }

        //DualBlade
        {
            actionMap.FindAction("DualBlade").started += context =>
            {
                weapon.SetDualBladeMode();
            };
        }

        //Warp
        {
            actionMap.FindAction("Warp").started += context =>
            {
                weapon.SetWarpMode();
            };
        }

        //FireBall
        {
            actionMap.FindAction("FireBall").started += context =>
            {
                weapon.SetFireBallMode();
            };
        }

        //Action
        {
            actionMap.FindAction("Action").started += context =>
            {
                weapon.DoAction();
            };
        }

        //SwordSkill
        {
            actionMap.FindAction("SwordSkill").started += context =>
            {
                if(weapon.SwordMode ==false)
                    weapon.SetSwordMode(); 

                weapon.DoSkill();
            };
        }

        //FireBallSkill
        {
            actionMap.FindAction("FireBallSkill").started += context =>
            {
                if (weapon.FireBallMode == false)
                    weapon.SetFireBallMode();  

                weapon.DoSkill();
            };
        }

    }

    //Player 데미지 처리
    public void OnDamage(GameObject attacker, Weapon causer, Vector3 hitPoint, DoActionData data)
    {
        healthPoint.Damage(data.Power);  

        StartCoroutine(Start_FrameDelay(attacker, data.StopFrame)); 

        if (data.HitParticle != null) 
        {
            GameObject obj = Instantiate<GameObject>(data.HitParticle, transform, false);
            obj.transform.localPosition = hitPoint + data.HitParticlePositionOffset;
            obj.transform.localScale = data.HitParticleScaleOffset;

            Destroy(obj, 2);
        }

        if (healthPoint.Dead == false)   
        {
            state.SetDamagedMode();

            transform.LookAt(attacker.transform, Vector3.up); 

            animator.SetInteger("ImpactType", (int)causer.Type);
            animator.SetInteger("ImpactIndex", (int)data.ImpactIndex);
            animator.SetTrigger("Impact");

            if (data.Distance > 0.0f)
                StartCoroutine(Start_Lanch(30, data.Distance)); 

            return;

        }

        state.SetDeadMode();

        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        animator.SetTrigger("Dead");

    }

    //경직 처리
    private IEnumerator Start_FrameDelay(GameObject attacker, int frame)
    {
        Animator attackAnimator = attacker.GetComponent<Animator>();

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
