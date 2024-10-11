using UnityEngine;

public partial class Player
{
    private bool bDrawing = false;
    private bool bSheathing = false;

    private bool bEquipped = false;

	private void Update_Drawing()
    {
        if (Input.GetButtonDown("Sword") == false)
            return;

        if (bDrawing == true)
            return;

        if (bSheathing == true)
            return;

        if (bAttacking == true)
            return;


        if (bEquipped == false)
        {
            bDrawing = true;
            animator.SetBool("Equip", true);

            return;
        }

        bSheathing = true;
        animator.SetBool("Unequip", true);
    }

    private void Begin_Equip() // animation event
    {
        swordObject.transform.parent.DetachChildren();

        swordObject.transform.position = Vector3.zero;
        swordObject.transform.rotation = Quaternion.identity;
        swordObject.transform.localScale = Vector3.one;

        swordObject.transform.SetParent(handTransform, false);
    }

    private void End_Equip() // animation event
    {
        bEquipped = true;

        bDrawing = false;
        animator.SetBool("Equip", false);
    }

    private void Begin_Unequip() // animation event
    {
        swordObject.transform.parent.DetachChildren();

        swordObject.transform.position = Vector3.zero;
        swordObject.transform.rotation = Quaternion.identity;
        swordObject.transform.localScale = Vector3.one;

        swordObject.transform.SetParent(holsterTransform, false);
    }

    private void End_Unequip() //animation event
    {
        bEquipped = false;

        bSheathing = false;
        animator.SetBool("Unequip", false);
    }


    private bool bAttacking;
    private bool bComboEnable;
    private bool bComboExist;
    
    private int comboIndex;
    public int ComboIndex { get => comboIndex; }

    public static bool playerAttack = false;
    private void Update_Attacking()
    {
        if (Input.GetButtonDown("Attack") == false)
            return;

        
        bool bCheck = false;
        bCheck |= (bEquipped == false);
        bCheck |= (bDrawing == true);
        bCheck |= (bSheathing == true);

        if (bCheck)
            return;

        if(bComboEnable)
        {
            bComboEnable = false;
            bComboExist = true;

            return;
        }


        if (bAttacking == true)
            return;


        moving.Stop();
        
        bAttacking = true;
        playerAttack = true;
        animator.SetBool("Attacking", true);
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

    private void End_Attack()
    {
        comboIndex = 0;


        moving.Move();
        
        bAttacking = false;
        playerAttack = false;
        animator.SetBool("Attacking", false);
    }

    private bool bBlocking;
    private void Update_Blocking() 
    {
        if (Input.GetButtonDown("Block") == false)
            return;

        bool bCheck = false;
        bCheck |= (bEquipped == false);
        bCheck |= (bDrawing == true);
        bCheck |= (bSheathing == true);

        if (bCheck)
            return;

        if (bBlocking == true)
            return;

        bBlocking = true;
        animator.SetBool("Blocking",true);
    }

    private void End_Block() 
    {
        bBlocking = false;
        animator.SetBool("Blocking", false);
    }

    public static bool bCounter = false;
    private void Update_Counter() 
    {
        if (Input.GetButtonDown("Counter") == false)
            return;

        bool bCheck = false;
        bCheck |= (bEquipped == false);
        bCheck |= (bDrawing == true);
        bCheck |= (bSheathing == true);

        if (bCheck)
            return;

        bCounter = true;
        moving.Stop();
        animator.SetTrigger("Counter");
    }

    private void End_Counter() 
    {
        moving.Move();
        bCounter = false;
    }
}