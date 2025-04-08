using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayer, _floorLayer;
    [SerializeField] private GameObject _bulletPrefab;
    private float _radious = 3;
    private PlayerController _playerControllerScript;

    #region Overcharged attack
    private bool _overchargedAttack = false;
    private float _specialAttackDuration = 0.5f;
    public float _specialAttackTimer;
    #endregion

    private void Start()
    {
        _playerControllerScript = GetComponent<PlayerController>(); //Assign PlayerController component.
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) //If player presses LMB:
        {
            Attack(); //Call attack.
        }
        if (_overchargedAttack) //If player can do an overcharged attack:
        {
            OverchargedAttackTimer(); //Tick down timer.
        }
    }

    public void Attack(float radious = 3)
    {
        if (_playerControllerScript.IsUsingChip()) //If player is using Chip:
        {
            Collider[] collisions = Physics.OverlapSphere(transform.position, radious, _enemyLayer); //Get all colliders with enemy layer in radious.
            if (collisions.Length > 0) //If any is inside Sphere:
            {
                foreach (var collided in collisions) //Iterate through all enemies found.
                {
                    collided.gameObject.GetComponentInParent<IAttackable>().OnAttacked(Random.Range(11, 21)); //Make them recieve damage.
                }
            }
        }
        else //If player is using Penny:
        {
            RaycastHit hit;
            var direction = Vector3.zero;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Set ray to where the mouse is in screen.
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _floorLayer)) //If point in screen is pointing to floor:
            {
                direction = hit.point - transform.position; //Set direction to a vector pointing towards point position.
                var bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity); //Instantiate bullet prefab.
                bullet.GetComponent<Bullet>().Init(direction.normalized, _overchargedAttack); //Set its velocity to move along vector normalized and tell if attack is overcharged.
            }
        }
    }

    public void SpecialAttack()
    {
        _overchargedAttack = true; //Set special ability to true.
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

    private void OnOverchargedAttackFinish() //Reset next shot and timescale to normal.
    {
        //Reset every value changed.
        Time.timeScale = 1;
        _overchargedAttack = false; //Set special ability to false.
    }

    //View sphere as red in scene

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f); //Red color
        Gizmos.DrawSphere(transform.position, _radious); //Draw sphere from child's position.
    }
}
