using UnityEngine;
using System.Collections.Generic;

namespace Generator3D
{
    public class Stairs
    {
        public Vector3Int Direction;
        public Vector3Int Start;
        public int Height;
        
        public Stairs(Vector3Int from, Vector3Int to)
        {
            if (from.y > to.y)
            {
                (from, to) = (to, from);
            }
            
            Direction = to - from;
            Height = Direction.y;
            Direction.y = 0;
            Direction.Clamp(-Vector3Int.one, Vector3Int.one);

            Start = from;
        }

        public List<Vector3Int> GetAllPositions()
        {
            List<Vector3Int> res = new List<Vector3Int>();

            Vector3Int pos = Start;
            if(Height != 1)Debug.Log(Height);
            Vector3Int endPos = new Vector3Int(
                (Start.x + Direction.x*Height),
                (Start.y + Height   ),
                (Start.z + Direction.z*Height));
            
            Vector3Int up = Vector3Int.up;
            
            while (pos.y < endPos.y)
            {                                                                    
                res.Add(pos + Direction);
                res.Add(pos + Direction + up);
                res.Add(pos + Direction * 2);
                res.Add(pos + Direction * 2 + up);
                                                                                 
                pos += 2 * Direction + 2 * up;
            }
            
            return res;
        }
        
        public static bool operator==(Stairs a, Stairs b)
        {
            return a.Direction == b.Direction &&
                   a.Start == b.Start &&
                   a.Height == b.Height;
        }
        
        public static bool operator!=(Stairs a, Stairs b)
        {
            return !(a == b);
        }
        
    }
}