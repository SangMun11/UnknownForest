using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Top_Camera_Wall : Camera_Wall
{
	private Vector3 Camera_position = new Vector3(0,30,-5);
	private void Start()
	{
		camera_dist = Vector3.Distance(transform.position, transform.position + Camera_position);

		//dir = new Vector3(Camera.transform.position.x , Camera.transform.position.y, Camera.transform.position.z  ).normalized;
		dir = Camera_position.normalized;
		
	}

	private void Update()
	{


		Vector3 ray_target = Camera_position;

		RaycastHit hitinfo;
		Debug.DrawRay(transform.position, dir * camera_dist, Color.red);
		Physics.Raycast(transform.position, ray_target, out hitinfo, camera_dist);

		if (hitinfo.point != Vector3.zero && (hitinfo.transform.CompareTag("Can_Destroed_Wall") || hitinfo.transform.CompareTag("Cant_Be_Passed_Wall")) && hitinfo.transform.gameObject.layer != 4)
		{
			Camera.transform.position = hitinfo.point;
			Camera.transform.Translate(dir * -1 * camera_fix);

			//cur_dist = 

		}
		else
		{
			Camera.transform.localPosition = Vector3.zero;
			//Camera.transform.Translate(dir * camera_dist);
			Camera.transform.position= transform.position + Camera_position;

			Camera.transform.Translate(dir * -1 * camera_fix);
		}
	}
}
