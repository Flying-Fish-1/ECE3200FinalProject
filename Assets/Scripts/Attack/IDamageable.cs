using UnityEngine;

public interface IDamageable
{
    // public int Health { set; get; }

    public void OnHit(int damage, Vector2 knockback);

}