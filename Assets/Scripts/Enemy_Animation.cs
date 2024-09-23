using UnityEngine;

public partial class Enemy
{
    private float attackDistance = 2.0f;
    private void Update_Attacking() 
    {
        swordObject.transform.parent.DetachChildren();

        swordObject.transform.position = Vector3.zero;
        swordObject.transform.rotation = Quaternion.identity;
        swordObject.transform.localScale = Vector3.one;

        swordObject.transform.SetParent(handTransform, false);

        if (Vector3.Distance(transform.position, player.transform.position) < attackDistance)
        {
            animator.SetBool("Attack", true);
        }
        else 
        {
            animator.SetBool("Attack", false);
        }    
    }

}
