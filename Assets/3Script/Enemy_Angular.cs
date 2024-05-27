using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Angular : MonoBehaviour
{
	Rigidbody rigid;
	void Awake()
	{
		rigid = gameObject.GetComponent<Rigidbody>();
	}
	void Update()
    {
        rigid.angularVelocity = Vector3.zero;

    }
}
