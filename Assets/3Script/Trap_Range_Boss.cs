using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Range_Boss : Trap_range
{
	float timer;
	[SerializeField]
	private Trap_Boss TB;
	private Vector3 shotpoint;

	void Awake()
	{
		timer = 0;
		shotpoint = transform.localPosition;
	}

	// Update is called once per frame
	void Update()
    {
		if (TB.isOut)
		{
			timer += Time.deltaTime;
			if (timer > 2f)
			{
				timer = 0;
				TB.isOut = false;
				transform.localPosition = shotpoint;
				//transform.rotation = Quaternion.Euler(0,0,-90);
				transform.localRotation = Quaternion.Euler(0,0,-90);

				GetComponent<Rigidbody>().isKinematic = true;
				GetComponent<Rigidbody>().useGravity = false;
				GetComponent<MeshRenderer>().enabled = true;
				GetComponent<CapsuleCollider>().enabled = true;
				GetComponent<SphereCollider>().enabled = true;
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

			}
		}
    }

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.gameObject.CompareTag("Player") || collision.transform.gameObject.CompareTag("Cant_Be_Passed_Wall"))
		{
			GetComponent<MeshRenderer>().enabled = false;
			GetComponent<CapsuleCollider>().enabled = false;
			GetComponent<SphereCollider>().enabled = false;
			//GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		}
	}
}
