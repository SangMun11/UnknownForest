using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Rock : Bullet_script
{
    Rigidbody rigid;
	public float speed;
    float angularPower = 2, timer;
    float scaleValue = 0.1f;
    bool isShoot, isCalc;
	private Vector3 m_LastPosition;

	private void Awake()
	{
		rigid = GetComponent<Rigidbody>();


	}

	void Update()
	{
		timer += Time.deltaTime;
		if (timer > 2f)
			isShoot = true;
		else
		{
			angularPower += Time.deltaTime*10f;
			scaleValue += Time.deltaTime*0.5f;
			transform.localScale = Vector3.one * scaleValue;

			rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
		}
		if (isShoot && !isCalc)
			StartCoroutine(cal_speed());
	}

	IEnumerator cal_speed()
	{
		isCalc = true;
		Vector3 q_0 = transform.transform.position;
		yield return new WaitForSeconds(0.1f);
		Vector3 q_1 = transform.transform.position;

		float distance = Vector3.Distance(q_0, q_1);
		//Debug.Log("distance = " +distance);
		speed = distance / 0.1f;
		//Debug.Log("velocity = "+velocity);

		isCalc = false;
	}

}
