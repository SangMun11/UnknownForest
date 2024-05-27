using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type { A, B, C, M, L, Chicken, M2, Boss};
public class EnemyState : MonoBehaviour
{
    [SerializeField]
    //private Type type;
    internal Type type;
    [SerializeField]
    internal int maxHealth, curHealth;
    public int Stage;
    public int Score;
    public GameObject[] Sound;
}
