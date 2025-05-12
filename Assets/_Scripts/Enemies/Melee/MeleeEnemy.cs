using System.Linq;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    public override void HandleAttacking(float distance)
    {
        _rb.velocity = Vector3.zero;

        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            Attack();
            _lastAttackTime = Time.time;
        }
        var attackTime = _anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "Cylinder_attack")?.length ?? 0f;
        StartCoroutine(CooldownAfterAttack(attackTime, distance));
    }
}