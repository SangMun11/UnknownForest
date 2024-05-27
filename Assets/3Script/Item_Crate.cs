using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Crate : MonoBehaviour
{
    private Effect_script efsc;
	private bool isopen;
	private MeshRenderer m;
	private BoxCollider b;
	private GameObject debris, DunCrea;

	void Start()
	{
		efsc = GameObject.Find("Object_Manager").GetComponent<Effect_script>();
		m = gameObject.GetComponent<MeshRenderer>();
		b = gameObject.GetComponent<BoxCollider>();
		DunCrea = GameObject.FindWithTag("DungeonCreator");
		debris = efsc.Debris[1];
	}
	void OnTriggerEnter(Collider other)
	{
		if ((other.gameObject.CompareTag("Close_Weapon") || other.gameObject.CompareTag("Bullet") || other.gameObject.CompareTag("Can_Grab") || other.gameObject.CompareTag("Trap_dam")) && !isopen)
		{
			if (other.gameObject.CompareTag("Can_Grab"))
			{
				if (other.gameObject.GetComponent<Grab_Speed>().velocity < 50)
					return;
			}

			isopen = true;
			m.enabled = false;
			b.enabled = false;
			int SelectWI = Random.Range(0, 11);
			switch (SelectWI)
			{
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:

					StartCoroutine(SelectItem());
					break;
				case 9:
				case 10:
					SelectWeapon();
					break;
			}
			Breaking();
		}
	}

	IEnumerator SelectGold()
	{
		int SelectItem = Random.Range(0, 11);
		GameObject dummy = null;
		switch (SelectItem)
		{
			//ÇÏÆ®
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:

				dummy = GameObject.Instantiate(efsc.Gold[4]) as GameObject;
				dummy.tag = "Untagged";
				dummy.transform.position = transform.position + Vector3.up * 3;
				yield return new WaitForSeconds(0.3f);
				dummy.tag = "Item";
				break;
			case 5:
			case 6:
			case 7:
			case 8:
				dummy = GameObject.Instantiate(efsc.Gold[5]) as GameObject;
				dummy.tag = "Untagged";
				dummy.transform.position = transform.position + Vector3.up * 3;
				yield return new WaitForSeconds(0.3f);
				dummy.tag = "Item";
				break;
			case 9:
			case 10:
				dummy = GameObject.Instantiate(efsc.Gold[6]) as GameObject;
				dummy.tag = "Untagged";
				dummy.transform.position = transform.position + Vector3.up * 3;
				yield return new WaitForSeconds(0.3f);
				dummy.tag = "Item";
				break;
		}
		if (DunCrea != null)
			dummy.transform.parent = DunCrea.transform;
		else { dummy.transform.parent = GameObject.Find("StartMap_Package").transform; }
		Destroy(gameObject);
	}

	IEnumerator SelectItem()
	{
		int SelectItem = Random.Range(0, 11);
		GameObject dummy = null;
		switch (SelectItem)
		{
			//ÇÏÆ®
			case 0:
			case 1:
			case 4:
				dummy = GameObject.Instantiate(efsc.Item[0]) as GameObject;
				dummy.tag = "Untagged";
				dummy.transform.position = transform.position + Vector3.up * 3;
				yield return new WaitForSeconds(0.3f);
				dummy.tag = "Item";
				break;
			//ÃÑ¾Ë
			case 2:
			case 3:
				dummy = GameObject.Instantiate(efsc.Item[1]) as GameObject;
				dummy.tag = "Untagged";
				dummy.transform.position = transform.position + Vector3.up * 3;
				yield return new WaitForSeconds(0.3f);
				dummy.tag = "Item";
				break;
			//ÆøÅº
			case 5:
				dummy = GameObject.Instantiate(efsc.Item[2]) as GameObject;
				dummy.tag = "Untagged";
				dummy.transform.position = transform.position + Vector3.up * 3;
				yield return new WaitForSeconds(0.3f);
				dummy.tag = "Item";
				break;
				//Àâ°Í

			case 6:
			case 7:
				dummy = GameObject.Instantiate(efsc.Item[4]) as GameObject;
				dummy.transform.position = transform.position + Vector3.up * 3;
				break;
			case 8:
			case 9:
				dummy = GameObject.Instantiate(efsc.Item[6]) as GameObject;
				dummy.transform.position = transform.position + Vector3.up * 3;
				break;
			case 10:

				dummy = GameObject.Instantiate(efsc.Item[5]) as GameObject;
				dummy.tag = "Untagged";
				dummy.transform.position = transform.position + Vector3.up * 3;
				yield return new WaitForSeconds(0.3f);
				dummy.tag = "Item";
				break;

		}

		if (DunCrea != null)
			dummy.transform.parent = DunCrea.transform;
		else { dummy.transform.parent = GameObject.Find("StartMap_Package").transform; }
		Destroy(gameObject);
	}
	void SelectWeapon()
	{
		int SelectWeapon = Random.Range(0, 11);
		GameObject dummy = null;
		switch (SelectWeapon)
		{
			case 0:
			case 1:
			case 2:
				dummy = GameObject.Instantiate(efsc.Weapon_item[0]) as GameObject;
				dummy.transform.position = transform.position + Vector3.up * 3;
				break;
			case 3:
			case 4:
			case 5:
			case 6:
				dummy = GameObject.Instantiate(efsc.Weapon_item[1]) as GameObject;
				dummy.transform.position = transform.position + Vector3.up * 3;
				break;
			case 7:
			case 8:
			case 9:
				dummy = GameObject.Instantiate(efsc.Weapon_item[2]) as GameObject;
				dummy.transform.position = transform.position + Vector3.up * 4;
				break;
			case 10:
				dummy = GameObject.Instantiate(efsc.Weapon_item[3]) as GameObject;
				dummy.transform.position = transform.position + Vector3.up * 5;
				break;
		}
		if (DunCrea != null)
			dummy.transform.parent = DunCrea.transform;
		else { dummy.transform.parent = GameObject.Find("StartMap_Package").transform; }

		Destroy(gameObject);
	}

	void Breaking()
	{
		GameObject dummy = Instantiate(debris, transform.position, Quaternion.identity);
		Rigidbody[] rigids = dummy.GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < rigids.Length; i++)
		{
			rigids[i].AddExplosionForce(1000, transform.position + new Vector3(0,-2,0), 10f);
		}
		//gameObject.SetActive(false);
	}
}
