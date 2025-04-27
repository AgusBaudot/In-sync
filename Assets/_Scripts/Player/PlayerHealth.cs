using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private int _maxHealth = 20;
    private int _currentHp;

    private void Start()
    {
        _currentHp = _maxHealth;
    }

    public void RecieveDamage(int damageAmount)
    {
        _currentHp -= damageAmount;
        if (_currentHp <= 0)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        Destroy(gameObject);
    }
}
