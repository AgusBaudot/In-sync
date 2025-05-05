using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    #region Variables
    public event Action<GameObject> OnTimeEnds;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private LayerMask _floorLayer;
    private float _speed = 15;
    private readonly int _lifeSpan = 3;
    private float _lifeTime = 0;
    #endregion

    public void Init(Vector3 pos, Vector3 dir) //Bullet constructor.
    {
        transform.position = pos;
        _rb.velocity = (gameObject.layer == 3) ?  dir * _speed : dir * _speed * 1.5f; //Set constant speed as vector for direction.
        var mousePos = Helpers.Camera.ScreenPointToRay(Input.mousePosition); //Get mouse position in world.
        RaycastHit hit;
        if (Physics.Raycast(mousePos, out hit, Mathf.Infinity, _floorLayer)) //If mouse is aiming into the shootable screen:
        {
            Vector3 direciton = hit.point - transform.position; //Calulate the direction between the current bullet position and the target position.
            transform.rotation = Quaternion.LookRotation(direciton, Vector3.up); //Apply that direction as rotation of GO.
        }
    } //DON'T TOUCH UNLESS BULLETS DON'T WORK
    private void Update()
    {
        _lifeTime += Time.deltaTime; //Tick down lifeTime Timer.
        if (_lifeTime >= _lifeSpan) //If lifeTime timer reaches lifeSpan of bullet:
        {
            Default();
        }
    } //DON'T TOUCH UNLESS BULLETS DON'T WORK
    private void OnTriggerEnter(Collider collision) //If GO collides with anything:
    {
        if (collision.transform.TryGetComponent(out IAttackable attackable)) //Check if other is attackable
        {
            CinemachineShake.Instance.ShakeCamera(0.35f, 0.15f); //Camera shake.
            attackable.OnAttacked(UnityEngine.Random.Range(1, 11)); //If it is, attack enemy.
        }
        if (gameObject.layer == 3) Default(); //Only default normal bullets with any collision.
        if (collision.gameObject.layer == 13) Default(); //Default every bullet upon hitting a wall.
    } //DON'T TOUCH UNLESS BULLETS DON'T WORK
    private void Default()
    { 
        _rb.velocity = Vector3.zero; //Reset velocity to 0.
        transform.localPosition = Vector3.up * 2; //Reset position to bullet pool.
        _lifeTime = 0; //Reset lifetime timer.
        transform.rotation = Quaternion.identity; //Reset rotation of GO.
        transform.localScale = (gameObject.layer == 3) ? Vector3.one * 0.2f : Vector3.one * 0.4f; //Set scale to 0.2 for normal bullets. 0.6 for overcharged ones.
        OnTimeEnds?.Invoke(gameObject); //Fire event after reseting bullet.
    } //DON'T TOUCH UNLESS BULLETS DON'T WORK
}
