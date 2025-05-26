using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBullet : MonoBehaviour
{
    #region Variables
    public event Action<GameObject> OnTimeEnds;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed = 15;
    [SerializeField] private int _damage;
    private readonly int _lifeSpan = 3;
    private float _lifeTime = 0;
    #endregion

    public void Init(Vector3 pos, Vector3 dir) //Bullet constructor.
    {
        transform.position = pos;
        _rb.velocity = dir.normalized * _speed;
        transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        //Vector3 direciton = dir - transform.position; //Calulate the direction between the current bullet position and the target position.
        //transform.rotation = Quaternion.LookRotation(direciton, Vector3.up); //Apply that direction as rotation of GO.
    }
    private void Update()
    {
        _lifeTime += Time.deltaTime; //Tick down lifeTime Timer.
        if (_lifeTime >= _lifeSpan) //If lifeTime timer reaches lifeSpan of bullet:
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision) //If GO collides with anything:
    {
        if (collision.transform.TryGetComponent(out IAttackable attackable)) //Check if other is attackable
        {
            attackable.OnAttacked(_damage); //If it is, attack enemy.
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        
    }
}
