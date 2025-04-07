using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayer, _structuresLayer;
    [SerializeField] private float _radious;
    [SerializeField] private Bullet _bulletPrefab;
    private PlayerController _playerControllerScript;

    private void Start()
    {
        _playerControllerScript = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (_playerControllerScript.IsUsingChip())
        {
            Collider[] collisions = Physics.OverlapSphere(transform.position, _radious, _enemyLayer);
            if (collisions.Length > 0)
            {
                foreach (var collided in collisions)
                {
                    collided.gameObject.GetComponentInParent<IAttackable>().OnAttacked(Random.Range(11, 21));
                }
            }
        }
        else
        {

            RaycastHit hit;
            var direction = Vector3.zero;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _structuresLayer))
            {
                direction = hit.point - transform.position;
                Instantiate(_bulletPrefab, transform.position, Quaternion.identity).Init(direction.normalized);
            }
        }
    }

    //View sphere as red in scene

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = new Color(1, 0, 0, 0.25f); //Red color
    //    Gizmos.DrawSphere(transform.position, _radious); //Draw sphere from child's position.
    //}
}
