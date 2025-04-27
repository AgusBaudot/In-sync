using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Following,
    Attacking
}

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour, IAttackable
{
    #region Serialized Fields
    [Header("Target")]
    [SerializeField] private Transform _player;

    [Header("Detection")]
    [SerializeField] private float _detectionRange = 10f;
    [SerializeField] private float _attackRange = 2f;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 3.5f;

    [Header("Attack")]
    [SerializeField] private float _attackCooldown = 1.5f;
    [SerializeField] private GameObject _dmgTextPrefab;

    [Header("Patrol")]
    [SerializeField] private Transform[] _patrolPoints;

    [Header("Animation")]
    [SerializeField] private Animator _anim;
    #endregion

    #region Private Fields
    private EnemyState _currentState = EnemyState.Idle;
    private float _lastAttackTime;
    private int _currentPatrolIndex = 0;
    private Rigidbody _rb;
    private int _hp = 100;
    #endregion

    #region Unity Methods

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _lastAttackTime = -_attackCooldown;
    }

    private void Update()
    {
        if (_player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        switch (_currentState)
        {
            case EnemyState.Idle:
                HandleIdle(distanceToPlayer);
                break;

            case EnemyState.Following:
                HandleFollowing(distanceToPlayer);
                break;

            case EnemyState.Attacking:
                HandleAttacking(distanceToPlayer);
                break;
        }
        _anim.SetBool("isMoving", _rb.velocity.magnitude > 0.1f);
    }

    private void LateUpdate()
    {
        FaceVelocityDirection();
    }

    #endregion

    #region State Handlers

    private void HandleIdle(float distance)
    {
        if (distance <= _detectionRange)
        {
            _currentState = EnemyState.Following;
            return;
        }

        Patrol();
    }

    private void HandleFollowing(float distance)
    {
        if (distance > _detectionRange)
        {
            _currentState = EnemyState.Idle;
            _rb.velocity = Vector3.zero;
            return;
        }
        //For melee enemy, if _player is detected, enemy won't stop following him until death or kill.


        if (distance <= _attackRange)
        {
            _currentState = EnemyState.Attacking;
            _rb.velocity = Vector3.zero;
            return;
        }
        if (_player.gameObject != null)
        {
            MoveTowards(_player.position);
        }
        else
        {
            _rb.velocity = Vector3.zero;
            _currentState = EnemyState.Idle;
        }
    }

    private void HandleAttacking(float distance)
    {
        _rb.velocity = Vector3.zero;

        if (distance > _attackRange)
        {
            _currentState = EnemyState.Following;
            return;
        }

        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            Attack();
            _lastAttackTime = Time.time;
        }
    }

    #endregion

    #region Helper Methods

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 velocity = direction * _moveSpeed;
        velocity.y = _rb.velocity.y;
        _rb.velocity = velocity;
    }

    private void FaceVelocityDirection()
    {
        Vector3 horizontalVelocity = _rb.velocity;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    private void Patrol()
    {
        //if (_patrolPoints.Length == 0)
        //{
        //    _rb.velocity = Vector3.zero;
        //    return;
        //}

        //Transform targetPoint = _patrolPoints[_currentPatrolIndex];
        //MoveTowards(targetPoint.position);

        //float distance = Vector3.Distance(transform.position, targetPoint.position);
        //if (distance <= _attackRange)
        //{
        //    Debug.Log(distance);
        //    _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
        //}
    }
    #endregion

    #region Attack
    private void Attack()
    {
        _anim.SetTrigger("Attack");
    }

    public void OnAttacked(int damageReceived)
    {
        _hp -= damageReceived;
        var dmgTextCanvas = Instantiate(_dmgTextPrefab, transform.position + Vector3.up * 4, Quaternion.Euler(30, 45, 0)); //Instantiate damage text.
        dmgTextCanvas.transform.GetChild(0).GetComponent<DamageUI>().ShowDamage(damageReceived); //Call show damage method with damage received from player.
        if (_hp <= 0) OnDeath();
    }

    public void OnDeath()
    {
        Destroy(gameObject);
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}

