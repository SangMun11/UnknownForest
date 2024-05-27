using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_script : MonoBehaviour
{
    public enum Type { Short, Long};
    public enum Sub_Type { None, Knife, Axe, Watermelone, WMS };
    public Type type;
    public Sub_Type sub_type;
    public int damage;
    public float rate;
    public int Ammo, maxAmmo;
    public bool isMotion, can_logging, islog;

    public BoxCollider[] RangeArea;
    //public BoxCollider RangeArea;
    public TrailRenderer Effect;
    public Transform bulletPosition;
    public GameObject bullet, bullet_Reinforce;
    public Transform bulletCasePosition;
    public GameObject bulletCase;
    private GameObject obj;

    private player_script player;
    private bool hit;
    private Effect_script efsc;

    public void Awake()
    {
        obj = GameObject.Find("Object_Manager");
        efsc = obj.GetComponent<Effect_script>();
        RangeArea = gameObject.GetComponents<BoxCollider>();
        player = gameObject.GetComponentInParent<player_script>();
    }

	void Start()
	{
        isMotion = false;
	}

	void Update()
	{
        if (can_logging && islog)
        {
            RaycastHit rayHits;
            hit = Physics.Raycast(player.transform.position + Vector3.up * 2, player.transform.forward, out rayHits, 2f);
            if (hit)
            {
                if (rayHits.transform.gameObject.CompareTag("Can_Destroed_Wall"))
                {
                    islog = false; can_logging = false;
                    obj.GetComponent<Debris_Create>().Breaking(rayHits.transform, 1000, 0);

                    Destroy(rayHits.transform.gameObject);
                    StartCoroutine(logout());
                    Ammo--;
                }
            }
        }



    }

    /*
    void OnDrawGizmos()
    {
        RaycastHit rayHits;
        // Physics.Raycast (레이저를 발사할 위치, 발사 방향, 충돌 결과, 최대 거리)
        hit = Physics.Raycast(player.transform.position + Vector3.up * 2, player.transform.forward, out rayHits, 4f);

        Gizmos.color = Color.red;
        if (hit)
        {
            Gizmos.DrawRay(player.transform.position + Vector3.up * 2, player.transform.forward * rayHits.distance);
        }
        else
        {
            Gizmos.DrawRay(player.transform.position + Vector3.up * 2, player.transform.forward * 2f);
        }
    }*/






    public void Use()
    {

        if (type == Type.Short)
        {
            isMotion = false;
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Long && Ammo > 0)
        {
            Ammo--;
            StartCoroutine("Shot");
        }
        else return;
    }

    IEnumerator Swing()
    {
        yield return null;
        isMotion = true;
        if (sub_type == Sub_Type.Axe)
            can_logging = true;
        yield return new WaitForSeconds(0.2f);

        foreach(BoxCollider range in RangeArea)
            range.enabled = true;
        //RangeArea.enabled = true;
        Effect.enabled = true;
        yield return new WaitForSeconds(0.025f);
        islog = true;

        yield return new WaitForSeconds(0.1f);
        foreach (BoxCollider range in RangeArea)
            range.enabled = false;
        //RangeArea.enabled = false;
        yield return new WaitForSeconds(0.1f);
        Effect.enabled = false;
        isMotion = false;
        islog = false;

    }
    
    IEnumerator Shot()
    {
        yield return null;
        GameObject instantBullet;

        if (!player.isRein)
            instantBullet = Instantiate(bullet, bulletPosition.position, bulletPosition.rotation);
        else
            instantBullet = Instantiate(bullet_Reinforce, bulletPosition.position, bulletPosition.rotation);

        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPosition.forward * 200;

        yield return new WaitForSeconds(0.1f);
        GameObject intantBulletcase = Instantiate(bulletCase, bulletCasePosition.position, bulletCasePosition.rotation);
        Rigidbody bulletCaseRigid = intantBulletcase.GetComponent<Rigidbody>();
        Vector3 caseVec = new Vector3(0,0,-1f) * Random.Range(3,2) + Vector3.up * Random.Range(1,2);
        bulletCaseRigid.AddForce(caseVec, ForceMode.Impulse);
        bulletCaseRigid.AddTorque(Vector3.up * 2);
    }

    public IEnumerator logout()
    {
        yield return new WaitForSeconds(0.5f);
        can_logging = true;
    }

	void OnCollisionEnter(Collision collision)
	{
        if (collision.transform.gameObject.CompareTag("Can_Destroed_Wall") && can_logging){

            Destroy(collision.gameObject);
        }
	}
	/*
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("닿음?>");
        ContactPoint point = collision.contacts[0];
        efsc.Select_effect(point.point, 1);
    }*/
}
