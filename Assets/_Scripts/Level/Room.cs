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
    //When setting doors to true, fire "Open" trigger.
    //When setting doors to false, fire "Close" trigger.

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
            door.transform.GetChild(0).GetComponent<DoorAnimationEvent>().OnAnimationEndEvent += OnAnimationEnd;
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
    }

    private void OpenDoors()
    {
        foreach (Collider door in _doors)
        {
            door.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Open");
        }
    }

    private void OnAnimationEnd()
    {
        Destroy(gameObject);
    }

    private void DisableTriggers(Collider other)
    {
        if (!_roomDetector.Contains(other)) return;
        if (_hasEntered) return;
        _hasEntered = true;
        OnRoomEnter?.Invoke(this);
        foreach (Collider trigger in _roomDetector)
        {
            Destroy(trigger.gameObject);
        }
        foreach (Collider door in _doors)
        {
            door.gameObject.SetActive(true);
            door.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Close");
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
