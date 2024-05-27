using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class This_Map_T : MonoBehaviour
{
    public Map_Tag.Dungeon_Info M_T;
    public enum Exist_Hole_Direction { None, North, East, South, West, };

    public int Stage;
    public bool Can_Start;
    public int[] Hole0 = new int[2];
    public int[] Hole1 = new int[2];
    public int[] Hole2 = new int[2];
    public int[] Hole3 = new int[2];
    public bool[] Is_Open = new bool[4];
    public bool CanDuplicate;
    public bool Exist_Hole;
    public Exist_Hole_Direction Where;
    public int[] Hole_WM = new int [2];
    public Transform[] Mob_spawn;
    public Transform[] Trap_spawn;
    public Transform[] Item_spawn;
    public Transform[] Gold_spawn;

}
