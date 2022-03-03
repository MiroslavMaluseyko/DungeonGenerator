using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Generator : MonoBehaviour
{
    [SerializeField] private int roomCountMin;
    [SerializeField] private int roomCountMax;
    [SerializeField] private int minRoomWidthX;
    [SerializeField] private int maxRoomWidthX;
    [SerializeField] private int minRoomWidthZ;
    [SerializeField] private int maxRoomWidthZ;
    [SerializeField] private Transform leftBotCorner;
    [SerializeField] private Transform rightUpCorner;

    [SerializeField] private Room[] roomPrefabs;

    private int roomsCount;
    private List<Room> createdRooms;
    private RoomPlacer2D roomPlacer2D;

    private void Start()
    {
        roomsCount = Random.Range(roomCountMin, roomCountMax+1);
        GenerateDungeon();
    }


    private void GenerateDungeon()
    {
        createdRooms = GenerateRooms();
        roomPlacer2D = new RoomPlacer2D(leftBotCorner.position, rightUpCorner.position, createdRooms);

        roomPlacer2D.PlaceAllRooms();
    }

    private List<Room> GenerateRooms()
    {
        List<Room> rooms = new List<Room>();
        for (int i = 0; i < roomsCount; i++)
        {
            rooms.Add(CreateRoom());
        }

        return rooms;
    }
    
    private Room CreateRoom()
    {
        Vector3 roomSize = new Vector3(Random.Range(minRoomWidthX, maxRoomWidthX+1), 1, Random.Range(minRoomWidthZ, maxRoomWidthZ+1));

        
            
        Room room = Instantiate(roomPrefabs[0], Vector3.up*100, Quaternion.identity).GetComponent<Room>();
        room.Size = roomSize;
        return room;
    }
    
    
}
