using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_script : MonoBehaviour
{
    public int damage;
	public bool isMelee, Rock;
	void Start()
	{

		if (!isMelee)
			Destroy(gameObject, 5);
	}
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Cant_Be_Passed_Wall") && !isMelee && !Rock)
		{
			Destroy(gameObject, 2);
		}
		else if (!Rock && !isMelee && (collision.gameObject.CompareTag("TreeBody") || collision.gameObject.CompareTag("WorldWall") 
			|| collision.gameObject.CompareTag("Close_Weapon") || collision.gameObject.CompareTag("Crate") || collision.gameObject.layer == 13) && collision.gameObject.layer != 4)
		{
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		
		if (!Rock && !isMelee && (other.gameObject.CompareTag("TreeBody") || other.gameObject.CompareTag("WorldWall") || other.gameObject.CompareTag("Cant_Be_Passed_Wall") ))
		{
			Destroy(gameObject);
		}
	}

}
