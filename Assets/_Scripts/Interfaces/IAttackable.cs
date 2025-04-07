using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    public void OnAttacked(int damageReceived);

    public void OnDeath();
}
