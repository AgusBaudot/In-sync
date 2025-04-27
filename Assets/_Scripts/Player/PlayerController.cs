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
    [SerializeField] LayerMask _floorRay, _wallLayer;
    private bool _chip;
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
    [SerializeField] private ParticleSystem _blinkParticlesEffect;
    #endregion

    #region Swap
    private bool _canSwap = true;
    private float _currentSwapCD;
    private readonly int _swapCD = 1;
    #endregion

    #region Components
    private Rigidbody _rb;
    private Animator _chipAnim, _pennyAnim;
    private PlayerAttack _playerAttackScript;
    #endregion

    public LayerMask _walkableLayer;

    private void Start()
    {
        _chip = true; //By default start with chip.
        CharacterSwap(); //Call character swap to only use Chip in scene.
        _playerAttackScript = GetComponent<PlayerAttack>(); //Get PlayerAttack component.
        _speed = _currentSprintCD = 5; //Set speed and currentspeed to 5.
        _rb = GetComponent<Rigidbody>(); //Get RigidBody component.
        _chipAnim = transform.GetChild(0).GetComponent<Animator>();
        _pennyAnim = transform.GetChild(1).GetComponent<Animator>();
        _chipAnim.applyRootMotion = false;
        _pennyAnim.applyRootMotion = false;
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
                ParticleInstantiation();
                StartCoroutine(Blink(() =>
                {
                    if (_chip)
                        _playerAttackScript.Attack(5);
                }));
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canSwap) //If player pressed shift:
        {
            _chip = !_chip; //Change chip value.
            CharacterSwap(); //Call character swap.
        }

        else if (!_canSwap)
        {
            _currentSwapCD -= Time.deltaTime;
            if (_currentSwapCD <= 0)
            {
                _canSwap = true;
            }
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
            var ray = Helpers.Camera.ScreenPointToRay(Input.mousePosition); //Store mouse position as Ray.
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _floorRay)) //Determine where player is pointing mouse to:
            {
                if (Vector3.Distance(hit.point, transform.position) <= 1.1f) //If distance between player and mouse is too little:
                {
                    return; //avoid recalculing input this frame.
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

    private void Move()
    {
        Vector3 moveDir = transform.forward * _input.magnitude * _speed;
        moveDir = new Vector3(moveDir.x, _rb.velocity.y, moveDir.z); //Maintain vertical velocity
        _rb.AddForce(moveDir - _rb.velocity, ForceMode.VelocityChange);
        if (_chip)
        {
            _chipAnim.SetBool("Walking", _input.magnitude != 0 && !_isSprinting);
        }
        else
        {
            _pennyAnim.SetBool("Walking", _input.magnitude != 0 && !_isSprinting);
        }
    } 

    private void SprintTimer()
    {
        _currentSprintCD -= Time.deltaTime; //Tick down sprint timer.

        if (_currentSprintCD <= 0) //If sprint has no time left:
        {
            _chipAnim.SetBool("Sprinting", false);
            _chipAnim.speed = 1;
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
        _chipAnim.SetBool("Sprinting", true);
        _isSprinting = true; //Set sprinting to true.
        _currentSprintCD = _sprintDuration; //Reset sprinting timer.
        if (_counter < _maxCounter) _counter++; //Add to counter if it is less than max.
        _chipAnim.speed = 0.5f + 0.8f * (_counter / _maxCounter);
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

    private IEnumerator Blink(Action onComplete) //Penny's blink ability
    {
        //STEP 0: enable overcharge ability and get children
        _isOvercharged = true;
        Transform chipModel = transform.GetChild(0);
        Transform pennyModel = transform.GetChild(1);
        GameObject activeModel = _chip ? chipModel.gameObject : pennyModel.gameObject;

        // STEP 1: Hide visuals immediately and spawn particle
        activeModel.SetActive(false);
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        // STEP 2: Calculate blink destination
        Vector3 direction = transform.forward;
        Vector3 targetPosition = transform.position + direction * _blinkDistance;


        if (!Physics.Raycast(targetPosition + Vector3.up, Vector3.down, out RaycastHit floorHit, Mathf.Infinity, _walkableLayer))
        {
            targetPosition = transform.position;
        }
        else if (Physics.Raycast(transform.position, direction, out RaycastHit wallHit, _blinkDistance, _wallLayer))
        {
            targetPosition = wallHit.point - direction * 0.5f;
        }

        // STEP 3: wait until blinkTime has passed and disable overcharge ability
        float timer = 0f;
        while (timer < _blinkTime)
        {
            timer += Time.deltaTime;
            yield return null; // Wait for frame
        }
        _isOvercharged = false;

        // STEP 4: Teleport before next frame renders
        _rb.MovePosition(targetPosition);
        _rb.AddForce(Vector3.zero - _rb.velocity, ForceMode.VelocityChange);
        yield return new WaitForFixedUpdate();

        // STEP 5: Re-enable collision
        collider.enabled = true;

        // STEP 6: Show correct model after frame.
        yield return null;
        chipModel.gameObject.SetActive(_chip);
        pennyModel.gameObject.SetActive(!_chip);

        onComplete?.Invoke();
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
        _canSwap = false;
        _currentSwapCD = _swapCD;
    }

    public bool IsUsingChip() => _chip;

    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }

    private void ParticleInstantiation()
    {
        var spawnPos = transform.position + Vector3.up * 1.5f;
        var particle = Instantiate(_blinkParticlesEffect, spawnPos, Quaternion.identity);
        particle.transform.localScale = Vector3.one; // force scale just in case
        particle.Play();
        Destroy(particle.gameObject, 0.5f);
    }
}
