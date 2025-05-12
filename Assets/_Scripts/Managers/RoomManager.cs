using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private Collider _player;
    //[SerializeField] private List<Collider> _rooms = new List<Collider>(); //Trigger colliders for detection with _player.
    //[SerializeField] private List<Collider> _doors = new List<Collider>(); //Non-trigger collider for closing _player's path.
    [SerializeField] private List<Room> _rooms = new List<Room>();
    private string _activeRoom = "";
    //List with enemies represents one room. Somehow connect enemy list with room list. (index).
    //When _player enters room:
    //1. Detect if room is cleared (enemy list is empty).
    //2. Door closes. (already done).
    //3. Update current room (check if already done).
    //4. Check if all enemies are defeated. Open doors again if they are.


    private void Start()
    {
        //_player.GetComponent<RoomCollider>().OnWallCollision += OnCollision;
        _activeRoom = "Room 1";
        //Foreach doors to set them all to false.
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    for (int i = 0; i < _rooms.Count; i++)
        //    {
        //        _rooms[i].gameObject.SetActive(true);
        //        _doors[i].gameObject.SetActive(false);
        //    }
        //}
    }

    public void OnCollision(Collider other)
    {
        var wall = other.transform.parent.parent;
        //if (wall.name != _activeRoom)
        //{
        //    ClearTriggers(other);
        //}
        _activeRoom = wall.name;
        Debug.Log($"Activated wall {wall.name}");
        //activate corresponding door based on wall collided. (maybe get index of room and setactive door of same index). Make lists for this to reduce time consumed and make it more optimized.
    }

    //private void ActivateRoom(Collider collided)
    //{
    //    _doors[_rooms.IndexOf(collided)].gameObject.SetActive(true); //Set true the door which index matches the collided wall.
    //    //_rooms[_rooms.IndexOf(collided)].gameObject.SetActive(false);
    //    collided.gameObject.SetActive(false);
    //}

    public string GetActiveRoom() => _activeRoom;
}
