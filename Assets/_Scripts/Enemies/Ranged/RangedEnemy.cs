using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RangedEnemy : Enemy
{
    public override void HandleAttacking(float distance)
    {
        float dot = Vector3.Dot(transform.forward, (_player.position - transform.position).normalized);
        //if (dot > 0.9f) return;
        _rb.velocity = Vector3.zero;

        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            Attack();
            _lastAttackTime = Time.time;
        }
        var attackTime = _anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "Armature|attackShoot")?.length ?? 0f;
        attackTime += _anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "Armature|attackCharge")?.length ?? 0f;
        StartCoroutine(CooldownAfterAttack(attackTime, distance));
    }
    
    public void FireProjectile()
    {
        Debug.Log("Change this for actual attack");
    }

    public void MoveKnockback()
    {
        _rb.MovePosition(_rb.position + _anim.deltaPosition);
    }
}
