using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss_misile : Bullet_script
{
	public Transform target;
	NavMeshAgent nav;

	private void Awake()
	{
		nav = GetComponent<NavMeshAgent>();
	}
	private void Update()
	{
		nav.SetDestination(target.position);
		
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

		if (!Rock && !isMelee && (other.gameObject.CompareTag("TreeBody") || other.gameObject.CompareTag("WorldWall") || other.gameObject.CompareTag("Cant_Be_Passed_Wall")))
		{
			Destroy(gameObject);
		}
	}
}        
