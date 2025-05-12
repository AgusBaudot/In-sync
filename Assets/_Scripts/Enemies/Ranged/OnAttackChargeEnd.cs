using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAttackChargeEnd : MonoBehaviour
{
    public void OnAttackChargeEndEvent()
    {
        transform.parent.parent.GetComponent<RangedEnemy>().FireProjectile();
    }
}
