using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EnemyState
{
    Idle,
    Following,
    Attacking,
    Stunned
}

[RequireComponent(typeof(Rigidbody))]
public abstract class Enemy : MonoBehaviour, IAttackable
{
    #region Serialized Fields
    [Header("Target")]
    [SerializeField] private protected Transform _player;

    [Header("Detection")]
    [SerializeField] private protected float _detectionRange = 10f;
    [SerializeField] private protected float _attackRange = 2f;
    [SerializeField] private RoomManager _roomManager;

    [Header("Movement")]
    [SerializeField] private protected float _moveSpeed = 3.5f;

    [Header("Patrol")]
    [SerializeField] private protected Transform[] _patrolPoints;

    [Header("Animation")]
    [SerializeField] private protected Animator _anim;

    [Header("Life")]
    [SerializeField] private protected int _hp = 100;

    [Header("Room")]
    [SerializeField] private int _room;

    [Header("Attack")]
    [SerializeField] private protected float _attackCooldown = 1.5f;
    [SerializeField] private protected GameObject _dmgTextPrefab;
    [SerializeField] private protected int _damage = 0;
    #endregion

    #region Private Fields
    private protected EnemyState _currentState = EnemyState.Idle;
    private protected float _lastAttackTime;
    private protected int _currentPatrolIndex = 0;
    private protected Rigidbody _rb;
    private protected bool isKnockback = false;
    private protected bool _isStunned = false;
    #endregion

    public event Action<Enemy> OnDeathEvent;
    public event Action OnAttackedEvent;

    #region Unity Methods

    private void Start()
    {
        _lastAttackTime = -_attackCooldown;
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_player == null) return;
        if (_isStunned) _currentState = EnemyState.Stunned;
        float distanceToPlayer = Vector3.Distance(_rb.position, _player.position);
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
            case EnemyState.Stunned:
                HandleStunned(distanceToPlayer);
                break;
        }
        _anim.SetBool("isMoving", _rb.velocity.magnitude > 0.1f);
    }

    private void LateUpdate()
    {
        if (!isKnockback)
            FaceVelocityDirection();
    }

    #endregion

    #region State Handlers
    public virtual void HandleIdle(float distance)
    {
        if (distance <= _detectionRange && _roomManager.GetActiveRoom() == _room)
        {
            _currentState = EnemyState.Following;
            return;
        }
    }

    public virtual void HandleFollowing(float distance)
    {
        if (distance > _detectionRange)
        {
            _currentState = EnemyState.Idle;
            if (!isKnockback)
                _rb.velocity = Vector3.zero;
            return;
        }

        if (distance <= _attackRange)
        {
            _currentState = EnemyState.Attacking;
            if (!isKnockback)
                _rb.velocity = Vector3.zero;
            return;
        }
        if (_player.gameObject != null)
        {
            //Vector3 moveDir = (_player.transform.position - transform.position);
            //_rb.AddForce(moveDir - _rb.velocity, ForceMode.VelocityChange);
            MoveTowards(_player.position);
        }
        else
        {
            if(!isKnockback)
                _rb.velocity = Vector3.zero;
            _currentState = EnemyState.Idle;
        }
    }

    public virtual void HandleAttacking(float distance)
    {
        if (!isKnockback)
            _rb.velocity = Vector3.zero; //Only set velocity to 0. Rest of attack logic is independent.
    }

    public virtual void HandleStunned(float distance)
    {
        if (!_isStunned)
        {
            if (distance > _detectionRange)
            {
                _currentState = EnemyState.Idle;
                if (!isKnockback)
                    _rb.velocity = Vector3.zero;
                return;
            }

            else if (distance <= _attackRange)
            {
                _currentState = EnemyState.Attacking;
                if (!isKnockback)
                    _rb.velocity = Vector3.zero;
                return;
            }

            else if (distance <= _detectionRange /*&& _roomManager.GetActiveRoom() == _room*/)
            {
                _currentState = EnemyState.Following;
                return;
            }
        }
    }
    #endregion

    #region Helper Methods
    private protected void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - _rb.position).normalized;
        Vector3 velocity = direction * _moveSpeed;
        velocity.y = _rb.velocity.y;
        _rb.velocity = velocity;
        //Vector3 desiredVelocity = direction * _moveSpeed;
        //Vector3 velocityChange = desiredVelocity - _rb.velocity;
        //_rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private protected void FaceVelocityDirection()
    {
        Vector3 horizontalVelocity = _rb.velocity;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);
            _rb.rotation = Quaternion.Slerp(_rb.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    private virtual protected IEnumerator CooldownAfterAttack(float time, float distance)
    {
        yield return Helpers.GetWait(time);
        if (distance > _attackRange)
        {
            _currentState = EnemyState.Following;
        }
        else if (distance > _detectionRange)
        {
            _currentState = EnemyState.Idle;
        }
    }

    public EnemyState GetEnemyState() => _currentState;

    public void Stun(float stunTime = 2f)
    {
        _isStunned = true;
        _currentState = EnemyState.Stunned;
        StartCoroutine(StunCooldown(stunTime));
    }

    private IEnumerator StunCooldown(float stunTime)
    {
        Debug.Log("Stun Started");
        yield return Helpers.GetWait(stunTime);
        _isStunned = false;
        Debug.Log("Stun finished");
    }
    #endregion

    #region Attack
    private protected void Attack()
    {
        _anim.SetTrigger("Attack");
    }

    public void OnAttacked(int damageReceived)
    {
        _hp -= damageReceived;
        OnAttackedEvent?.Invoke();
        var dmgTextCanvas = Instantiate(_dmgTextPrefab, transform.position + Vector3.up * 4, Quaternion.Euler(30, 45, 0)); //Instantiate damage text.
        dmgTextCanvas.transform.GetChild(0).GetComponent<DamageUI>().ShowDamage(damageReceived); //Call show damage method with damage received from player.
        if (_hp <= 0) OnDeath();
    }

    public void OnDeath()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        OnDeathEvent?.Invoke(this);
    }

    #endregion

    private protected void OnDrawGizmos()
    {
        //Gizmos.color = Color.black;
        //Gizmos.DrawWireSphere(transform.position, _detectionRange);
        //Gizmos.color = Color.white;
        //Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}

