using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Script : MonoBehaviour
{
    public enum Type
    {
        Default_W, Ammo, Boom, heart, ReinforceB, Coin,
    }

    public Type Type_I;
    public int value;
    public int Durability;
    private Rigidbody rigid;

	void Awake()
	{
        rigid = GetComponent<Rigidbody>();
	}

	void OnCollisionEnter(Collision collision)
	{
        if (collision.gameObject.layer == 3)
        {
            rigid.isKinematic = true;
            GetComponent<SphereCollider>().enabled = false;
        }
	}
}
