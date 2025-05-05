using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private Collider player;
    [SerializeField] private List<Collider> rooms = new List<Collider>(); //Trigger colliders for detection with player.
    [SerializeField] private List<Collider> doors = new List<Collider>(); //Non-trigger collider for closing player's path.
    private string activeRoom = "";

    private void Start()
    {
        player.GetComponent<RoomCollider>().OnWallCollision += OnCollision;
        foreach (Collider door in doors)
        {
            door.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            foreach (Collider wall in doors)
            {
                wall.gameObject.SetActive(false);
            }
        }
    }

    public void OnCollision(Collider wall)
    {
        activeRoom = wall.name;
        ActivateRoom(wall);
        Debug.Log($"Activated wall {wall.name}");
        //activate corresponding door based on wall collided. (maybe get index of room and setactive door of same index). Make lists for this to reduce time consumed and make it more optimized.
    }

    private void ActivateRoom(Collider collided)
    {
        doors[rooms.IndexOf(collided)].gameObject.SetActive(true); //Set true the door which index matches the collided wall.
    }

    public string ActiveRoom() => activeRoom;
}
