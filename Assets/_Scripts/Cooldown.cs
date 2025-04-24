using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown : MonoBehaviour
{
    private float _duration, _timer;

    public Cooldown(float duration)
    {
        _duration = duration;
        _timer = 0f;
    }

    private void Start()
    {
        _timer = _duration;
    }

    public void Tick(float deltaTime)
    {
        if (_timer > 0)
            _timer -= deltaTime;
    }

    public bool IsReady => _timer <= 0f;

    public void Reset() => _timer = 0f;

    public float RemainingTime => Mathf.Max(_timer, 0f);

    public void SetDuration(float newDuration)
    {
        _duration = newDuration;
    }
}
