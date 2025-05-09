using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothTime;
    private Vector3 _offset;
    private Vector3 _currentVelocity;

    private void Awake()
    {
        _offset = transform.position - _target.position;
    }

    private void FixedUpdate()
    {
        if (_target != null)
        {
            Vector3 targetPosition = _target.position + _offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, _smoothTime);
        }
    }
}
