using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private const int _turnSpeed = 360;
    private Vector3 _input;
    private bool _chip;
    [SerializeField] LayerMask _floorRay;

    #region Sprint
    private bool _isSprinting = false;
    private float _speed, _currentSprintCD, _counter;
    private const int _maxCounter = 6;
    private const int _sprintCD = 5;
    private const int _normalSpeed = 5;
    [Range(10f, 30f)]
    [SerializeField] private float _maxSpeed;
    #endregion

    #region Blink
    private const int _blinkCD = 2;
    private int _blinkDistance = 7;
    private float _currentBlinkCD = _blinkCD;
    private bool _canBlink = true;
    #endregion

    private void Start()
    {
        _chip = true;
        CharacterSwap();
        _rb = GetComponent<Rigidbody>();
        _speed = _currentSprintCD = 5;
    }

    private void Update()
    {
        GatherInput();
        Look();
        if (_isSprinting) DashTimer();
        if (Input.GetKeyDown(KeyCode.Q)) //Key to swap characters. Implement swap CD (or condition) later.
        {
            _chip = !_chip;
            CharacterSwap();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GatherInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)) //If player presses space and is using chip then amplify its velocity.
        {
            if (_chip)
            {
                _isSprinting = true;
                VelAmplifier();
            }
            else if (!_chip && _canBlink)
            {
                _canBlink = false;
                Blink(); //0 to 7. 7 to 21. 21 to 49. 49 to 105.
            }
        }

        if (!_canBlink)
        {
            BlinkTimer();
        }

        if (!_isSprinting)
        {
            _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }

        else
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _floorRay))
            {
                if (Vector3.Distance(hit.point, transform.position) <= 1.1f)
                {
                    _input = Vector3.zero;
                    return;
                }
                _input = hit.point - transform.position;
            }
        }
        _input = _input.normalized;
    }

    private void Look()
    {
        if (_input != Vector3.zero)
        {   //If player is sprinting, use input from mouse position. If not, use input with isometric position.
            Vector3 relative = _isSprinting ? (transform.position + _input) - transform.position : (transform.position + _input.ToIso()) - transform.position; //Stores relative angle.
            Quaternion rot = Quaternion.LookRotation(relative, transform.up); //Rotate towards relative in y axis.

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
        }
    }

    private void Move() => _rb.MovePosition(transform.position + (transform.forward * _input.magnitude) * _speed * Time.deltaTime);

    private void DashTimer()
    {
        _currentSprintCD -= Time.deltaTime;

        if (_currentSprintCD <= 0)
        {
            _speed = _normalSpeed;
            _isSprinting = false;
            _counter = 0;
        }
        else
        {
            _speed = _normalSpeed + _counter/_maxCounter * (_maxSpeed - _normalSpeed);
        }
    }

    private void VelAmplifier()
    {
        _currentSprintCD = _sprintCD;
        if (_counter == _maxCounter) return;
        _counter++;
    }

    private void BlinkTimer()
    {
        _currentBlinkCD -= Time.deltaTime;

        if (_currentBlinkCD <= 0)
        {
            _canBlink = true;
            _currentBlinkCD = _blinkCD;
        }
    }

    private void Blink()
    {
        transform.Translate(_blinkDistance * Vector3.forward);
    }

    private void CharacterSwap()
    {
        transform.GetChild(0).gameObject.SetActive(_chip); //First child is chip. Enable when player has swapped to chip.
        transform.GetChild(1).gameObject.SetActive(!_chip); //Second child is chipn't. Enable then player has swapped to chipn't.
        _currentSprintCD = 0;
        _speed = _normalSpeed; //Make maybe slow down instead of sudden in future.
    }

    public bool IsUsingChip() => _chip;
}
