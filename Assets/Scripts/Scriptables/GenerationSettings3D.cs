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

        //seed for generation
        public string seed;



        public void SetFields(GenerationSettings3D settings)
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