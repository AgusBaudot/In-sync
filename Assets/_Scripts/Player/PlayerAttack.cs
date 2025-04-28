using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayer, _floorLayer;
    [SerializeField] private GameObject[] _bulletPool, _overchargedBulletPool;

    #region Overcharged attack
    private bool _overchargedAttack = false;
    private float _specialAttackDuration = 0.25f;
    private float _specialAttackTimer;
    #endregion

    #region Components
    private PlayerController _playerControllerScript;
    private Animator _chipAnim, _pennyAnim;
    #endregion

    #region AttackCD
    private bool _canAttackChip, _canAttackPenny;
    private float _currentPennyCD, _currentChipCD;
    float _pennyAttackCD = 0.35f;
    float _chipAttackCD = 0.5f;
    #endregion

    private void Start()
    {
        _canAttackChip = _canAttackPenny = true;
        _playerControllerScript = GetComponent<PlayerController>(); //Assign PlayerController component.
        _chipAnim = transform.GetChild(0).GetComponent<Animator>();
        _pennyAnim = transform.GetChild(1).GetComponent<Animator>();
        foreach (GameObject bullet in _bulletPool)
        {
            bullet.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) //If player presses LMB:
        {
            if ((_playerControllerScript.IsUsingChip() && _canAttackChip) || (!_playerControllerScript.IsUsingChip() && _canAttackPenny))
                Attack(); //Call attack.
        }

        else if (!_canAttackChip)
        {
            _currentChipCD -= Time.deltaTime;
            if (_currentChipCD <= 0)
            {
                _canAttackChip = true;
            }
        }

        else if (!_canAttackPenny)
        {
            _currentPennyCD -= Time.deltaTime;
            if (_currentPennyCD <= 0)
            {
                _canAttackPenny = true;
                if (Input.GetMouseButton(0))
                {
                    Attack();
                    _canAttackPenny = false;
                }
            }
        }

        if (_overchargedAttack) //If player can do an overcharged attack:
        {
            _pennyAnim.SetTrigger("OverchargedAttack");
            OverchargedAttackTimer(); //Tick down timer.
        }
    }

    public void Attack(float radious = 3)
    {
        if (_playerControllerScript.IsUsingChip()) //If player is using Chip:
        {
            _chipAnim.SetTrigger("Attacking");
            Collider[] collisions = Physics.OverlapSphere(transform.position, radious, _enemyLayer); //Get all colliders with enemy layer in radious.
            if (collisions.Length > 0) //If any is inside Sphere:
            {
                CinemachineShake.Instance.ShakeCamera(1.5f, 0.15f); //Camera shake.
                foreach (var collided in collisions) //Iterate through all enemies found.
                {
                    collided.gameObject.GetComponentInParent<IAttackable>().OnAttacked(Random.Range(11, 21)); //Make them recieve damage.
                }
            }
            _canAttackChip = false;
            _currentChipCD = _chipAttackCD;
        }
        else //If player is using Penny:
        {
            if (!_overchargedAttack) _pennyAnim.SetTrigger("Attacking");
            else _pennyAnim.SetTrigger("OverchargedShot");
            RaycastHit hit;
            var direction = Vector3.zero;
            var ray = Helpers.Camera.ScreenPointToRay(Input.mousePosition); //Set ray to where the mouse is in screen.
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _floorLayer)) //If point in screen is pointing to floor:
            {
                direction = hit.point - transform.position; //Set direction to a vector pointing towards point position.
                var bullet = NextBullet().GetComponent<Bullet>();
                bullet.Init((transform.position + (Vector3.up * 2)), direction.normalized); //Set its velocity to move along vector normalized and tell if attack is overcharged.
                bullet.OnTimeEnds += DeactivateBullet;
                OnOverchargedAttackFinish(); //After player shoots, finish overcharged state.
            }
            _canAttackPenny = false;
            _currentPennyCD = _pennyAttackCD;
        }
    }

    public void SpecialAttack()
    {
        _overchargedAttack = true; //Set special ability to true.
        _playerControllerScript.SetCanMove(false);
        _specialAttackTimer = _specialAttackDuration; //Set timer to full.
        Time.timeScale = 0.5f; //Halve game velocity
    }

    private void OverchargedAttackTimer()
    {
        _specialAttackTimer -= Time.deltaTime; //Tick down special abilty timer.

        if (_specialAttackTimer <= 0) //If timer has finished:
        {
            OnOverchargedAttackFinish(); //Reset values when ability ends.
        }
    }

    private void OnOverchargedAttackFinish()
    {
        //Reset every value changed.
        Time.timeScale = 1;
        _overchargedAttack = false; //Set special ability to false.
        _playerControllerScript.SetCanMove(true);
    }

    private GameObject NextBullet()
    {
        if (!_overchargedAttack)
        {
            foreach(GameObject bullet in _bulletPool)
            {
                if (!bullet.activeSelf)
                {
                    bullet.SetActive(true);
                    return bullet;
                }
            }
            return null;
        }
        else
        {
            foreach (GameObject overchargedBullet in _overchargedBulletPool)
            {
                if (!overchargedBullet.activeSelf)
                {
                    overchargedBullet.SetActive(true);
                    return overchargedBullet;
                }
            }
            return null;
        }
    }

    private void DeactivateBullet(GameObject bullet)
    {
        bullet.SetActive(false);
    }

    //View sphere as red in scene
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = new Color(1, 0, 0, 0.25f); //Red color
    //    Gizmos.DrawSphere(transform.position, _radious); //Draw sphere from child's position.
    //}
}
