using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generator2D
{

    [CreateAssetMenu(fileName = "GenerationSettings2D", menuName = "Settings Block/Generation Settings 2D")]
    public class GenerationSettings2D : ScriptableObject
    {
        //Size of field to spawn rooms
        public Vector2Int size;

        //count of rooms 
        public int roomCount;

        //number of extra tries to place the room
        public int extraTries;

        //minimal size of rooms will be from x to y
        public Vector2Int roomMinSize;

        //maximal size of rooms will be from x to y
        public Vector2Int roomMaxSize;

        //chance of generating extra edge between rooms
        public float cycleChance;

        //seed for generation
        public string seed;
        


        public void SetFields(GenerationSettings2D settings)
        {
            size = settings.size;
            roomCount = settings.roomCount;
            extraTries = settings.extraTries;
            roomMinSize = settings.roomMinSize;
            roomMaxSize = settings.roomMaxSize;
            cycleChance = settings.cycleChance;
        }
        
    }
}
