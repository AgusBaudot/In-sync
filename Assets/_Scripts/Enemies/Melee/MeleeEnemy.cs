using System.Linq;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    public override void HandleFollowing(float distance)
    {
        if (distance <= _attackRange)
        {
            _currentState = EnemyState.Attacking;
            if (!isKnockback)
                _rb.velocity = Vector3.zero;
            return;
        }
        if (_player.gameObject != null)
        {
            MoveTowards(_player.position);
        }
        else
        {
            if (!isKnockback)
                _rb.velocity = Vector3.zero;
            _currentState = EnemyState.Idle;
        }
    }

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

    public float GetAttackRange() => _attackRange;
}