using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Boss : Trap_script
{
    public bool isOut;

    private void Start()
    {
        gobj = GameObject.Find("GameManager");
        int Spawn_stage = Random.Range(1, 11);
        switch (Spawn_stage)
        {
            case 1:
                Destroy(gameObject);
                break;
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
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
    void TargetSearch()
    {
        float targetRange = 60f;
        RaycastHit rayHits;
        hit = Physics.Raycast(transform.position + transform.forward * 4, transform.forward, out rayHits, targetRange);


        if (hit && !isOut)
        {
            switch (rayHits.transform.gameObject.layer)
            {
                case 6:
                case 8:
                case 11:
                case 12:
                case 13:
                case 14:

                    isShot = true;
                    isOut = true;
                    Effects.GetComponent<AudioSource>().Play();


                    //bullet.transform.Find("GrabRange").gameObject.layer = 14;
                    bullet.layer = 13;
                    bullet_rig.isKinematic = false;
                    bullet_rig.useGravity = true;
                    Invoke("Shoot", 0.05f);
                    break;
            }

        }
    }
    void Shoot()
    {
        bullet_rig.AddForce(transform.forward * 8000);
        isShot = false;
    }
}
