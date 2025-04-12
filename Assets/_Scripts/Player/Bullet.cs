using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed; //Remove serialized
    private readonly int _lifeSpan = 3;
    private float _lifeTime = 0;

    public void Init(Vector3 dir, bool isOverchaged) //Bullet constructor.
    {
        if (isOverchaged) transform.localScale *= 3; //If bullet is overcharged, increase its size 3 times.
        _rb.velocity = dir * _speed; //Set constant speed as vector for direction.
        transform.rotation = Quaternion.Euler(30, 45, 0); //Set rotation to face camera.
    }

    private void Update()
    {
        _lifeTime += Time.deltaTime; //Tick down lifeTime Timer.
        if (_lifeTime >= _lifeSpan) //If lifeTime timer reaches lifeSpan of bullet:
            Destroy(gameObject); //Destroy GO
    }

    private void OnCollisionEnter(Collision collision) //If GO collides with anything:
    {
        if (collision.transform.TryGetComponent<IAttackable>(out IAttackable attackable)) //Check if other is attackable
        {
            attackable.OnAttacked(Random.Range(1, 11)); //If it is, attack enemy.
        }
        Destroy(gameObject); //Destroy bullet when collision happens.
    }
}
