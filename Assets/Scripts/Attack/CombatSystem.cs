using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    // 定义委托
    public delegate void AttackEventHandler(GameObject attacker, GameObject target, int damage);

    // 定义静态事件
    public static event AttackEventHandler OnPlayerAttackHit;  // 玩家攻击命中事件
    public static event AttackEventHandler OnEnemyAttackHit;   // 敌人攻击命中事件

    // 触发玩家攻击事件
    public static void TriggerPlayerAttack(GameObject attacker, GameObject target, int damage)
    {
        OnPlayerAttackHit?.Invoke(attacker, target, damage);
    }

    // 触发敌人攻击事件
    public static void TriggerEnemyAttack(GameObject attacker, GameObject target, int damage)
    {
        OnEnemyAttackHit?.Invoke(attacker, target, damage);
        //Debug.Log("Enemy Attack is Triggered");
        Debug.Log("damage: " + damage);
    }
}
