using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    public event Action OnAttackedEvent;

    public void OnAttacked(int damageReceived);

    public void OnDeath();
}
