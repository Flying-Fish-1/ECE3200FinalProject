using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackAttempt : MonoBehaviour
{
    public Animator animator;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
        animator.SetBool("tryAttack", true);
        }
        Invoke("ResetTryAttack", 0.1f);
    }

    public void ResetTryAttack()
    {
        animator.SetBool("tryAttack", false);
    }
}
