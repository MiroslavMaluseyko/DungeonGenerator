using System;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private Vector3 size;
    [SerializeField] private Vector3 pos;
    [SerializeField] public GameObject gameObjectRoomPrefab;
    private GameObject createdRoom;

    public Vector3 Size
    {
        get => size;
        set 
        { 
            size = Vector3. Max(Vector3.one, value);
            if (createdRoom != null)
            {
                createdRoom.transform.localScale = size;
            }
        }
    }
    public Vector3 Pos 
    {
        get => pos;
        set
        {
            pos = value;
            transform.position = pos;
        }
    }

    private void Start()
    {
        createdRoom = Instantiate(gameObjectRoomPrefab, transform);
        Size = size;
        Pos = pos;
    }
}