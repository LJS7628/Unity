using UnityEngine;

public partial class Enemy
{
    //3frame = beginCombo;
    //12frame = endCombo;

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
        //animator.SetTrigger("NextCombo");
    }

    private void End_Attack()
    {

        moving.Move();
        animator.SetBool("Attack", false);
    }
}