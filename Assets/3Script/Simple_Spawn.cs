using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple_Spawn : MonoBehaviour
{
    public GameObject[] Enemy;
    public int value;
    private int a;

    void Awake()
    {
        Invoke("Spawn", 2);
    }

    void Spawn()
    {
        GameObject dummy = Instantiate(Enemy[value]) as GameObject;
        dummy.transform.position = transform.position;
    }

}
