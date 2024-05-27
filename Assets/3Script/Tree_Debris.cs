using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_Debris : MonoBehaviour
{
    GameObject Debris = null;
    [SerializeField]float power = 0f;
    [SerializeField]Vector3 offset;


	private void Awake()
	{
        Debris = GameObject.Find("Object_Manager").transform.GetComponent<Effect_script>().Debris[0];
	}

	private void Start()
	{
        Explosion();
	}
	public void Explosion()
    {
        GameObject dummy = Instantiate(Debris, transform.position, Quaternion.identity);
        Rigidbody[] rigids = dummy.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rigids.Length; i++)
        {
            rigids[i].AddExplosionForce(power, transform.position + offset, 10f);
        }
        gameObject.SetActive(false);
    }
}
