using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RangedEnemy : Enemy
{
    [SerializeField, Range(0f, 1f)] private float attackFacingThreshold = 0.95f;
    [SerializeField] private float _rotationSpeed = 7.5f;
    [SerializeField] private GameObject _bulletPrefab;
    [Header("Knockback")]
    [SerializeField] private float _knockbackDuration;

    private Vector3 _playerPositionWhenAttacked;
    private bool _isChargingAttack = false;

    public override void HandleAttacking(float distance)
    {
        if (_isChargingAttack) return;
        #region Rotate towards player
        Vector3 toPlayer = (_player.position - transform.position).normalized;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(toPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _rotationSpeed * Time.deltaTime);
        }
        #endregion

        #region Is enemy facing player?
        float facingDot = Vector3.Dot(transform.forward, toPlayer);

        if (facingDot < attackFacingThreshold)
        {
            return;
        } 
        #endregion

        if(!isKnockback)
            _rb.velocity = Vector3.zero;

        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            _isChargingAttack = true;
            Attack();
            _lastAttackTime = Time.time;
            _playerPositionWhenAttacked = _player.position;
        }
        var attackTime = _anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "Armature|attackShoot")?.length ?? 0f;
        attackTime += _anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "Armature|attackCharge")?.length ?? 0f;
        StartCoroutine(CooldownAfterAttack(attackTime, distance));
    }

    public void FireProjectile()
    {
        var bullet = Instantiate(_bulletPrefab);
        bullet.GetComponent<EnemyBullet>().Init((transform.position + Vector3.up), (_playerPositionWhenAttacked - transform.position).normalized);
        _isChargingAttack = false;
        MoveKnockback();
    }

    public void MoveKnockback()
    {
        if (_currentState != EnemyState.Attacking) return;
        Vector3 awayFromPlayer = (transform.position - _playerPositionWhenAttacked).normalized;
        awayFromPlayer.y = 0;
        _rb.AddForce(awayFromPlayer * 6f, ForceMode.VelocityChange);
        isKnockback = true;
        Invoke(nameof(EndKnockback), _knockbackDuration);
    }

    private void EndKnockback()
    {
        isKnockback = false;
        _rb.velocity = Vector3.zero;
    }

    private new void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Vector3 forward = transform.forward * 2f;
        //Quaternion leftRayRotation = Quaternion.AngleAxis(36f, Vector3.up); // ~0.8 dot
        //Quaternion rightRayRotation = Quaternion.AngleAxis(-36f, Vector3.up);
        //Gizmos.DrawRay(transform.position, leftRayRotation * forward);
        //Gizmos.DrawRay(transform.position, rightRayRotation * forward);

        float coneAngle = Mathf.Acos(attackFacingThreshold) * Mathf.Rad2Deg;
        Vector3 forward = transform.forward;

        // Visualize the forward facing angle as two rays
        Quaternion leftRayRotation = Quaternion.AngleAxis(-coneAngle, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(coneAngle, Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        float rayLength = 3f;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, leftRayDirection * rayLength);
        Gizmos.DrawRay(transform.position, rightRayDirection * rayLength);
        Gizmos.DrawRay(transform.position, forward * rayLength); // Forward line
    }
}
