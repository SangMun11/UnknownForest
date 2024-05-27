using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_script : MonoBehaviour
{
    public GameObject[] effect;
    public GameObject[] Item;
    public GameObject[] Weapon_item;
    public GameObject Target_dummy;
    public GameObject Dungeon_Creator;
    public GameObject[] Enemies;
    public GameObject[] Traps;
    public GameObject[] ItemCrate;
    public GameObject[] Gold;
    public GameObject[] Debris;
    public GameObject TreeBody;

	public void Awake()
	{
	}

	private void Update()
	{
        
    }
	public void Select_effect(Vector3 vec, int effect_num)
    {
        switch (effect_num)
        {
            case 1:
                StartCoroutine(effect_instatiate(vec,effect[0]));
                break;
            case 2:
                StartCoroutine(effect_instatiate(vec, effect[1]));
                break;
            case 3:
                StartCoroutine(effect_instatiate(vec, effect[2]));
                break;
            case 4:
                StartCoroutine(effect_instatiate(vec, effect[3]));
                break;
        }
    }

    IEnumerator effect_instatiate(Vector3 vec, GameObject e)
    {
        GameObject effect = Instantiate(e);
        effect.transform.position = vec;
        yield return new WaitForSeconds(1f);
        Destroy(effect);
    }


    public int Enemy_Select()
    {
        int random_select = Random.Range(0,5);

        switch (random_select)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                return Random.Range(0, 4);
            case 4:
                return 4 + Random.Range(0,3);
        }
        return 3;
    }

    public int Gold_Select()
    {
        int random_select = Random.Range(0,16);
        switch (random_select)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                return -1;

            case 8:
            case 9:
                return 0;
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
                return Gold_Sub_Select(0);
            case 15:
                return 4;
        }
        return 0;
    }
    private int Gold_Sub_Select(int size)
    {
        int Select_num = Random.Range(0, 11);
        switch (Select_num)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
                return size;
            case 5:
            case 6:
            case 7:
            case 8:
                return size + 1;
            case 9:
            case 10:
                return size + 2;
        }
        return 0;
    }
}
