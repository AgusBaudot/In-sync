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
    private bool _canMove = true;
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
        _chip = true; //By default start with chip.
        CharacterSwap(); //Call character swap to only use Chip in scene.
        _rb = GetComponent<Rigidbody>(); //Get RigidBody component.
        _playerAttackScript = GetComponent<PlayerAttack>(); //Get PlayerAttack component.
        _speed = _currentSprintCD = 5; //Set speed and currentspeed to 5.
    }

    private void Update()
    {
        GatherInput(); //First we detect user inputs.
        Look(); //Then we look towards input.
        if (_isSprinting) SprintTimer(); //If player is sprinting, call sptint timer.
    }

    private void FixedUpdate()
    {
        if (_canMove) Move(); //Handle physics.
    }

    private void GatherInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)) //If player presses space:
        {
            if (_chip) //If player is using chip:
            {
                VelAmplifier(); //Call vel amplifier.
            }
            else if (!_chip && _canBlink) //If player is using Penny and can blink:
            {
                _canBlink = false; //He can't blink anymore.
                StartCoroutine(Blink()); //Start blink Coroutine.
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) //If player pressed Q:
        {
            _chip = !_chip; //Change chip value.
            CharacterSwap(); //Call character swap.
        }

        if (!_canBlink) //If player can't blink:
        {
            BlinkTimer(); //Call blink timer.
        }

        if (!_isSprinting) //If player isn't sprinting:
        {
            _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")); //Use WASD as movement.
        }

        else //If he is:
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Store mouse position as Ray.
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _floorRay)) //Determine where player is pointing mouse to:
            {
                if (Vector3.Distance(hit.point, transform.position) <= 1.1f) //If distance between player and mouse is too little:
                {
                    _input = Vector3.zero; //Don't move.
                    return;
                }
                _input = hit.point - transform.position; //Set input to distance between mouse position and player's
            }
        }
        _input = _input.normalized; //Normalize input's magnitude.
    }

    private void Look()
    {
        if (_input != Vector3.zero) //If player is trying to move:
        {   //If player is sprinting, use input from mouse position. If not, use input with isometric position.
            Vector3 relative = _isSprinting ? (transform.position + _input) - transform.position : (transform.position + _input.ToIso()) - transform.position; //Stores relative angle.
            Quaternion rot = Quaternion.LookRotation(relative, transform.up); //Rotate towards relative in y axis.

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime); //Rotate towards input.
        }
    }

    private void Move() => _rb.MovePosition(transform.position + (transform.forward * _input.magnitude) * _speed * Time.deltaTime); //Move player.

    private void SprintTimer()
    {
        _currentSprintCD -= Time.deltaTime; //Tick down sprint timer.

        if (_currentSprintCD <= 0) //If sprint has no time left:
        {
            _speed = _normalSpeed; //Set speed to normal.
            _isSprinting = false; //Set sprinting to false.
            _isOvercharged = false; //Set overcharged to false.
            _counter = 0; //Set counter to 0.
        }
        else //If player still keeps sprinting:
        {
            _speed = _normalSpeed + _counter/_maxCounter * (_maxSpeed - _normalSpeed); //Aument speed depending on how many times the player has pressed space.
        }
    }

    private void VelAmplifier()
    {
        _isSprinting = true; //Set sprinting to true.
        _currentSprintCD = _sprintDuration; //Reset sprinting timer.
        if (_counter < _maxCounter) _counter++; //Add to counter if it is less than max.
        _isOvercharged = (_counter == _maxCounter); //Set overcharged to true if player has reached the max amount of space presses. Set to false otherwise.
    }

    private void BlinkTimer()
    {
        _currentBlinkCD -= Time.deltaTime; //Tick down timer.

        if (_currentBlinkCD <= 0) //If timer reaches 0:
        {
            _canBlink = true; //Player can blink.
            _currentBlinkCD = _blinkCD; //Reset blink timer.
        }
    }

    private IEnumerator Blink() //Penny's blink ability
    {
        GetComponent<BoxCollider>().enabled = false; //Deactivate player's collider.
        var selectedChild = transform.GetChild(1).gameObject; //Store penny's GO into selectedChild.
        selectedChild.SetActive(false); //Deactivate penny's GO.
        _isOvercharged = true; //Set overcharged to true.
        yield return Helpers.GetWait(_blinkTime); //Wait blink time before doing anything more.
        _isOvercharged = false; //Set overcharged to false.
        transform.Translate(_blinkDistance * Vector3.forward); //Move GO the blink distance.
        GetComponent<BoxCollider>().enabled = true; //Enable collider.
        selectedChild.SetActive(!_chip); //Set penny's GO if player is using it.
        transform.GetChild(0).gameObject.SetActive(_chip); //Set chip's GO if player is using it.
        if (_chip) _playerAttackScript.Attack(5); //After teleporting, perform automatic attack of radious 5.
        //Should be on CharacterSwap(). Figure out a way to execute after blink is finished. (Blinked is IEnumerator).
    }

    private void CharacterSwap()
    {
        transform.GetChild(0).gameObject.SetActive(_chip); //Enable chip when using it, disable it otherwise.
        transform.GetChild(1).gameObject.SetActive(!_chip); //Enable penny when using it, disable it otherwise.
        _currentSprintCD = 0; //Set sprint timer to 0 to stop sprinting.
        _speed = _normalSpeed; //Make maybe slow down instead of sudden in future.
        if (_isOvercharged) //If player was overcharged when doing swap:
        {
            if (_chip) //If overcharge is with chip:
            {
                transform.GetChild(0).gameObject.SetActive(false); //Deactivate GO so it doesn't appear mid-blink.
                //_playerAttackScript.Attack(5); Should be here. Figure out a way to execute after blink is finished. (Blinked is IEnumerator).
            }
            else //If overcharge is with penny:
            {
                _playerAttackScript.SpecialAttack(); //Call her special attack.
            }
        } 
    }

    public bool IsUsingChip() => _chip;

    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }

    //public bool IsOverCharged() => _isOvercharged;
}
