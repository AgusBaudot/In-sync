using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoorAnimationEvent : MonoBehaviour
{
    public event Action OnAnimationEndEvent;
    public void OnAnimationEnd()
    {
        OnAnimationEndEvent?.Invoke();
    }
}
