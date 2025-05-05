using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class test : MonoBehaviour
{
    #region Variables
    public event Action<GameObject> OnTimeEnds;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private LayerMask _floorLayer;
    private float _speed = 15;
    private readonly int _lifeSpan = 3;
    private float _lifeTime = 0;
    private readonly List<Vector3> _primitiveVectors = new List<Vector3>
    {
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back,
        new Vector3(1, 0, -1).normalized,
        new Vector3(-1, 0, 1).normalized,
        new Vector3(1, 0, 1).normalized,
        new Vector3(-1, 0, -1).normalized
    };
    private float[] _vectorDistance = new float[8];
    public float _testRotation;
    #endregion

    public void Init(Vector3 pos, Vector3 dir) //Bullet constructor.
    {
        transform.position = pos;
        _rb.velocity = (gameObject.layer == 3) ? dir * _speed : dir * _speed * 1.5f; //Set constant speed as vector for direction.
    } //DON'T TOUCH UNLESS BULLETS DON'T WORK
    private void Update()
    {
        _lifeTime += Time.deltaTime; //Tick down lifeTime Timer.
        if (_lifeTime >= _lifeSpan) //If lifeTime timer reaches lifeSpan of bullet:
        {
            Default();
        }
        var mousePos = Helpers.Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(mousePos, out hit, Mathf.Infinity, _floorLayer))
        {
            Vector3 direction = new Vector3(hit.point.x, 0, hit.point.z);
            transform.rotation = Quaternion.LookRotation(hit.point, Vector3.up);
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
        _rb.velocity = Vector3.zero;
        transform.localPosition = Vector3.up * 2;
        _lifeTime = 0;
        transform.rotation = Quaternion.identity;
        transform.localScale = (gameObject.layer == 3) ? Vector3.one * 0.2f : Vector3.one * 0.4f; //Set scale to 0.2 for normal bullets. 0.6 for overcharged ones.
        OnTimeEnds?.Invoke(gameObject);
    } //DON'T TOUCH UNLESS BULLETS DON'T WORK

    //private Vector3 CheckClosest(Vector3 myVector)
    //{
    //    for (int i = 0; i < _primitiveVectors.Count; i++) //Iterate through all vectors
    //    {
    //        _vectorDistance[i] = Vector3.Distance(_primitiveVectors[i], myVector); //Calculate distance between each of the 8 directions and mouse position.
    //    }
    //    return _primitiveVectors[Array.IndexOf(_vectorDistance, Mathf.Min(_vectorDistance))];
    //    //Return vector from vectorList whose index is the same as the minimum value of distanceArray.
    //}
}
