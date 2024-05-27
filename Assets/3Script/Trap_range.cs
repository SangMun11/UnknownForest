using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_range : MonoBehaviour
{
    public bool dotDam, isNuck;
    public int Damage;
	public GameObject Effects;
	protected Grab_Speed grab = null;
	protected int durability;
	protected bool sounds;

	void Awake()
	{
		if (gameObject.layer == 13)
		{
			grab = transform.Find("GrabRange").GetComponent<Grab_Speed>();
			durability = 3;
		}
	}

	void Update()
	{
		if (grab != null)
		{
			if (grab.velocity > 30f && grab.CompareTag("Can_Grab"))
			{
				gameObject.layer = 18;
			}
			else
				gameObject.layer = 9;

		}
		else if (!sounds && !dotDam)
		{
			sounds = true;
			StartCoroutine(Effectmusic());
		}
	}
	IEnumerator Effectmusic()
	{
		yield return new WaitForSeconds(0.6f);
		Effects.GetComponent<AudioSource>().Play();
		sounds = false;
	}


	private void OnCollisionEnter(Collision collision)
	{
		if (grab != null)
		{
			if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
			{
				if (gameObject.layer == 13)
				{
					durability--;
					if (durability <= 0)
						Destroy(gameObject);
				}
			}
			else if (collision.gameObject.CompareTag("Close_Weapon"))
				gameObject.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}
}
