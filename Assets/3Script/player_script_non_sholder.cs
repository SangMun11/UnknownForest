using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum STANDARD_p
//{
  //  STD_SPEED = 20,
//};

public class player_script_non_sholder : MonoBehaviour
{
    public Camera FollowCamera;
    private float verti_in = 0, hori_in = 0, fDelay;
    private bool isRun = false, isDodge = false, canDodge = true, isSwap, isGrab=false;
    private bool isiDown, isfDown, isfReady, isReload, isRdown, isMove, isTurn, Grab_Swap;
    private float speed = (float)STANDARD_p.STD_SPEED, Grab_radius;

    //private bool isLanded;
    Animator anim;
    Vector3 direction, direction_attack;
    Vector3 Dodge_v;
    Rigidbody player_r, Grab_t;
    BoxCollider Grab_b;

    IEnumerator Throw;
    GameObject nearObject, grabObject;
    MeshRenderer[] meshs;
    public Weapon_script equipWeapon;
    private int equipWeapon_Index = -1, grab_dummy_weapon =-1, WeaponIndex = -1;



    public GameObject[] Weapons;
    public GameObject Grenadeobject;
    public Transform GrabPosition;
    public bool[] hasWeapons;
    public int pAmmo, hasGrenades, pHealth, MaxpAmmo, maxpHealth, MaxpGrenades;
    bool sDown1, sDown2, gDown;
    bool isBorder, isBorder_x, isBorder_z, invincibility;

    //스타트 문
    void Start(){
        player_r = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        Throw = Throw_Material();
    }

    //업데이트 문
    void Update()
    {
        User_Input();
        p_Move();
        Dodge();        
        Interaction();
        Swap();
        Attack();
        Reload();
        Granade();
        Grab();
    }

	void FixedUpdate()
	{
        //이동관련
        do
        {
            isTurn = false;
            if (canDodge && !isSwap)
            {
                if (equipWeapon != null)
                {
                    if (!equipWeapon.isMotion && !isReload)
                    { isTurn = true; speed = (int)STANDARD_p.STD_SPEED; }
                    else if (isReload) { isTurn = true; }
                    else { speed = (int)STANDARD_p.STD_SPEED * 0.5f; isTurn = false; }
                }
                else { isTurn = true; speed = (int)STANDARD_p.STD_SPEED; }
            }
            if (isTurn)
            {
                if (direction != Vector3.zero)
                { player_r.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.3f)); }

                p_Turn();
            }
            if (!isBorder)
                player_r.MovePosition(player_r.position + direction * speed * Time.deltaTime * ((isRun) ? 0.5f : 1f));
            player_r.angularVelocity = Vector3.zero;
        } while (false);
        StopToWall();
	}



    //사용자 입력 문
    void User_Input(){
        verti_in = Input.GetAxis("Vertical");
        hori_in = Input.GetAxis("Horizontal");
        isMove = Input.GetButton("Vertical") || Input.GetButton("Horizontal");
        isRun = Input.GetButton("Run");
        isDodge = Input.GetButtonDown("Jump");
        isfDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        isiDown = Input.GetButtonDown("Interaction");
        isRdown = Input.GetButtonDown("Reload");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
    }

    void p_Move(){
        direction = new Vector3(hori_in, 0, verti_in).normalized;

        if (!canDodge) direction = Dodge_v;
        if (!isMove && canDodge) direction = Vector3.zero;
        //IsGround();
        
        //if (!isLanded)//Velocity로 이동시 낙하 속도에 대한 문제 존재.
            //transform.Translate(-Vector3.up *gravity* Time.deltaTime);
        if (isBorder_x)
            direction.x /= 4;
        if (isBorder_z)
            direction.z /= 4;

        anim.SetBool("Run", isMove);
        anim.SetBool("Walk", isRun);
    }

    void StopToWall()
    {
        isBorder = Physics.Raycast(transform.position, direction, 2, LayerMask.GetMask("Wall")) ||
            Physics.Raycast(transform.position, direction, 2, LayerMask.GetMask("Water"));
        isBorder_x = Physics.Raycast(transform.position, Vector3.right*direction.x, 2, LayerMask.GetMask("Wall")) ||
            Physics.Raycast(transform.position, Vector3.right * direction.x, 2, LayerMask.GetMask("Water"));
        isBorder_z = Physics.Raycast(transform.position, Vector3.forward * direction.z, 2, LayerMask.GetMask("Wall")) ||
            Physics.Raycast(transform.position, Vector3.forward * direction.z, 2, LayerMask.GetMask("Water"));
        
    }

    void p_Turn()
    {
        //키보드 회전
        //transform.LookAt(direction + transform.position);

        //마우스 회전
        if (isfDown)
        {
            Ray ray = FollowCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }

    }


    //회피문
    void Dodge(){
        if (equipWeapon != null)
            if (equipWeapon.isMotion)
                return;
        if (isDodge && canDodge && isMove){
            transform.LookAt(direction + transform.position);
            Dodge_v = direction;
            speed = (float)STANDARD_p.STD_SPEED*2;
            canDodge = false;
            anim.SetTrigger("Dodge");
            isReload = false;
            StopCoroutine("Reload_time");
            invincibility = true;
            Invoke("Dodge_reset", 0.5f);
        }
    }
    void Dodge_reset(){
        invincibility = false;
        speed = (float)STANDARD_p.STD_SPEED;
        canDodge = true;

    }


    //공격문
    void Attack()
    {
        if (equipWeapon == null || isGrab)
            return;

        fDelay += Time.deltaTime;
        isfReady = equipWeapon.rate < fDelay;

        if (isfDown && isfReady && canDodge && !isSwap && !isReload)
        {
            p_Turn();
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon_script.Type.Short? "Swing" : equipWeapon.Ammo != 0? "Shot" : "none");

            if (equipWeapon.type == Weapon_script.Type.Long && equipWeapon.Ammo == 0)
                Reload();
            fDelay = 0;
        }
    }

    //폭탄
    void Granade()
    {
        if (hasGrenades == 0)
            return;
        if (gDown)
        {
            Ray ray = FollowCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 2;

                GameObject instantgrenade = Instantiate(Grenadeobject, transform.position, transform.rotation);
                Rigidbody rigidGranade = instantgrenade.GetComponent<Rigidbody>();
                rigidGranade.AddForce(nextVec.normalized*5, ForceMode.Impulse);

                hasGrenades--;
                //Granade[hasGranades].setActive(false);
            }
        }
    }

    //권총 재장전 문
    void Reload()
    {
        if (pAmmo == 0 || !canDodge || isGrab || Grab_Swap)
            return;
        if (equipWeapon == null)
            return;
        if (equipWeapon.type != Weapon_script.Type.Long)
            return;

        if ((isRdown || isfDown) && !isDodge && !isSwap && isfReady && !isReload)
        {
            isReload = true;
            anim.SetTrigger("Reload");
            speed = (float)STANDARD_p.STD_SPEED/2;
            //Invoke("ReloadOut", 2f);
            StartCoroutine("Reload_time");
        }
    }

    IEnumerator Reload_time()
    {
        yield return new WaitForSeconds(2);
        ReloadOut();
    }

    void ReloadOut()
    {
        if (pAmmo > 0 && isReload)
        {
            speed = (float)STANDARD_p.STD_SPEED;
            pAmmo -= 1;
            equipWeapon.Ammo = 1;
        }
        isReload = false;
    }

    //무기 변경 문
    void Swap()
    {
        if (isGrab) return;
        //int WeaponIndex = -1;
        if (sDown1) WeaponIndex = 0;
        if (sDown2) WeaponIndex = 1;

        if (WeaponIndex == equipWeapon_Index && !Grab_Swap)
            return;
        if (WeaponIndex >=0 && !Grab_Swap)
            if (!hasWeapons[WeaponIndex])
                   return;

        if ((sDown1 || sDown2) && !isDodge || Grab_Swap)
        {
            if (Grab_Swap) Grab_Swap = false;
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);
            equipWeapon = Weapons[WeaponIndex].GetComponent<Weapon_script>();
            equipWeapon.gameObject.SetActive(true);
            equipWeapon_Index = WeaponIndex;

            anim.SetTrigger("Swap");
            isSwap = true;
            isReload = false;
            StopCoroutine("Reload_time");
            speed = (int)STANDARD_p.STD_SPEED / 2;
            Invoke("Swap_reset", 0.5f);
        }
    }
    void Swap_reset()
    {
        speed = (int)STANDARD_p.STD_SPEED;
        isSwap = false;
    }


    //아이템 입수 코드
    void Interaction()
    {
        if (nearObject != null)
        {
            if (nearObject.tag == "Weapon")
            {
                Item_Script item = nearObject.GetComponent<Item_Script>();
                int weaponIndex = item.value;
                if (item.value == 1)
                    pAmmo += 10;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
            else if (nearObject.tag == "Can_Grab" && isiDown && !isGrab)
            {
                isGrab = true;
                grabObject = nearObject;
                nearObject = null;
                if (equipWeapon != null)
                    equipWeapon.gameObject.SetActive(false);
                equipWeapon = null;
                grab_dummy_weapon = WeaponIndex;
                Grab_t = grabObject.GetComponentInParent<Rigidbody>();
                Grab_b = grabObject.GetComponentInParent<BoxCollider>();
                Grab_radius = Grab_b.size.z;
                GrabPosition.Translate(new Vector3(0, 0, -Grab_radius-0.5f));
                WeaponIndex = -1;
            }
        }
    }

    void Grab()
    {
        if (isGrab && grabObject == null) { isGrab = false; WeaponIndex = grab_dummy_weapon; if (grab_dummy_weapon != -1) { Grab_Swap = true; }
            GrabPosition.Translate(new Vector3(0, 0, +Grab_radius + 0.5f));
            StopCoroutine(Throw);
        }
        else if (isGrab)
        {
            grabObject.transform.tag = "Untagged";
            StopCoroutine("Reload_time");
            if (grabObject == null)
            { isGrab = false; return; }

            //폭탄이면서 만약 터졌다면 내려놓거라
            if (grabObject.transform.parent.gameObject.tag == "Item")
            { if (grabObject.GetComponentInParent<Grenade_script>().isTime) { grabObject = null; return; } }

            Grab_t.position = GrabPosition.position;

            Grab_t.velocity = Vector3.zero;
            Grab_t.angularVelocity = Vector3.zero;
            if (direction != Vector3.zero)
                Grab_t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.3f);

            if (Input.GetButtonDown("Fire1"))
            {
                Throw = Throw_Material();
                StartCoroutine(Throw);
            }

        }
        else
        {
            grabObject = null;
        }

    }
    IEnumerator Throw_Material()
    {
        anim.SetTrigger("Throw");
        yield return new WaitForSeconds(0.1f);


        Ray ray = FollowCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, 100))
        {
            Vector3 nextVec = rayHit.point - transform.position;
            Grab_t.AddForce(nextVec.normalized * 50, ForceMode.Impulse);
        }
        grabObject.transform.tag = "Can_Grab";
        GrabPosition.Translate(new Vector3(0, 0, +Grab_radius + 0.5f));

        grabObject = null; isGrab = false;
        WeaponIndex = grab_dummy_weapon;
        yield return new WaitForSeconds(0.5f);
        if (grab_dummy_weapon != -1)
            Grab_Swap = true;
        yield return null;
    }
    

	//플레이어 주변 아이템 탐색 문
	void OnTriggerEnter(Collider other)
	{
        if (other.tag == "Item")
        {
            Item_Script item = other.GetComponent<Item_Script>();
            switch (item.Type_I)
            {
                case Item_Script.Type.Ammo:
                    pAmmo += item.value;
                    if (pAmmo > MaxpAmmo)
                        pAmmo = MaxpAmmo;
                    break;
                case Item_Script.Type.Boom:
                    hasGrenades += item.value;
                    if (hasGrenades > MaxpGrenades)
                        hasGrenades = MaxpGrenades;
                    break;
                case Item_Script.Type.heart:
                    pHealth += item.value;
                    if (pHealth > maxpHealth)
                        pHealth = maxpHealth;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {
            if (!invincibility)
            {
                Bullet_script enemyBullet = other.GetComponent<Bullet_script>();
                pHealth -= enemyBullet.damage;
                if (other.GetComponent<Rigidbody>() != null)
                    Destroy(other.gameObject);
                StartCoroutine(OnDamage());
            }
        }
	}
    public void HitByGrenade()
    {
        if (!invincibility)
        {
            pHealth -= 5;
            StartCoroutine(OnDamage());
        }
    }

    IEnumerator OnDamage()
    {
        invincibility = true;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }
        yield return new WaitForSeconds(1f);
        invincibility = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
    }

	void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Item" || other.tag == "Can_Grab")
            nearObject = other.gameObject;


        //Debug.Log("" + other.name);
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Item" || other.tag == "Can_Grab")
            nearObject = null;
    }


    //플레이어 높이 위치 탐색 문
    /*
    void IsGround(){    //땅에 닿았는가?
        //Raycast를 플레이어 아래로 쏴 땅이 있는지 확인
        Vector3 p_position = transform.position;
        Vector3 down_position = p_position + Vector3.down * 1.0f;

        RaycastHit hit;
        isLanded = false;
        if (Physics.Linecast(p_position, down_position, out hit))
            isLanded = true;
    }*/
}
