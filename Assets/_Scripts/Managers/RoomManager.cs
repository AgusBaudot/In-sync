using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private List<Room> _rooms = new List<Room>();
    [SerializeField] private Canvas _canvas;
    private int _counter = 0;
    private int _currentRoom = -1;

    private void Start()
    {
        foreach (Room room in _rooms)
        {
            room.OnRoomCleared += RoomCleared;
            room.OnRoomEnter += EnteredRoom;
        }
    }

    private void RoomCleared()
    {
        _counter++;
        _currentRoom = -1;
        if (_counter == _rooms.Count)
        {
            Cursor.visible = true;
            _canvas.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void EnteredRoom(Room room)
    {
        _currentRoom = _rooms.IndexOf(room) + 1;
    }

    public int GetActiveRoom() => _currentRoom;
}
