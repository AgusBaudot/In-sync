using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomCollider : MonoBehaviour
{
    public event Action<Collider> OnWallCollision;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RoomWall"))
        {
            WallCollisionCaller(other);
        }
    }

    public void WallCollisionCaller(Collider other)
    {
        OnWallCollision?.Invoke(other);
    }
}
