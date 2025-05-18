using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    private int _damage = 0;
    public void DoDamage()
    {
        if (_damage == 0) Debug.Log("Damage is 0!");
        if (Vector3.Distance(_player.transform.position, transform.position) <= //If distance between player and enemy by the time
            transform.parent.parent.GetComponent<MeleeEnemy>().GetAttackRange()) //the enemy attacks if less than enemy's atk range:
        {
            _player.GetComponent<IAttackable>().OnAttacked(_damage);
            Debug.Log("attacked");
        }
        Debug.Log(Vector3.Distance(_player.transform.position, transform.position));
    }

    public void SetDamage(int dmg)
    {
        _damage = dmg;
    }
}
