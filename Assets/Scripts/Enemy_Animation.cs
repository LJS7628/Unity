using UnityEngine;

public partial class Enemy
{

    private bool bComboEnable;
    private bool bComboExist;
    private bool bAttacking;

    private int comboIndex;
    public int ComboIndex { get => comboIndex; }

    private void Update_Attacking()
    {
            if (bComboEnable)
        {
            bComboEnable = false;
            bComboExist = true;

            return;
        }

        if (bAttacking == true)
            return;

        moving.Stop();
        bAttacking = true;
        animator.SetBool("Attack", true);
    }

    private void Begin_Collision()
    {
        sword.Begin_Collision();
    }

    private void End_Collision()
    {
        sword.End_Collision();
    }

    private void Begin_Combo()
    {
        bComboEnable = true;
    }

    private void End_Combo()
    {
        bComboEnable = false;
    }

    private void Begin_Attack()
    {
        if (bComboExist == false)
            return;

        bComboExist = false;
        comboIndex++;
        animator.SetTrigger("NextCombo");
     
    }

    private int attackCount = 0;
    private void End_Attack()
    {
        comboIndex = 0;
        attackCount ++;
        moving.Move();
        bAttacking = false;
        animator.SetBool("Attack", false);
    }

    private bool bBlocking;
    private void Update_Blocking() 
    {
        bool bCheck = false;
        bCheck |= (bAttacking == true);

        if (bCheck)
            return;

        if (bBlocking == true)
            return;

        bBlocking = true;
        animator.SetBool("Blocking", true);
    }

    private void End_Block()
    {
        Debug.Log("call");
        attackCount = 0;
        bBlocking = false;
        animator.SetBool("Blocking", false);

    }

}