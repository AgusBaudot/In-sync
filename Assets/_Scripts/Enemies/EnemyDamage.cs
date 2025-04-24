using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    public void DoDamage()
    {
        Debug.Log("test");
        //Do damage to player.
        //_player.GetComponent<PlayerHealth>().SomeFunction();
    }
}
