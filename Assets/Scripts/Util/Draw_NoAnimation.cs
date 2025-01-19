using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw_NoAnimation : StateMachineBehaviour
{
    private WeaponComponent weapon;
    private bool bExcution;

    //���� ����� ���� ���⿡ ���� �ִϸ��̼� �� ���� ó��
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if(weapon == null)  
            weapon = animator.GetComponent<WeaponComponent>();

        bExcution = true;

        weapon.Begin_Equip();
    }
     public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);  

        if(bExcution == false)
            return;

        bExcution=false;
        weapon.End_Equip();
    }

}
