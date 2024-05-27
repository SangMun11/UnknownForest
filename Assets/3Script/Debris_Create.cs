using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris_Create : MonoBehaviour
{
    private Effect_script efsc;
	private GameObject[] debris;
    void Start()
    {
        efsc = GameObject.Find("Object_Manager").GetComponent<Effect_script>();
		debris = efsc.Debris;
    }

	public void Breaking(Transform trans, float power, int debris_num)
	{
		GameObject dummy = Instantiate(debris[debris_num], trans.position, Quaternion.identity);
		Rigidbody[] rigids = dummy.GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < rigids.Length; i++)
		{
			//rigids[i].AddExplosionForce(1000, transform.position + new Vector3(0, -2, 0), 10f);
			rigids[i].AddExplosionForce(power, trans.position + new Vector3(0, -2, 0), 10f);
		}
		//gameObject.SetActive(false);
	}
}
