using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyAttack : MonoBehaviour
{
    public Collider2D swordCollider;
    public int damage = 30;
    public float knockbackForce = 1000f;

    public void AttackAction()
    {
        print(swordCollider.enabled);
        swordCollider.enabled = true;
        print(swordCollider.enabled);
        // transform.localPosition = rightAttackOffset;
    }

    public void StopAttack()
    {
        swordCollider.enabled = false;
        Debug.Log("Stopping Heavy Attack!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
    //     print(other);
    //     print(other.tag == "Enemy");
        if (other.tag == "Enemy")
        {
            IDamageable damageableObject = other.GetComponent<IDamageable>();
            print("null check");
            print(damageableObject != null);
            if (damageableObject != null) 
            {
                Vector3 parentPosition = gameObject.GetComponentInParent<Transform>().position;
                Vector2 direction = (Vector2)(other.gameObject.transform.position - parentPosition).normalized;
                Vector2 knockback = direction * knockbackForce;

                //Enemy enemy = other.GetComponent<Enemy>();
                //if (enemy != null)
                //{
                //    enemy.Health -= damage;
                //}
                Debug.Log("Heavy Attacking Enemy!");
                damageableObject.OnHit(damage, knockback);
                //other.SendMessage("OnHit", damage, knockback);
            }
        }
    }
}
