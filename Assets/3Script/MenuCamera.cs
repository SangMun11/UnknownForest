using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    private GameManager gobj;

	void Awake()
	{
		gobj = GameObject.Find("GameManager").GetComponent<GameManager>();
	}

	void Update()
	{
		
	}


}
