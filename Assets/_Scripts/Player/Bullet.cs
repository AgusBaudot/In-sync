using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed;
    private readonly int _lifeSpan = 3;
    private float _lifeTime = 0;

    public void Init(Vector3 dir)
    {
        _rb.velocity = dir * _speed;
        transform.rotation = Quaternion.Euler(30, 45, 0);
    }

    private void Update()
    {
        if (_lifeTime < _lifeSpan)
        {
            _lifeTime += Time.deltaTime;
        }
        if (_lifeTime >= _lifeSpan)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent<IAttackable>(out IAttackable attackable))
        {
            attackable.OnAttacked(Random.Range(1, 11));
        }
        Destroy(gameObject);
    }
}
