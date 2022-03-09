using UnityEngine;

namespace Generator3D
{

    [CreateAssetMenu(fileName = "GenerationSettings3D", menuName = "Settings Block/Generation Settings 3D")]
    public class GenerationSettings3D : ScriptableObject
    {
        //Size of field to spawn rooms
        public Vector3Int size;

        //count of rooms 
        public int roomCount;

        //number of extra tries to place the room
        public int extraTries;

        //minimal size of rooms will be from x to y
        public Vector3Int roomMinSize;

        //maximal size of rooms will be from x to y
        public Vector3Int roomMaxSize;

        //chance of generating extra edge between rooms
        public float cycleChance;

        [Header("Pathfinding")]
        public float stairsCost;
        public float emptyCellCost;
        public float roomCost;
        public float pathCost;
        
        //seed for generation
        [Header("Seed")]
        public string seed;
    }
}