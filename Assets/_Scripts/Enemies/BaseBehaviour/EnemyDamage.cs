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
        _player.GetComponent<PlayerHealth>().RecieveDamage(_damage);
    }

    public void SetDamage(int dmg)
    {
        _damage = dmg;
    }
}
