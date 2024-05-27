using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_script : MonoBehaviour
{
    public bool isMelee;
    public GameObject bullet, Effects;
    protected GameObject gobj, DunCrea;
    protected bool isShot, hit;
    protected Rigidbody bullet_rig;

	private void Start()
	{
        gobj = GameObject.Find("GameManager");
        int Spawn_stage = Random.Range(1 * gobj.GetComponent<GameManager>().stage, 11);
        switch (Spawn_stage)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
                Destroy(gameObject);
                break;
            case 9:
            case 10:
                break;
        }

        if (!isMelee)
            bullet_rig = bullet.GetComponent<Rigidbody>();
        DunCrea = GameObject.FindWithTag("DungeonCreator");
    }
	void Update()
	{

        if (!isMelee && !isShot)
            TargetSearch();
	}
    /*
	private void OnDrawGizmos()
	{
        float targetRange = 30f;
        RaycastHit rayHits;
        hit = Physics.Raycast(transform.position + transform.forward * 4, transform.forward, out rayHits, targetRange);

        Gizmos.color = Color.red;
        if (hit)
        {
            Gizmos.DrawRay(transform.position, transform.forward * rayHits.distance);
        }
        else
        {
            Gizmos.DrawRay(transform.position, transform.forward * targetRange);
        }
    }
    */
	void TargetSearch()
    {
        //float targetRadius = 1f;
        float targetRange = 30f;
        //RaycastHit[] rayHits = Physics.SphereCast(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player", "Player_bullet", "Enemy_bullet", "Can_Grab", "Trap", "Wall"));
        RaycastHit rayHits;
        //hit = Physics.SphereCast(transform.position + transform.forward * 4, targetRadius, transform.forward, out rayHits, targetRange);
        hit = Physics.Raycast(transform.position + transform.forward*3.5f - transform.up *0.3f, transform.forward, out rayHits, targetRange);



        if (hit)
        {
            switch (rayHits.transform.gameObject.layer)
            {
                case 6:
                case 8:
                case 11:
                case 12:
                case 13:
                case 14:
                case 18:
                case 19:
                    isShot = true;
                    Effects.GetComponent<AudioSource>().Play();
                    bullet.transform.Find("GrabRange").gameObject.SetActive(true);
                    bullet.transform.Find("GrabRange").gameObject.layer = 14;
                    bullet.layer = 13;
                    bullet.transform.parent = DunCrea.transform;
                    bullet_rig.isKinematic = false;
                    bullet_rig.useGravity = true;
                    Invoke("Shoot", 0.001f);
                    break;
            }

        }
    }
    void Shoot()
    {
        bullet_rig.AddForce(transform.forward * 4000);
        gameObject.GetComponent<Trap_script>().enabled = false;
    }

}
