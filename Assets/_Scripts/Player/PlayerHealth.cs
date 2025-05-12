using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    private int _maxHealth = 100;
    public int _currentHp { get; private set; }
    public event Action OnAttacked;
    [SerializeField] private Canvas _canvas;

    private void Start()
    {
        _currentHp = _maxHealth;
    }

    public void RecieveDamage(int damageAmount)
    {
        _currentHp -= damageAmount;
        OnAttacked?.Invoke();
        if (_currentHp <= 0)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        _canvas.transform.GetChild(1).gameObject.SetActive(true);
        Destroy(gameObject);
    }
}
