using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Wall : MonoBehaviour
{
	public GameObject player;
	public GameObject Camera;
	public int type;

	public float camera_dist, camera_fix = 2f;
	private float camera_width = -10f, camera_height = 2f;

	public Vector3 dir;

	void Start()
	{
		switch (type)
		{
			case 0:
				camera_width = -10f; camera_height = 2f; camera_fix = 2f;
				break;
			case 1:
				camera_width = 1; camera_height = 40f; camera_fix = 2f;
				break;
		}

		camera_dist = Mathf.Sqrt(camera_width * camera_width + camera_height * camera_height);

		dir = new Vector3(0, camera_height, camera_width).normalized;
	}

	private void Update()
	{
		

		Vector3 ray_target = transform.up * camera_height + transform.forward * camera_width;

		RaycastHit hitinfo;
		//Debug.DrawRay(transform.position, dir * camera_dist, Color.red);
		Physics.Raycast(transform.position, ray_target, out hitinfo, 5f);

		if (hitinfo.point != Vector3.zero && (hitinfo.transform.CompareTag("Can_Destroed_Wall") || hitinfo.transform.CompareTag("Cant_Be_Passed_Wall")))
		{
			Camera.transform.position = hitinfo.point;
			Camera.transform.Translate(dir * -1 * camera_fix);

			//cur_dist = 
			
		}
		else
		{
			Camera.transform.localPosition = Vector3.zero;
			Camera.transform.Translate(dir * camera_dist);

			Camera.transform.Translate(dir * -1 * camera_fix);
		}
	}
}
