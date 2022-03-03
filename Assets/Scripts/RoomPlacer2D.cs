using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RoomPlacer2D
{

    private List<Room> rooms;
    private Vector3 lpos;
    private Vector3 rpos;


    public RoomPlacer2D(Vector3 leftBotPos, Vector3 rightUpPos, List<Room> rooms)
    {
        lpos = leftBotPos;
        rpos = rightUpPos;
        this.rooms = rooms;
    }

    public async void PlaceAllRooms()
    {
        foreach (var room in rooms)
        {
            await PlaceRoom(room);
        }
    }

    public async Task PlaceRoom(Room room)
    {
        await Task.Delay(100);

        room.Pos = FindPlaceForSpawn(room);
    }


    private Vector3 FindPlaceForSpawn(Room room)
    {
        Vector3 spawnPos;
        
        
        int count = 0;
        do
        {
            count++;
            float x = Mathf.RoundToInt(Random.Range(lpos.x, rpos.x));
            float y = Mathf.RoundToInt(Random.Range(lpos.y, rpos.y));
            float z = Mathf.RoundToInt(Random.Range(lpos.z, rpos.z));
            spawnPos = new Vector3(x, y, z);
            
        } while (Physics.CheckBox(spawnPos, room.Size/2, Quaternion.identity));

        Debug.Log($"I tried {count} times.");

        return spawnPos;
    }
}
