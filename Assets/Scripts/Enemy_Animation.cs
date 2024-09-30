using UnityEngine;

public partial class Enemy
{

    private bool bComboEnable;
    private bool bComboExist;

    private int comboIndex;
    public int ComboIndex { get => comboIndex; }
    private void Update_Attacking()
    {
        moving.Stop();
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


    private void Begin_Attack()
    {
        //if (bComboExist == false)
        //    return;

        //bComboExist = false;

        comboIndex++;
        animator.SetTrigger("NextCombo");
    }

    private void End_Attack()
    {
        comboIndex = 0;
        animator.SetBool("Attack", false);
    }

    private void Begin_Combo()
    {
        bComboEnable = true;
    }

    private void End_Combo()
    {
        bComboEnable = false;
    }
}