using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss_script : Enemy_script
{
	private int AttackType;
	public GameObject[] missile;
	public Transform[] Port;
	public GameObject BossWall, BossBomb;
	Vector3 lookVec;
	Vector3 tauntVec;
	bool isLook;
	private GameObject curdun;

	protected void Awake()
	{
		//parent_o = transform.parent.gameObject;
		efsc = GameObject.Find("Object_Manager").GetComponent<Effect_script>();
		parent_o = transform.gameObject;
		state = parent_o.GetComponent<EnemyState>();
		type = state.type;
		maxHealth = state.maxHealth; curHealth = state.curHealth;

		rigid = parent_o.GetComponent<Rigidbody>();
		//boxcollider = parent_o.GetComponent<BoxCollider>();
		meshs = parent_o.GetComponentsInChildren<MeshRenderer>();
		if (type == Type.M)
			smesh = parent_o.GetComponentInChildren<SkinnedMeshRenderer>();

		nav = parent_o.GetComponent<NavMeshAgent>();
		anim = parent_o.GetComponentInChildren<Animator>();

		isLook = true;
		Burn = Burn_Off();
		nav.enabled = false;
		StartCoroutine(Think());
		isStart = true;
		curdun = GameObject.FindWithTag("DungeonCreator");
		StartCoroutine(Ready_To_Start());
	}

	IEnumerator Ready_To_Start()
	{
		yield return new WaitForSeconds(3f);
		if (nav.isOnNavMesh)
			nav.enabled = true;
		isStart = true;
	}


	void Chasing_Range()
	{
		RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 70, Vector3.up, 0f, LayerMask.GetMask("Player"));

		//범위내에 있다면
		if ((rayHits.Length != 0) && !isAttack)
		{
			if (!TargetSearch)
			{ 
			efsc.Select_effect(transform.position + (Vector3.up * 5), 2); }
			//Debug.Log("범위 안");
			Target = rayHits[0].transform;

			nav.enabled = true;
			TargetSearch = true; isChase = true;
		}
	}

	
	void Update()
	{
		if (isStart && !isDie)
		{

			//불뎀과 죽음
			Burn_dam();
			if (curHealth <= 0)
			{ StopAllCoroutines(); StartCoroutine(Dead()); }
			else
			{
				//추격
				Chasing_Range();

				//맞으면 색깔 깜빡
				if (isDam && !isDamS)
				{
					isDamS = true;
					IEnumerator Damage_ie = Damage_color();
					StartCoroutine(Damage_ie);
				}
			}



			if (isLook && Target != null && !isDie)
			{
				float h = Input.GetAxis("Horizontal");
				float v = Input.GetAxis("Vertical");
				lookVec = new Vector3(h, 0, v) * 5f;
				transform.LookAt(Target.position + lookVec);
			}
			else if (!isDie && isStart && Target != null)
			{
				nav.SetDestination(tauntVec);
			}
		}
	}

	void FixedUpdate()
	{
		if (isStart)
		{
			if (!isDie && !Stun)
			{
				//Targeting();
				FreezeVelocity();
			}
		}
	}

	IEnumerator Think()
	{
		yield return new WaitForSeconds(0.1f);
		AttackType = Random.Range(0, 11);

		//몬스터 타입에 따라 탐색범위
		switch (AttackType)
		{
			case 0:
			case 1:
			case 2:
				StartCoroutine(MissileShot());
				break;
			case 3:
			case 4:
				//미
				StartCoroutine(GuidedMissileShot());
				break;
			case 5:
			case 6:
				//돌
				StartCoroutine(RockShot());
				break;
			case 7:
			case 8:
				//점
				StartCoroutine(Taunt());
				break;
			case 9:
				StartCoroutine(Summon_survo());
				break;
			case 10:
				
				StartCoroutine(Aerial_Bombing());
				break;
		}
	}

	IEnumerator MissileShot()
	{
		if (Target != null)
		{
			FreezeVelocity();
			anim.SetTrigger("Shot");
			yield return new WaitForSeconds(0.2f);
			GameObject dummy = Instantiate(missile[1], Port[0].position - Vector3.up*4, Port[0].rotation);
			dummy.GetComponent<Rigidbody>().AddForce(transform.forward * 50, ForceMode.Impulse);

			FreezeVelocity();

			yield return new WaitForSeconds(0.3f);
			dummy = Instantiate(missile[1], Port[1].position - Vector3.up * 4, Port[1].rotation);
			dummy.GetComponent<Rigidbody>().AddForce(transform.forward * 50, ForceMode.Impulse);

			yield return new WaitForSeconds(1f);
		}
		if (Random.Range(0, 10) > 7)
			StartCoroutine(GuidedMissileShot());
		else
			StartCoroutine(Think());
	}

	IEnumerator GuidedMissileShot()
	{
		if (Target != null)
		{
			FreezeVelocity();
			anim.SetTrigger("Shot");
			yield return new WaitForSeconds(0.2f);
			GameObject dummy = Instantiate(missile[0], Port[0].position, Port[0].rotation);

			Boss_misile bossdummy = dummy.GetComponent<Boss_misile>();
			bossdummy.target = Target;

			FreezeVelocity();

			yield return new WaitForSeconds(0.3f);
			dummy = Instantiate(missile[0], Port[1].position, Port[1].rotation);
			bossdummy = dummy.GetComponent<Boss_misile>();
			bossdummy.target = Target;
			
			yield return new WaitForSeconds(2f);
		}
		StartCoroutine(Think());
	}

	IEnumerator Aerial_Bombing()
	{
		if(Target != null)
		{


			FreezeVelocity();
			GameObject dummy;
			Vector3 dummy_vec;
			for (int i = 0; i < Random.Range(3, 5); i++)
			{
				FreezeVelocity();

				yield return new WaitForSeconds(1f);
				FreezeVelocity();

				state.Sound[2].GetComponent<AudioSource>().Play();
				for (int j = 0; j < 5; j++)
				{
					dummy_vec = transform.position + new Vector3(Random_absover(70, 20), 50, Random_absover(80, 20));
					dummy = Instantiate(missile[2], dummy_vec, Port[0].rotation);
				}
				anim.SetTrigger("Shot");
			}
			

			yield return new WaitForSeconds(3f);
		}
		StartCoroutine(Think());
	}

	IEnumerator RockShot()
	{
		if (Target != null)
		{
			isLook = false;
			anim.SetTrigger("BigShot");
			FreezeVelocity();
			GameObject dummy = Instantiate(bullet) as GameObject;
			dummy.transform.localPosition = transform.position;
			dummy.transform.localRotation = transform.rotation;
			yield return new WaitForSeconds(1f);
			state.Sound[4].GetComponent<AudioSource>().Play();


			yield return new WaitForSeconds(2f);
			isLook = true;
		}
		StartCoroutine(Think());
	}
	IEnumerator Taunt()
	{
		if (Target != null)
		{
			FreezeVelocity();
			tauntVec = Target.position + lookVec;
			nav.isStopped = false;
			isLook = false;
			gameObject.GetComponent<CapsuleCollider>().enabled = false;
			anim.SetTrigger("Taunt");

			yield return new WaitForSeconds(1.5f);
			attackArea.enabled = true;
			nav.isStopped = true;
			state.Sound[3].GetComponent<AudioSource>().Play();

			yield return new WaitForSeconds(0.5f);
			attackArea.enabled = false;
			gameObject.GetComponent<CapsuleCollider>().enabled = true;

			yield return new WaitForSeconds(1f);
			isLook = true;

		}
		StartCoroutine(Think());
	}

	IEnumerator Summon_survo()
	{
		curdun = GameObject.FindWithTag("DungeonCreator");

		if (Target != null)
		{
			int num, times = Random.Range(3, 7);
			GameObject dummy;
			FreezeVelocity();
			for (int i = 0; i < times; i++)
			{
				yield return new WaitForSeconds(1f);
				anim.SetTrigger("Shot");

				num = Random.Range(0, 3);
				state.Sound[5 + num].GetComponent<AudioSource>().Play();
				dummy = Instantiate(efsc.GetComponent<Effect_script>().Enemies[num]) as GameObject;
				dummy.GetComponent<Enemy_script>().curHealth *= 3;
				dummy.transform.localPosition = transform.position + new Vector3(Random_absover(20, 10), 2, Random_absover(20, 10));
				dummy.GetComponent<Enemy_script>().Target = Target;
				dummy.transform.parent = curdun.transform;
			}
			yield return new WaitForSeconds(4f);
		}
		StartCoroutine(Think());
	}



	



	private IEnumerator Ondamage(Vector3 reactVec, bool isg)
	{
		Invincibility = true;
		isChase = false; Stun = true;
		state.Sound[0].GetComponent<AudioSource>().Play();
		//if (!isAttack)
		isDam = true;

		

		yield return new WaitForSeconds(0.2f);

		if (curHealth > 0)
		{
			yield return new WaitForSeconds(1f);

			Invincibility_reset();

			yield return new WaitForSeconds(1f);

			
			Stun = false;



		}
	}

	private IEnumerator Dead()
	{
		if (curHealth <= 0)
		{
			isDie = true;
			rigid.velocity = Vector3.zero;
			RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 100, Vector3.up, 0f, LayerMask.GetMask("Player"));
			if (rayHits.Length != 0)
			{
				GameObject.FindWithTag("Player").GetComponent<player_script>().score += state.Score;
			}

			if (nav.isOnNavMesh)
			{
				nav.enabled = true; nav.isStopped = true;
			}
			isDam = false;
			gameObject.layer = 12;
			nav.enabled = false;

			anim.SetTrigger("Die");
			state.Sound[1].GetComponent<AudioSource>().Play();
			rigid.velocity = Vector3.zero;
			rigid.angularVelocity = Vector3.zero;


			yield return new WaitForSeconds(0.7f);
			if (type != Type.M)
			{ foreach (MeshRenderer mesh in meshs) { mesh.material.color = Color.gray; } }
			else
				smesh.material.color = Color.gray;
			BossBomb.SetActive(true);
			BossWall.SetActive(false);


			gameObject.GetComponent<Enemy_script>().enabled = false;

			//Destroy(gameObject, 3);
		}
	}


	public new void HitByGrenade(Vector3 explosionpos)
	{
		if (!Invincibility)
		{
			Invincibility = true;
			curHealth -= 7;
			Vector3 reactVec = transform.position - explosionpos;
			reactVec.y = 0.5f;
			StartCoroutine(Ondamage(reactVec.normalized, true));
		}
	}

}
