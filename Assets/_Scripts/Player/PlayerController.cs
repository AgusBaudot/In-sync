using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Movement
    private readonly int _turnSpeed = 360;
    private Vector3 _input;
    private bool _isOvercharged = false;
    [SerializeField] LayerMask _floorRay;
    #endregion

    #region Sprint
    private bool _isSprinting = false;
    private float _speed, _currentSprintCD, _counter;
    private readonly int _maxCounter = 6;
    private readonly int _sprintDuration = 5;
    private readonly int _normalSpeed = 5;
    private readonly int _maxSpeed = 15;
    #endregion

    #region Blink
    private readonly int _blinkCD = 2;
    private int _blinkDistance = 7;
    private float _currentBlinkCD = 2;
    private bool _canBlink = true;
    private float _blinkTime = 0.25f;
    #endregion

    //Overchaged with chip: Player is going at max speed. Or, counter == maxCounter.
    //Overcharged with penny: Player has changed while in dash.

    private Rigidbody _rb;
    private bool _chip;
    private PlayerAttack _playerAttackScript;

    private void Start()
    {
        _chip = true;
        CharacterSwap();
        _rb = GetComponent<Rigidbody>();
        _playerAttackScript = GetComponent<PlayerAttack>();
        _speed = _currentSprintCD = 5;
    }

    private void Update()
    {
        GatherInput();
        Look();
        if (_isSprinting) SprintTimer();
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
                StartCoroutine(Blink());
            }
        }

        if (Input.GetKeyDown(KeyCode.Q)) //Key to swap characters. Implement swap CD (or condition) later.
        {
            _chip = !_chip;
            CharacterSwap();
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

    private void SprintTimer()
    {
        _currentSprintCD -= Time.deltaTime;

        if (_currentSprintCD <= 0)
        {
            _speed = _normalSpeed;
            _isSprinting = false;
            _isOvercharged = false;
            _counter = 0;
        }
        else
        {
            _speed = _normalSpeed + _counter/_maxCounter * (_maxSpeed - _normalSpeed);
        }
    }

    private void VelAmplifier()
    {
        _currentSprintCD = _sprintDuration;
        if (_counter < _maxCounter) _counter++;
        _isOvercharged = (_counter == _maxCounter);
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

    private IEnumerator Blink() //Penny's blink ability
    {
        GetComponent<BoxCollider>().enabled = false; //Deactivate player's collider.
        var selectedChild = transform.GetChild(1).gameObject; //Store penny gj into selectedChild.
        selectedChild.SetActive(false); //Deactivate penny gj.
        _isOvercharged = true; //Set overcharged to true.
        yield return Helpers.GetWait(_blinkTime); //Wait blink time before anything else
        _isOvercharged = false; //Set overcharged to false.
        transform.Translate(_blinkDistance * Vector3.forward);
        GetComponent<BoxCollider>().enabled = true;
        selectedChild.SetActive(!_chip);
        transform.GetChild(0).gameObject.SetActive(_chip);
        if (_chip) _playerAttackScript.Attack(5); //Should be on CharacterSwap(). Figure out a way to execute after blink is finished. (Blinked is IEnumerator).
    }

    private void CharacterSwap()
    {
        transform.GetChild(0).gameObject.SetActive(_chip); //First child is chip. Enable when player has swapped to chip.
        transform.GetChild(1).gameObject.SetActive(!_chip); //Second child is chipn't. Enable then player has swapped to penny.
        _currentSprintCD = 0;
        _speed = _normalSpeed; //Make maybe slow down instead of sudden in future.
        if (_isOvercharged)
        {
            if (_chip)
            {
                transform.GetChild(0).gameObject.SetActive(false);
                //_playerAttackScript.Attack(5); Should be here. Figure out a way to execute after blink is finished. (Blinked is IEnumerator).
            }
            else
            {
                _playerAttackScript.SpecialAttack();
            }
        } 
    }

    public bool IsUsingChip() => _chip;

    //public bool IsOverCharged() => _isOvercharged;
}
