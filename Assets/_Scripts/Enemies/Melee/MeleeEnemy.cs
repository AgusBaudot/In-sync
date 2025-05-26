using System.Collections;
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
        if (!_rb.isKinematic)
            _rb.velocity = Vector3.zero;

        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            Attack();
            MeleeAttack();
            _lastAttackTime = Time.time;
        }
        var attackTime = _anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "Cylinder_attack")?.length ?? 0f;
        StartCoroutine(CooldownAfterAttack(attackTime, distance));
    }

    private protected override IEnumerator CooldownAfterAttack(float time, float distance)
    {
        _rb.isKinematic = true;
        yield return Helpers.GetWait(time);
        _rb.isKinematic = false;
        _currentState = EnemyState.Following;
    }

    private void MeleeAttack()
    {
        if (Vector3.Distance(_player.transform.position, transform.position) <= //If distance between player and enemy by the time
            _attackRange + 0.5f) //the enemy attacks if less than enemy's atk range:
        {
            _player.GetComponent<IAttackable>().OnAttacked(_damage);
        }
    }

    public float GetAttackRange() => _attackRange;
}