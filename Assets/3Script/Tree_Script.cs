using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_Script : MonoBehaviour
{
	GameObject Treebody;
	void Awake()
	{
		Treebody = Instantiate(GameObject.Find("Object_Manager").GetComponent<Effect_script>().TreeBody) as GameObject;
		Treebody.transform.localPosition = transform.position;
		
		Treebody.transform.parent = transform;
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.transform.GetComponent<Boss_Rock>() != null && other.transform.GetComponent<Boss_Rock>().speed >= 20)
		{
			Destroy(gameObject, 0.05f);
		}
	}
	


	/*
    private MeshRenderer this_mesh;
	private RaycastHit[] rayHits;
	private bool mesh_e = true, isSearch;
	private Weapon_script wea;
	private GameObject debris;
	private Effect_script efsc;

	private void Awake()
	{
		efsc = GameObject.Find("Object_Manager").GetComponent<Effect_script>();
		this_mesh = GetComponent<MeshRenderer>();
		debris = efsc.Debris[0];
	}

	void Update()
    {

		if (!isSearch)
		{
			isSearch = true;
			StartCoroutine(search());
		}

	}
	IEnumerator search()
	{
		yield return new WaitForSeconds(2f);
		rayHits = Physics.SphereCastAll(transform.position, 150, Vector3.up, 0f, LayerMask.GetMask("Player"));
		if (rayHits.Length != 0 && !mesh_e)
		{
			mesh_e = true;
			this_mesh.enabled = true;
			yield return new WaitForSeconds(5f);
		}
		else if (rayHits.Length == 0 && mesh_e)
		{
			mesh_e = false;
			this_mesh.enabled = false;
		}
		isSearch = false;
	}


	/*
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Close_Weapon"))
		{
			wea = other.GetComponent<Weapon_script>();
			if (wea.can_logging && !wea.islog)
			{
				wea.islog = true;
				wea.Ammo--;
				Destroy(gameObject);
				wea.StartCoroutine(wea.logout());
			}
		}
	}*/
}
