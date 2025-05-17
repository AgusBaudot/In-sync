using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IAttackable
{
    private int _maxHealth = 100;
    public int _currentHp { get; private set; }
    public event Action OnAttackedEvent;
    [SerializeField] private Canvas _canvas;

    private void Start()
    {
        _currentHp = _maxHealth;
    }

    public void OnDeath()
    {
        _canvas.transform.GetChild(1).gameObject.SetActive(true);
        Destroy(gameObject);
    }

    public void OnAttacked(int damageAmount)
    {
        _currentHp -= damageAmount;
        OnAttackedEvent?.Invoke();
        if (_currentHp <= 0)
        {
            OnDeath();
        }
    }
}
