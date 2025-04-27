using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    public void DoDamage()
    {
        _player.GetComponent<PlayerHealth>().RecieveDamage(5);
    }
}
