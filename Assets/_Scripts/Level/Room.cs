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

    private bool _hasEntered = false;

    public Action OnRoomCleared;
    public Action<Room> OnRoomEnter;

    private void Start()
    {
        foreach (Enemy enemy in _enemies)
        {
            if (enemy != null)
            {
                enemy.OnDeathEvent += CheckRoomClear;
            }
        }
        foreach (Collider door in _doors)
        {
            door.gameObject.SetActive(false);
        }
        _colliderScript.OnWallCollision += DisableTriggers;
    }

    private void CheckRoomClear(Enemy enemy)
    {
        _enemies.Remove(enemy);

        if (_enemies.Count == 0)
        {
            Debug.Log("Room Cleared!");
            OnRoomCleared?.Invoke();
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
        }
        gameObject.SetActive(false);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        Debug.Log("Entered in same script.");
    //        DisableTriggers();
    //        //Call method and destroy all triggers of this room to avoid "re-entering".
    //    }
    //}

    private void DisableTriggers(Collider other)
    {
        if (!_roomDetector.Contains(other)) return;
        if (_hasEntered) return;
        _hasEntered = true;
        OnRoomEnter?.Invoke(this);
        foreach (Collider trigger in _roomDetector)
        {
            trigger.gameObject.SetActive(false);
            //Destroy(trigger.gameObject)?
        }
        foreach (Collider door in _doors)
        {
            door.gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        _colliderScript.OnWallCollision -= DisableTriggers;
    }

    private void OnDisable()
    {
        _colliderScript.OnWallCollision -= DisableTriggers;
    }
}
