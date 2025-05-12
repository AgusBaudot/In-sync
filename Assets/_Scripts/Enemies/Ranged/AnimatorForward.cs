using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorForward : MonoBehaviour
{
    private void OnAnimatorMove()
    {
        transform.parent.parent.GetComponent<RangedEnemy>().MoveKnockback();
    }
}
