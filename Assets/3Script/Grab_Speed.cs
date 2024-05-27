using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab_Speed : MonoBehaviour
{
    public float velocity;
    bool isCalc;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCalc)
        {
			IEnumerator cal = cal_speed();
			StartCoroutine(cal);
        }
    }


	IEnumerator cal_speed()
	{
		isCalc = true;
		Vector3 q_0 = transform.transform.position;
		yield return new WaitForSeconds(0.1f);
		Vector3 q_1 = transform.transform.position;

		float distance = Vector3.Distance(q_0, q_1);
		//Debug.Log("distance = " +distance);
		velocity = distance / 0.1f;
		//Debug.Log("velocity = "+velocity);

		isCalc = false;
	}

}
