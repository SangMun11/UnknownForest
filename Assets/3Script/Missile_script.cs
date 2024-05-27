using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_script : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right * 30 * Time.deltaTime);
    }
}
