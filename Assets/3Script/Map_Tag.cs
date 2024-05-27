using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STANDARD
{
    STD_SPEED = 10,
    STD_INTERVAL = 84,
    STD_W_H = 14,
    North = 1,
    East = 2,
    South = 3,
    West = 0,
};

public class Map_Tag : MonoBehaviour
{
    

    public struct Dungeon_Info
    {
        public int Stage;
        public bool Can_Start;
        public bool[,] Hole;
        public bool[] Is_Open;
        public bool CanDuplicate;
        public bool Exist_Hole;
    };
}
