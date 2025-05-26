using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IAttackable
{
    private int _maxHealth = 100;
    public int _currentHp { get; private set; }
    public event Action OnAttackedEvent;
    [SerializeField] private GameObject _cursor;
    [SerializeField] private Canvas _canvas;

    private void Start()
    {
        _currentHp = _maxHealth;
    }

    public void OnDeath()
    {
        Cursor.visible = true;
        _cursor.SetActive(false);
        _canvas.transform.GetChild(1).gameObject.SetActive(true);
        Destroy(gameObject);
    }

    public void OnAttacked(int damageAmount)
    {
        CinemachineShake.Instance.ShakeCamera(0.75f, 0.3f); //Bigger camera shake.
        _currentHp -= damageAmount;
        OnAttackedEvent?.Invoke();
        if (_currentHp <= 0)
        {
            _currentHp = 0;
            OnDeath();
        }
    }
}
