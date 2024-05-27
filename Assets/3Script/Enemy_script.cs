using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_script : MonoBehaviour
{
	//public enum Type { A, B, C};
	//public Type type;
	//public int maxHealth, curHealth;
	protected EnemyState state;
	protected Type type;
	[SerializeField]
	public int maxHealth, curHealth;
	protected Effect_script efsc;
	[SerializeField]
	protected GameObject dummy, respawn_way;
	protected float time_des_d, timer_air, dot_dam;

	public Transform Target;
	public bool isChase;
	public BoxCollider attackArea;
	public GameObject bullet;
	public GameObject CanGrab;
	
	[SerializeField]
	protected bool Invincibility = false, isAttack, isStart, isDie, Stun, TargetSearch, AngerTime, nonAnger,
		isstarted, isLanded, BurnOff, isDam, isDamS, freezenchicken, isEffect;
	public bool isBurn;
	protected IEnumerator Attack_coroutine, Burn;
	protected Rigidbody rigid;
	//BoxCollider boxcollider;

	protected MeshRenderer[] meshs;
	protected SkinnedMeshRenderer smesh;

	protected Animator anim;
	//네비게이션
	protected NavMeshAgent nav;
	protected GameObject parent_o;
	protected bool isRdown, isBorder;
	protected IEnumerator idle;

	private void Awake()
	{
		//parent_o = transform.parent.gameObject;
		efsc = GameObject.Find("Object_Manager").GetComponent<Effect_script>();
		parent_o = transform.gameObject;
		state = parent_o.GetComponent<EnemyState>();
		type = state.type;
		maxHealth = state.maxHealth; curHealth = state.curHealth;

		rigid = parent_o.GetComponent<Rigidbody>();
		//boxcollider = parent_o.GetComponent<BoxCollider>();
		Attack_coroutine = Attack();
		meshs = parent_o.GetComponentsInChildren<MeshRenderer>();
		if (type == Type.M)
			smesh = parent_o.GetComponentInChildren<SkinnedMeshRenderer>();

		nav = parent_o.GetComponent<NavMeshAgent>();
		anim = parent_o.GetComponentInChildren<Animator>();

		idle = Idle_state();
		Burn = Burn_Off();
		nav.enabled = false;
		StartCoroutine(Ready_To_Start());
		if (type != Type.Chicken && type != Type.Boss)
			rigid.mass = 10;
		//Invoke("ChaseStart", 2f);
		//Debug.Log(type + "  " +Vector3.Distance(Target.position, transform.position));
	}

	IEnumerator Ready_To_Start()
	{
		yield return new WaitForSeconds(3f);
		
			nav.enabled = true;
		isStart = true;
	}


	void Chasing_Range()
	{
		RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 30, Vector3.up, 0f, LayerMask.GetMask("Player"));
		if (TargetSearch)
			rayHits = Physics.SphereCastAll(transform.position, 50, Vector3.up, 0f, LayerMask.GetMask("Player"));
		//범위내에 있다면
		if ((rayHits.Length != 0) && !isAttack)
		{
			if (!TargetSearch)
			{ efsc.Select_effect(transform.position + (Vector3.up*5), 2); }
			//Debug.Log("범위 안");
			StopCoroutine(idle);
			Destroy(dummy);
			dummy = null; isstarted = false;
			Target = rayHits[0].transform;
			nonAnger = false;
			
				nav.enabled = true;
			TargetSearch = true;isChase = true;
			anim.SetBool("Walk", true);
		}
		//범위내에 없으며 데미지가 없으면
		else if (rayHits.Length == 0 && curHealth == maxHealth && Target != null || nonAnger)
		{
			if (dummy == null)
				nav.enabled = false;	
			TargetSearch = false;
			isChase = false;
			Target = null;
			if (type == Type.B)
				anim.SetBool("ReadyToAction",false);
			anim.SetBool("Walk", false);
			nonAnger = false;
		}
		//범위 내에 없으며 데미지를 달아 있다면. 분노
		else if (!AngerTime && curHealth != maxHealth && Target != null)
		{
			AngerTime = true;
			StartCoroutine(Timer_anger());
		}
	}
	IEnumerator Timer_anger()
	{
		yield return new WaitForSeconds(3f);
		RaycastHit[] rayHitss = Physics.SphereCastAll(transform.position, 75, Vector3.up, 0f, LayerMask.GetMask("Player"));
		if (rayHitss.Length == 0)
			nonAnger = true;
		AngerTime = false;
	}

	IEnumerator Idle_state()
	{				
		isstarted = true;
		if (dummy == null)
		{
			yield return new WaitForSeconds(2f);
			dummy = GameObject.Instantiate(efsc.Target_dummy) as GameObject;
			dummy.transform.parent = gameObject.transform;
			dummy.transform.position = transform.position + new Vector3(Random_absover(8f,3), 3, Random_absover(8f,3));
			//Target = dummy.transform;
			//Debug.Log(dummy.transform.position);
			anim.SetBool("Walk",true);
			if (nav.isOnNavMesh)
				nav.enabled = true;
			if (nav.enabled)
				nav.SetDestination(dummy.transform.position);
			time_des_d = 0;
		}
		else
		{
			bool isBorder = Physics.Raycast(transform.position + Vector3.up, transform.forward, 2, LayerMask.GetMask("Wall", "Map", "Water"));

			if (Vector3.Distance(dummy.transform.position, transform.position) < 1f || time_des_d > 3f || isBorder)
			{
				anim.SetBool("Walk", false);
				nav.enabled = false;
				yield return new WaitForSeconds(Random.Range(3.0f, 6.0f));
				Destroy(dummy);
				dummy = null;
				time_des_d = 0;
			}
		}
		isstarted = false;

		//dummy = Instantiate(efsc.Target_dummy, new Vector3(Random.Range(1.0f,3.0f),transform.position.y, Random.Range(1.0f, 3.0f)), Quaternion.identity);

		//Target = 
	}


	void Update()
	{
		if (isStart && !isDie)
		{
			if (nav.enabled && !isLanded)
			{ nav.enabled = false; }

			RaycastHit rayHits;
			isBorder = Physics.Raycast(transform.position + transform.up * 1, transform.forward, out rayHits, 2);

			Burn_dam();
			if (curHealth <= 0)
			{StopAllCoroutines(); StartCoroutine(Dead()); }
			else if (!(isBorder && rayHits.transform.gameObject.CompareTag("Shield")))
			{

				if (type != Type.Chicken)
					Chasing_Range();

				if (!TargetSearch && !isstarted && !freezenchicken)
				{
					if (dummy == null)
					{
						idle = Idle_state();
						StartCoroutine(idle);
					}
					//여기 문제. 더미가 사라지면 망가짐
					else if (dummy != null)
					{
						time_des_d += Time.deltaTime;
						if (!isstarted && !TargetSearch)
						{ idle = Idle_state(); StartCoroutine(idle); }
					}
				}
				if (isDam && !isDamS)
				{
					isDamS = true;
					IEnumerator Damage_ie = Damage_color();
					StartCoroutine(Damage_ie);
				}


				if (nav.enabled && TargetSearch && nav.isOnNavMesh)
				{
					nav.SetDestination(Target.position);
					nav.isStopped = !isChase;
				}
			}
		}



		/*
		isRdown = Input.GetButtonDown("Reload");
		if (isRdown)
		{
			StartCoroutine(Test());
		}
		*/
	}
	/*
	IEnumerator Test()
	{
		nav.enabled = false;
		rigid.AddForce(Vector3.up * 10, ForceMode.Impulse);
		yield return new WaitForSeconds(5f);
		nav.enabled = true;
	}
	*/

	//플레이어 탐색
	private void Targeting()
	{
		float targetRadius = 0;
		float targetRange = 0;

		//몬스터 타입에 따라 탐색범위
		switch (type)
		{
			case Type.A:
				targetRadius = 1f;
				targetRange = 3f;
				break;
			case Type.B:
				targetRadius = 2f;
				targetRange = 15f;
				break;
			case Type.C:
				targetRadius = 0.5f;
				targetRange = 25f;
				break;
			case Type.L:
				targetRadius = 1f;
				targetRange = 13f;
				break;
			case Type.M:
				targetRadius = 1f;
				targetRange = 3f;
				break;
		}

		//탐색
		RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

		if (rayHits.Length > 0 && !isAttack && curHealth > 0 &&!Stun)
		{
			isAttack = true;
			Attack_coroutine = Attack();
			StartCoroutine(Attack_coroutine);
		}
	}

	private IEnumerator Attack()
	{	
		isChase = false;
		anim.SetBool("Attack", true);
		anim.SetBool("Walk", false);
		switch (type)
		{
			case Type.A:
				//foreach (MeshRenderer mesh in meshs) { mesh.material.color = Color.yellow; }
				yield return new WaitForSeconds(0.5f);
				attackArea.enabled = true;
				yield return new WaitForSeconds(0.3f);
				attackArea.enabled = false;

				yield return new WaitForSeconds(0.5f);
				//foreach (MeshRenderer mesh in meshs) { mesh.material.color = Color.white; }
				break;

			case Type.B:
				//foreach (MeshRenderer mesh in meshs) { mesh.material.color = Color.yellow; }
				yield return new WaitForSeconds(0.1f);
				rigid.angularVelocity = Vector3.zero;
				anim.SetBool("ReadyToAction", true);
				yield return new WaitForSeconds(0.7f);
				rigid.angularVelocity = Vector3.zero;
				rigid.AddForce(transform.forward * 250, ForceMode.Impulse);
				attackArea.enabled = true;

				yield return new WaitForSeconds(1f);
				rigid.velocity = Vector3.zero;
				attackArea.enabled = false;
				rigid.angularVelocity = Vector3.zero;
				anim.SetBool("ReadyToAction", false);
				yield return new WaitForSeconds(3f);
				//foreach (MeshRenderer mesh in meshs) { mesh.material.color = Color.white; }
				break;

			case Type.C:
				yield return new WaitForSeconds(0.5f);
				GameObject instantiateBullet = Instantiate(bullet, transform.position, transform.rotation);
				Rigidbody rigidBullet = instantiateBullet.GetComponent<Rigidbody>();
				rigidBullet.velocity = transform.forward * 20;

				yield return new WaitForSeconds(2f);
				break;

			case Type.L:
				nav.enabled = false;
				rigid.angularVelocity = Vector3.zero;
				rigid.AddForce((transform.forward + transform.up) * 200, ForceMode.Impulse);
				attackArea.enabled = true;

				yield return new WaitForSeconds(2f);
				while (true)
				{
					yield return new WaitForSeconds(0.1f);
					if (isLanded)
					{
						rigid.velocity = Vector3.zero;

						attackArea.enabled = false;
						
							nav.enabled = true;
						break;
					}
				}
				rigid.angularVelocity = Vector3.zero;
				anim.SetBool("Attack", false);
				yield return new WaitForSeconds(2f);
				anim.SetBool("Walk", true);
				break;

			case Type.M:
				int Phase = Random.Range(0, 4);
				switch (Phase)
				{
					case 0:
					case 1:
					case 2:
						yield return new WaitForSeconds(0.5f);

						attackArea.enabled = true;
						yield return new WaitForSeconds(0.3f);
						attackArea.enabled = false;

						yield return new WaitForSeconds(0.5f);
						break;
					case 3:
						anim.SetBool("Attack02", true);
						yield return new WaitForSeconds(0.35f);
						rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
						attackArea.enabled = true;
						attackArea.tag = "EnemyBullet_Renforcement";
						Bullet_script e_b = attackArea.GetComponent<Bullet_script>();
						e_b.damage += 2;

						yield return new WaitForSeconds(1f);
						rigid.velocity = Vector3.zero;
						attackArea.tag = "EnemyBullet";
						e_b.damage -= 2;
						attackArea.enabled = false;
						anim.SetBool("Attack02", false);
						anim.SetBool("Attack",false);
						yield return new WaitForSeconds(2f);

						break;

				}
				break;
		}


		anim.SetBool("Attack", false);
		anim.SetBool("Walk", true);
		isChase = true;
		isAttack = false;

	}

	private void AttackOut()
	{
		isChase = false;
		isAttack = false;
		if (attackArea != null)
			attackArea.enabled = false;
		if (type != Type.Chicken)
			anim.SetBool("Attack", false);
	}

	void FixedUpdate()
	{
		if (isStart)
		{
			if (!isDie && !Stun)
			{
				if (type != Type.Chicken)
					Targeting();
				FreezeVelocity();
			}
			IsGround();


			//치킈잉ㅇㅇㅇㅇㅇㅇㄴ
			if (type == Type.Chicken)
			{
				RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 5, Vector3.up, 0f, LayerMask.GetMask("Player" , "PlayerInvincibility") );
				if (rayHits.Length != 0)
				{
					freezenchicken = true;
					if (nav.enabled)
					{
						anim.SetBool("Walk", false);
						rigid.velocity = Vector3.zero;
						//nav.isStopped = true;
						nav.enabled = false;
					}
				}
				else if (!nav.enabled)
				{
					RaycastHit[] rayHits2 = Physics.SphereCastAll(transform.position, 25, Vector3.up, 0f, LayerMask.GetMask("Player", "PlayerInvincibility"));
					if (rayHits2.Length == 0)
					{
						freezenchicken = false;
						if (nav.isOnNavMesh)
							nav.enabled = true;
						//nav.isStopped = false;
						rigid.velocity = Vector3.zero;
						rigid.angularVelocity = Vector3.zero;
					}
				}
			}
		}

	}


	protected void FreezeVelocity()
	{
		if (isChase)
		{
			rigid.velocity = Vector3.zero;
		}
		rigid.angularVelocity = Vector3.zero;
	}

	//이걸로 맞았는지 안맞았는지 구현하셈
	protected void OnCollisionEnter(Collision collision)
	{

		//Debug.Log(collision.gameObject.tag);
		
		if ((collision.gameObject.CompareTag("Bullet") || collision.gameObject.CompareTag("Close_Weapon") || collision.gameObject.CompareTag("Can_Grab")
			|| collision.gameObject.CompareTag("Trap_dam")))
		{
			if (isEffect) {
				ContactPoint a = collision.contacts[0];
				//Debug.Log(collision.gameObject.tag+" 닿음 "+a.point);
				efsc.Select_effect(a.point + (Vector3.up * 2), 1);
				isEffect = false;
			}
		}
	}


	protected void OnTriggerEnter(Collider other)
	{
		if (!Invincibility)
		{
			if (other.CompareTag("Close_Weapon"))
			{
				Weapon_script weapon = other.GetComponent<Weapon_script>();
				isEffect = true;
				curHealth -= weapon.damage;

				if (weapon.sub_type.Equals(Weapon_script.Sub_Type.Knife) || weapon.sub_type.Equals(Weapon_script.Sub_Type.Watermelone)) {
					weapon.Ammo--;
				}



				Vector3 reactVec = transform.position - other.transform.position;
				reactVec.y = 0f;
				StartCoroutine(Ondamage(reactVec.normalized, false));
			}
			else if (other.CompareTag("Bullet"))
			{
				isEffect = true;
				Bullet_script bullet = other.GetComponent<Bullet_script>();
				curHealth -= bullet.damage;
				Vector3 reactVec = transform.position - other.transform.position;
				reactVec.y = 0f;

				Destroy(other.gameObject);

				StartCoroutine(Ondamage(reactVec.normalized, false));
			}
			//물품던졌을때 속력구하기
			else if (other.CompareTag("Can_Grab"))
			{

				if (!Invincibility && type != Type.Chicken)
				{
					if (other.GetComponent<Grab_Speed>().velocity >= 30)
					{
						isEffect = true;
						Invincibility = true;
						curHealth -= 1;
						Vector3 reactVec = transform.position - other.transform.position;
						reactVec.y = 0.5f;
						StartCoroutine(Ondamage(reactVec.normalized, true));
					}
				}

			}

			else if (other.CompareTag("Trap_dam"))
			{
				if (!Invincibility)
				{
					isEffect = true;
					Trap_range Trap_r = other.gameObject.GetComponent<Trap_range>();
					if (Trap_r.dotDam)
						return;
					Invincibility = true;
					Vector3 reactvec = transform.position - other.gameObject.transform.position;
					reactvec.y = 0.5f;
					curHealth -= Trap_r.Damage;

					StartCoroutine(Ondamage(reactvec.normalized, Trap_r.isNuck));
				}
			}
		}
	}

	protected void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Trap_dam"))
		{
			Trap_range dummy = other.gameObject.GetComponent<Trap_range>();
			if (dummy.dotDam)
			{
				isBurn = true; BurnOff = false;
				StopCoroutine(Burn);
				dot_dam += Time.deltaTime;
			}
			else if (!Invincibility)
			{
				Invincibility = true;
				Vector3 reactvec = transform.position - other.gameObject.transform.position;
				reactvec.y = 0.5f;
				curHealth -= dummy.Damage;

				StartCoroutine(Ondamage(reactvec.normalized, dummy.isNuck));
			}

		}
	}

	protected IEnumerator Damage_color()
	{
		if (isDam && type != Type.M)
		{
			yield return new WaitForSeconds(0.1f);
			foreach (MeshRenderer mesh in meshs)
			{ mesh.material.color = Color.gray; }
			yield return new WaitForSeconds(0.1f);
			foreach (MeshRenderer mesh in meshs)
			{ mesh.material.color = Color.white; }
			isDamS = false;
		}
		else if (isDam)
		{
			yield return new WaitForSeconds(0.1f);
			smesh.material.color = Color.gray;
			yield return new WaitForSeconds(0.1f);
			smesh.material.color = Color.white;
			isDamS = false;
		}
	}

	protected void Burn_dam()
	{
		if (isBurn)
		{
			dot_dam += Time.deltaTime;
			if (type != Type.M)
			{
				foreach (MeshRenderer mesh in meshs)
				{ mesh.material.color = new Color(255 / 255f, 120 / 255f, 100 / 255f); }
			}
			else
				smesh.material.color = new Color(255 / 255f, 120 / 255f, 100 / 255f);

			if (dot_dam >= 2f)
			{
				efsc.Select_effect(transform.position, 4);
				curHealth -= 1;
				dot_dam = 0;
			}
		}
		else if (!isBurn && dot_dam != 0)
		{
			dot_dam -= Time.deltaTime;
			if (dot_dam <= 0f)
				dot_dam = 0;
		}

		if (!BurnOff && isBurn)
		{ Burn = Burn_Off(); StartCoroutine(Burn); }
	}
	protected IEnumerator Burn_Off()
	{
		BurnOff = true;
		yield return new WaitForSeconds(3f);
		isBurn = false;
		BurnOff = false;
		if (type != Type.M)
		{
			foreach (MeshRenderer mesh in meshs)
			{ mesh.material.color = Color.white; }
		}
		else
			smesh.material.color = Color.white;
	}



	public void HitByGrenade(Vector3 explosionpos)
	{
		if (!Invincibility)
		{
			curHealth -= 7;
			Vector3 reactVec = transform.position - explosionpos;
			reactVec.y = 0.5f;
			StartCoroutine(Ondamage(reactVec.normalized, true));
		}
	}

	private IEnumerator Ondamage(Vector3 reactVec, bool isg)
	{
		Invincibility = true;
		isChase = false; Stun = true;
		if (type != Type.Boss)
			anim.SetBool("Walk",false);
		state.Sound[0].GetComponent<AudioSource>().Play();
		//if (!isAttack)
		isDam = true;
		if (nav.enabled)
			nav.enabled = false;

		if (isg)
		{
			reactVec = reactVec.normalized;
			reactVec += Vector3.up;
			if (type != Type.Chicken)
				rigid.AddForce(reactVec * 50, ForceMode.Impulse);
			else
				rigid.AddForce(reactVec * 10, ForceMode.Impulse);
		}
		else
		{
			reactVec = reactVec.normalized;
			reactVec += Vector3.up;
			if (type != Type.Chicken)
				rigid.AddForce(reactVec * 25, ForceMode.Impulse);
			else
				rigid.AddForce(reactVec * 5, ForceMode.Impulse);
		}
		
		yield return new WaitForSeconds(0.2f);

		if (curHealth > 0)
		{
			yield return new WaitForSeconds(1f);

				Invincibility_reset();

			yield return new WaitForSeconds(1f);

				if (type != Type.Chicken)
				{
					while (true)
					{
						yield return new WaitForSeconds(0.1f);
						if (isLanded)
						{
							isChase = true;
							
								nav.enabled = true;
							anim.SetBool("Walk", true);
							break;
						}
					}
				}
				Stun = false;



		}
	}

	private IEnumerator Dead()
	{
		if (curHealth <= 0)
		{
			if (type != Type.Chicken)
				rigid.mass = 1;
			isDie = true;
			RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 60, Vector3.up, 0f, LayerMask.GetMask("Player"));
			if (rayHits.Length != 0)
			{
				GameObject.FindWithTag("Player").GetComponent<player_script>().score += state.Score;
			}

			StopCoroutine(Attack_coroutine);
			AttackOut();
			
			//if(nav.isOnNavMesh)
				//nav.enabled = true; nav.isStopped = true;
			
			isDam = false;
			gameObject.layer = 12;
			nav.enabled = false;
			if (type != Type.Chicken && type != Type.L)
				anim.SetTrigger("Die");
			state.Sound[1].GetComponent<AudioSource>().Play();
			rigid.velocity = Vector3.zero;
			rigid.angularVelocity = Vector3.zero;
			CanGrab.SetActive(true);			
			if (type == Type.L)
			{ anim.SetTrigger("Die"); yield return new WaitForSeconds(0.2f); anim.speed = 0.0f; gameObject.GetComponent<CapsuleCollider>().radius = 0.55f; }
			else if (type == Type.M)
			{ gameObject.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.55f, 0.7f); }

			if (type == Type.Chicken)
			{
				if (isBurn)
				{
					GameObject dummy_h = Instantiate(efsc.Item[0]) as GameObject;
					dummy_h.transform.position = transform.position + Vector3.up * 2;
				}
				efsc.Select_effect(transform.position + (Vector3.up * 2), 3);
				Destroy(gameObject, 0.5f);
			}


			yield return new WaitForSeconds(0.7f);
			if (type != Type.M)
			{ foreach (MeshRenderer mesh in meshs) { mesh.material.color = Color.gray; } }
			else
				smesh.material.color = Color.gray;



			gameObject.GetComponent<Enemy_script>().enabled = false;

			//Destroy(gameObject, 3);
		}
	}

	protected void Invincibility_reset() { 
		Invincibility = false;
		isDam = false;
	}

	protected float Random_absover(float from, float over)
	{
		float res;
		res = Random.Range(over, from) * (Mathf.Pow(-1f,Random.Range(0,2)));
		return res;
	}

	//플레이어 높이 위치 탐색 문
	
    private void IsGround(){    //땅에 닿았는가?
        //Raycast를 플레이어 아래로 쏴 땅이 있는지 확인
        Vector3 p_position = transform.position;
        Vector3 down_position = p_position + Vector3.down * 0.5f;
		
        RaycastHit hit;
        isLanded = false;
        if (Physics.Linecast(p_position, down_position, out hit))
            isLanded = true;
		if (!isLanded)
			timer_air += Time.deltaTime;
		else timer_air = 0;

		if (timer_air > 3f && type != Type.Chicken)
		{
			timer_air = 0;
			transform.position = transform.position - (Vector3.up * 1);
		}
		else if (transform.position.y < -50)
			Destroy(gameObject);
		else if (transform.position.y < -20)
		{//Debug.Log(transform.position +"  " + gameObject.name);
		 //Destroy(gameObject); 
			transform.position = transform.position + new Vector3(0, 6, 0);
		}
		

		if (!nav.enabled && !isDam && isLanded && !isAttack &&!isDie )
		{ nav.enabled = true; }
		else if (nav.enabled && !isLanded)
		{ nav.enabled = false; }
    }

}
