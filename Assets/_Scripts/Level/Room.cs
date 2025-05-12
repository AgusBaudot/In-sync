using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Enemy> _enemies;
    [SerializeField] private List<Collider> _doors; //Non-trigger walls for closing the player inside each room.
    [SerializeField] private List<Collider> _roomDetector; //Trigger walls for deteting if the player steps inside a room.
    [SerializeField] private RoomCollider _colliderScript;

    private void Start()
    {
        foreach (Enemy enemy in _enemies)
        {
            if (enemy != null)
            {
                enemy.OnDeathEvent += CheckRoomClear;
            }
        }
        _colliderScript.OnWallCollision += DisableTriggers;
        //foreach (Collider door in _doors)
        //{
        //    door.gameObject.SetActive(false);
        //}
    }

    private void CheckRoomClear(Enemy enemy)
    {
        _enemies.Remove(enemy);

        if (_enemies.Count == 0)
        {
            Debug.Log("Room Cleared!");
            OpenDoors();
            return;
        }
        Debug.Log("Enemy defeated!");
    }

    private void OpenDoors()
    {
        for (int i = 0; i < _doors.Count; i++)
        {
            _doors[i].gameObject.SetActive(false);
            _roomDetector[i].gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered a room");
            DisableTriggers();
            //Call method and destroy all triggers of this room to avoid "re-entering".
        }
    }

    private void DisableTriggers()
    {
        Debug.Log("Player entered a room");
        foreach (Collider trigger in _roomDetector)
        {
            trigger.gameObject.SetActive(false);
        }
    }
}
