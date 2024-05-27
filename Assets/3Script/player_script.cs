using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STANDARD_p
{
  STD_SPEED = 20,
};

public class player_script : MonoBehaviour
{

    //숄더뷰 카메라.
    public GameObject FollowCamera_game;
    public GameObject FollowCam_game;
    private GameObject Gobj;
    public GameObject AmingPoint;
    private Camera FollowCamera;
    private Transform followcam;

    public int view_position = 0;

    private float verti_in = 0, hori_in = 0, fDelay, dot_dam;
    private bool isRun = false, isDodge = false, canDodge = true, isSwap, isGrab=false, isStun, isBurn, isBurnOff, isDam, isDamS;
    private bool isiDown, isfDown, isfReady, isReload, isRdown, isMove, isTurn, Grab_Swap, isvDown, isThrow, isPortal;
    private float speed = (float)STANDARD_p.STD_SPEED, Grab_radius;
    public GameObject Curdun;

    //private bool isLanded;
    Animator anim;
    Vector3 direction, direction_attack;
    Vector3 Dodge_v;
    Rigidbody player_r, Grab_t;
    CapsuleCollider Grab_b;
    private Effect_script efsc;

    IEnumerator Throw, Damage, Burn;
    [SerializeField]GameObject nearObject, grabObject;
    MeshRenderer[] meshs;
    public Weapon_script equipWeapon;
    private int equipWeapon_Index = -1, grab_dummy_weapon =-1, WeaponIndex = -1;
    [SerializeField]
    private Transform player_cam;

    public Player_Musicmanager EffectMusic;
    public GameObject[] Weapons;
    public GameObject Grenadeobject;
    public Transform GrabPosition, GrabPosition2;
    public bool[] hasWeapons;
    public int pAmmo, hasGrenades, pHealth, Money, MaxpAmmo, maxpHealth, MaxpGrenades, score;
    bool sDown1, sDown2, sDown3, gDown;
    bool isBorder, isBorder_x, isBorder_z, invincibility;
    public bool isRein, isTeleport, isPause, isStop;

    //스타트 문
    void Awake(){
        Gobj = GameObject.Find("GameManager");
        efsc = GameObject.Find("Object_Manager").GetComponent<Effect_script>();
        player_r = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        Throw = Throw_Material();
        Burn = Burn_Off();
        FollowCamera = FollowCamera_game.GetComponent<Camera>();
        followcam = FollowCam_game.GetComponent<Transform>();
    }

    //업데이트 문
    void Update()
    {
        if (pHealth <= 0 && !isStop)
        {OnDie(); EffectMusic.EffectAllOut(); }

        if (grabObject == null)
            Gobj.GetComponent<GameManager>().GrabKey.SetActive(false);



        if (!isStop && Input.GetKeyDown(KeyCode.Escape) && !Gobj.GetComponent<GameManager>().isStart)
        {
            EffectMusic.EffectAllOut();
            if (!isPause) { Gobj.GetComponent<GameManager>().PausePan(); isPause = true; }
            else { Gobj.GetComponent<GameManager>().PauseOut(); isPause = false; }

        }

        User_Input();
        if (!isStun && !isStop && !isPause)
        {
            p_Move();
            Dodge();
            Interaction();
            Swap();
            Attack();
            Reload();
            Granade();
        }
        Grab();
        if (!isDamS && isDam && !isStop && !isPause)
        {
            isDamS = true;
            IEnumerator damage_ie = Damage_color();
            StartCoroutine(damage_ie);
        }

        if (!isStop && !isPause)
        {
            Burn_dam();
            Camera_view();
            LookAround();
            //Debug.DrawRay(followcam.position, new Vector3(followcam.forward.x, 0f, followcam.forward.z).normalized, Color.red);
            followcam.position = player_cam.position;

            if (equipWeapon_Index != -1 && (equipWeapon.sub_type.Equals(Weapon_script.Sub_Type.Axe) || equipWeapon.sub_type.Equals(Weapon_script.Sub_Type.Watermelone) || equipWeapon.sub_type.Equals(Weapon_script.Sub_Type.Knife))
                && equipWeapon.Ammo <= 0)
            {
                Gobj.GetComponent<GameManager>().WeaponIm(-1);
                equipWeapon.gameObject.SetActive(false);
                equipWeapon.Ammo = 0;
                equipWeapon = null;
                hasWeapons[equipWeapon_Index] = false;

                WeaponIndex = -1;
                equipWeapon_Index = -1;
            }

            if (isPortal && !isTeleport && isiDown)
            {

                isTeleport = true;
                isPortal = false;
                Gobj.GetComponent<GameManager>().PortalKey.SetActive(false);
                StartCoroutine(Gobj.GetComponent<Portal_Script>().GotoNext());
            }
            

            /////////////////치투
            if (Input.GetKeyDown(KeyCode.P))
            {
                StartCoroutine(Gobj.GetComponent<Portal_Script>().GotoNext());
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                pHealth += 30;
                pAmmo += 30;
                hasGrenades += 30;
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                GameObject dummy;
                Vector3 ins_for = transform.position + this.transform.localRotation * Vector3.forward * 5;
                dummy = Curdun.GetComponent<Dungeon_Creator>().Instantiate_object(efsc.Weapon_item[1], ins_for + Vector3.up * 3);
                dummy = Curdun.GetComponent<Dungeon_Creator>().Instantiate_object(efsc.Weapon_item[2], ins_for + Vector3.up * 5);
                dummy = Curdun.GetComponent<Dungeon_Creator>().Instantiate_object(efsc.Weapon_item[3], ins_for + Vector3.up * 8);
            }
        }

    }

	void FixedUpdate()
	{
        //이동관련
        do
        {
            isTurn = false;
            //회피동작이 아니며 스왑중이 아닐때 움직임이 가능하다
            if (canDodge && !isSwap && !isStun)
            {
                //무기를 들고있을 때
                if (equipWeapon != null)
                {
                    //무기를 휘두르고 있지 않다면? 
                    if (!equipWeapon.isMotion && !isReload)
                    { isTurn = true; speed = (int)STANDARD_p.STD_SPEED; }
                    else if (isReload) { isTurn = true; }
                    else { speed = (int)STANDARD_p.STD_SPEED; isTurn = false; }
                }
                else { isTurn = true; speed = (int)STANDARD_p.STD_SPEED; }
            }

            //이동 방향으로 바라보기
            if (isTurn && !isStun && !isThrow)
            {
                if (direction != Vector3.zero)
                { player_r.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.3f)); }

                p_Turn();
            }

            //이동
            if (!isBorder && !isStun)
            {
                player_r.MovePosition(player_r.position + direction * speed * Time.deltaTime * ((isRun) ? 0.5f : 1f));
                if (direction != Vector3.zero && canDodge)
                {
                    if (!isRun)
                    {
                        EffectMusic.EffectPlay(0);
                        EffectMusic.EffectOut(1);
                    }
                    else
                    {
                        EffectMusic.EffectOut(0);
                        EffectMusic.EffectPlay(1);
                    }
                }
                else
                {
                    EffectMusic.EffectOut(0);
                    EffectMusic.EffectOut(1);
                }
            }///////////////////////////
            else
            { EffectMusic.EffectOut(0); }
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
        sDown3 = Input.GetButtonDown("Swap3");
        isvDown = Input.GetButtonDown("View_Change");
    }

    void p_Move() {

        switch (view_position) {
            case 0:
                Vector3 directionfor = new Vector3(followcam.forward.x, 0f, followcam.forward.z).normalized;
                Vector3 directionrig = new Vector3(followcam.right.x, 0f, followcam.right.z).normalized;
                direction = directionfor * verti_in + directionrig * hori_in;
                break;
            case 1:
                direction = new Vector3(hori_in, 0, verti_in).normalized;
                break;
    }
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
        isBorder = Physics.Raycast(transform.position +Vector3.up, direction, 2, LayerMask.GetMask("Wall", "Map", "Water"));
        isBorder_x = Physics.Raycast(transform.position +Vector3.up, Vector3.right * direction.x, 2, LayerMask.GetMask("Wall", "Map", "Water"));
        isBorder_z = Physics.Raycast(transform.position +Vector3.up, Vector3.forward * direction.z, 2, LayerMask.GetMask("Wall", "Map", "Water"));
    }

    void p_Turn()
    {
        //키보드 회전
        //transform.LookAt(direction + transform.position);

        //마우스 회전
        switch(view_position)
        {
            case 1:
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
                break;
        }

    }

    private void LookAround()
    {
        switch (view_position)
        {
            case 0:
                Vector2 MouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                Vector3 camAngle = followcam.rotation.eulerAngles;
                float x = camAngle.x - MouseDelta.y;

                if (x < 180f)
                { x = Mathf.Clamp(x, -1f, 35f); }
                else
                { x = Mathf.Clamp(x, 335f, 361f); }

                followcam.rotation = Quaternion.Euler(x, camAngle.y + MouseDelta.x, camAngle.z);
                break;
            case 1:
                followcam.rotation = Quaternion.Euler(followcam.rotation.x, transform.eulerAngles.y, followcam.rotation.z);
                break;
        }
    

    }

    //카메라 뷰 바꾸기
    void Camera_view()
    {
        if ((isvDown))
        {
            view_position = (view_position == 0) ? 1 : 0;

            switch (view_position)
            {
                case 0:
                    AmingPoint.SetActive(true);

                    FollowCam_game.SetActive(true);
                    FollowCamera_game.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
                case 1:
                    AmingPoint.SetActive(false);

                    FollowCam_game.SetActive(false);
                    FollowCamera_game.SetActive(true);
                    Cursor.lockState = CursorLockMode.Confined;
                    break;
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
            gameObject.layer = 19;
            Invoke("Dodge_reset", 0.6f);
            EffectMusic.EffectPlay(3);
        }
    }
    void Dodge_reset(){
        invincibility = false;
        gameObject.layer = 6;

        speed = (float)STANDARD_p.STD_SPEED;
        canDodge = true;
        EffectMusic.EffectOut(3);

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
            switch (view_position) {
                case 0:
                    Vector3 direction_dummy = new Vector3(followcam.forward.x, 0f, followcam.forward.z).normalized + transform.position;
                    direction_dummy.y = transform.position.y;
                    transform.LookAt(direction_dummy);
                    break;
                case 1:
                    p_Turn();
                    break;

            }

            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon_script.Type.Short? "Swing" : equipWeapon.Ammo != 0? "Shot" : "none");

            EffectMusic.SwingOut(equipWeapon_Index);
            EffectMusic.SwingPlay(equipWeapon_Index);

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
            EffectMusic.EffectPlay(2);
            switch (view_position)
            {
                case 0:
                    Vector3 di_d = new Vector3(followcam.forward.x, 0f, followcam.forward.z).normalized;
                    Vector3 direction_dummy = di_d + transform.position;
                    direction_dummy.y = transform.position.y;
                    transform.LookAt(direction_dummy);
                    di_d.y = 1;
                    GameObject ig = Instantiate(Grenadeobject, transform.position + transform.forward*2, transform.rotation);
                    Rigidbody rG = ig.GetComponent<Rigidbody>();

                    if (isRun)
                        rG.AddForce(di_d * 5, ForceMode.Impulse);
                    else
                        rG.AddForce(di_d * 25, ForceMode.Impulse);

                    hasGrenades--;
                    break;

                case 1:
                    Ray ray = FollowCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit rayHit;
                    if (Physics.Raycast(ray, out rayHit, 100))
                    {
                        Vector3 nextVec = (rayHit.point - transform.position).normalized;
                        nextVec.y = 1;

                        GameObject instantgrenade = Instantiate(Grenadeobject, transform.position+ transform.forward * 2, transform.rotation);
                        Rigidbody rigidGranade = instantgrenade.GetComponent<Rigidbody>();
                        if (isRun)
                            rigidGranade.AddForce(nextVec * 5, ForceMode.Impulse);
                        else
                            rigidGranade.AddForce(nextVec * 25, ForceMode.Impulse);

                        hasGrenades--;
                        //Granade[hasGranades].setActive(false);
                    }
                    break;
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

        if ((isRdown || (isfDown && equipWeapon.Ammo == 0)) && !isDodge && !isSwap && isfReady && !isReload)
        {
            isReload = true;
            anim.SetTrigger("Reload");
            pAmmo += equipWeapon.Ammo;
            equipWeapon.Ammo = 0;
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

            int reloadammo = (pAmmo > equipWeapon.maxAmmo) ? equipWeapon.maxAmmo : pAmmo;
            equipWeapon.Ammo = reloadammo;
            pAmmo -= reloadammo;
        }
        isReload = false;
    }

    //무기 변경 문
    void Swap()
    {
        if (isGrab || !canDodge) return;
        //int WeaponIndex = -1;
        if (sDown1) WeaponIndex = 0;
        if (sDown2) WeaponIndex = 1;
        if (sDown3)
        {
            if (hasWeapons[2])
                WeaponIndex = 2;
            else if (hasWeapons[3])
                WeaponIndex = 3;
            else if (hasWeapons[4])
                WeaponIndex = 4;
            else if (hasWeapons[5])
                WeaponIndex = 5;
        }

        if (WeaponIndex == equipWeapon_Index && !Grab_Swap)
            return;
        if (WeaponIndex >= 0)
            if (!hasWeapons[WeaponIndex])
            {Grab_Swap = false; return; }

        if ((sDown1 || sDown2 || sDown3) && !isDodge || Grab_Swap)
        {
            if (Grab_Swap) Grab_Swap = false;
            if (equipWeapon != null)
            {
                if (equipWeapon_Index == 3)
                    equipWeapon.can_logging = true; 
                equipWeapon.isMotion = false; equipWeapon.Effect.enabled = false ; equipWeapon.gameObject.SetActive(false); 
            }
            
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
            if (nearObject.CompareTag("Weapon"))
            {
                if (isiDown)
                {
                    Item_Script item = nearObject.GetComponent<Item_Script>();
                    Gobj.GetComponent<GameManager>().GrabKey_i.SetActive(false);

                    int weaponIndex_d = item.value;
                    if (item.value == 1)
                        pAmmo += 7;

                    if (item.value == 2 || item.value == 3 || item.value == 4 || item.value == 5)
                    {
                        if (equipWeapon_Index == 2 || equipWeapon_Index == 3 || equipWeapon_Index == 4 || equipWeapon_Index == 5)
                        {
                            equipWeapon.gameObject.SetActive(false);
                            equipWeapon = null;
                            WeaponIndex = -1;
                            equipWeapon_Index = -1;
                        }

                        Vector3 ins_for = transform.position + this.transform.localRotation * Vector3.forward * 5;
                        if (hasWeapons[2])
                        {
                            GameObject dummy;
                            if (Curdun != null)
                                dummy = Curdun.GetComponent<Dungeon_Creator>().Instantiate_object(efsc.Weapon_item[1], ins_for + Vector3.up * 2);
                            else
                            {
                                dummy = Instantiate(efsc.Weapon_item[1]) as GameObject;
                                dummy.transform.position = ins_for + Vector3.up * 2;
                                dummy.transform.parent = GameObject.Find("StartMap_Package").transform;
                            }

                            dummy.GetComponent<Item_Script>().Durability = Weapons[2].GetComponent<Weapon_script>().Ammo;

                            hasWeapons[2] = false;
                        }
                        if (hasWeapons[3])
                        {
                            GameObject dummy;
                            if (Curdun != null)
                                dummy = Curdun.GetComponent<Dungeon_Creator>().Instantiate_object(efsc.Weapon_item[2], ins_for + Vector3.up * 3);
                            else
                            {
                                dummy = Instantiate(efsc.Weapon_item[2]) as GameObject;
                                dummy.transform.position = ins_for + Vector3.up * 3;
                                dummy.transform.parent = GameObject.Find("StartMap_Package").transform;
                            }
                            dummy.GetComponent<Item_Script>().Durability = Weapons[3].GetComponent<Weapon_script>().Ammo;
                            hasWeapons[3] = false;
                        }
                        if (hasWeapons[4])
                        {
                            GameObject dummy;
                            if (Curdun != null)
                                dummy = Curdun.GetComponent<Dungeon_Creator>().Instantiate_object(efsc.Weapon_item[3], ins_for + Vector3.up * 8);
                            else
                            {
                                dummy = Instantiate(efsc.Weapon_item[3]) as GameObject;
                                dummy.transform.position = ins_for + Vector3.up * 8;
                                dummy.transform.parent = GameObject.Find("StartMap_Package").transform;
                            }
                            dummy.GetComponent<Item_Script>().Durability = Weapons[4].GetComponent<Weapon_script>().Ammo;

                            hasWeapons[4] = false;
                        }
                        if (hasWeapons[5])
                        {
                            GameObject dummy;
                            if (Curdun != null)
                                dummy = Curdun.GetComponent<Dungeon_Creator>().Instantiate_object(efsc.Weapon_item[4], ins_for + Vector3.up * 10);
                            else
                            {
                                dummy = Instantiate(efsc.Weapon_item[4]) as GameObject;
                                dummy.transform.position = ins_for + Vector3.up * 10;
                                dummy.transform.parent = GameObject.Find("StartMap_Package").transform;
                            }
                            dummy.GetComponent<Item_Script>().Durability = Weapons[5].GetComponent<Weapon_script>().Ammo;

                            hasWeapons[5] = false;
                        }
                    }
                    hasWeapons[weaponIndex_d] = true;

                    if (weaponIndex_d != 1)
                        Weapons[weaponIndex_d].GetComponent<Weapon_script>().Ammo = nearObject.GetComponent<Item_Script>().Durability;  /////////////////////////////////////

                    if (equipWeapon_Index == -1 && item.value != 1)
                    {
                        Grab_Swap = true;
                        if (hasWeapons[2]) WeaponIndex = 2;
                        else if (hasWeapons[3]) WeaponIndex = 3;
                        else if (hasWeapons[4]) WeaponIndex = 4;
                        else if (hasWeapons[5]) WeaponIndex = 5;
                    }

                    //아이템 이미지 띄우기
                    if (item.value == 1)
                        Gobj.GetComponent<GameManager>().ItemGet(3);
                    else
                    {
                        if (item.value == 5 || item.value == 4)
                            Gobj.GetComponent<GameManager>().ItemGet(7);
                        else
                            Gobj.GetComponent<GameManager>().ItemGet(item.value + 3);
                    }


                    if (item.value != 1)
                    {
                        if (item.value >= 4)
                            Gobj.GetComponent<GameManager>().WeaponIm(2);
                        else
                            Gobj.GetComponent<GameManager>().WeaponIm(item.value-2);
                    }

                    EffectMusic.ItemPlay(Random.Range(0, 2) + 4); 

                    Destroy(nearObject);
                }
            }
            else if (nearObject.CompareTag("Can_Grab") && isiDown && !isGrab)
            {
                
                isGrab = true;
                grabObject = nearObject;
                nearObject = null;
                if (equipWeapon != null)
                    equipWeapon.gameObject.SetActive(false);
                equipWeapon = null;
                grab_dummy_weapon = WeaponIndex;
                Grab_t = grabObject.GetComponentInParent<Rigidbody>();
                Grab_b = grabObject.GetComponentInParent<CapsuleCollider>();

                Grab_radius = Grab_b.radius;

                if (grabObject.transform.parent.CompareTag("Enemy") && grabObject.transform.parent.gameObject.GetComponent<EnemyState>().type == Type.M)
                    GrabPosition.Translate(new Vector3(0, 0, -Grab_radius - 0.5f + 3f));
                else
                { GrabPosition.Translate(new Vector3(0, 0, -Grab_radius - 0.5f)); }
                WeaponIndex = -1;
                equipWeapon_Index = -1;
                Gobj.GetComponent<GameManager>().GrabKey.SetActive(true);
                Gobj.GetComponent<GameManager>().GrabKey_i.SetActive(false); 

            }
        }
    }

    void Grab()
    {
        if (isGrab && grabObject == null) { isGrab = false; WeaponIndex = grab_dummy_weapon; if (grab_dummy_weapon != -1) { Grab_Swap = true; }
            GrabPosition.Translate(new Vector3(0, 0, +Grab_radius + 0.5f));
            StopCoroutine(Throw);
            Gobj.GetComponent<GameManager>().GrabKey.SetActive(false);
        }
        else if (isGrab)
        {
            grabObject.transform.tag = "Untagged";
            StopCoroutine("Reload_time");
            if (grabObject == null)
            { isGrab = false; return; }

            //폭탄이면서 만약 터졌다면 내려놓거라
            if (grabObject.transform.parent.gameObject.CompareTag("Item"))
            { if (grabObject.GetComponentInParent<Grenade_script>().isTime) { Gobj.GetComponent<GameManager>().GrabKey.SetActive(false); grabObject = null; return; } }

            if (!grabObject.transform.parent.CompareTag("Trap_dam"))
                Grab_t.position = GrabPosition.position;
            else if (grabObject.transform.parent.CompareTag("Shield"))
                Grab_t.position = GrabPosition.position + Vector3.up * 5;
            else
            {
                Grab_t.position = GrabPosition2.position;
            }

            Grab_t.velocity = Vector3.zero;
            Grab_t.angularVelocity = Vector3.zero;
            if (grabObject.transform.parent.CompareTag("Trap_dam") || grabObject.transform.parent.CompareTag("Shield") )
            {
                Grab_t.rotation = Quaternion.FromToRotation(transform.up, transform.forward);
            }
            else if (direction != Vector3.zero)
                Grab_t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.3f);

            if (Input.GetButtonUp("Fire1") && canDodge && !isStun && !isThrow)
            {
                isThrow = true;
                Throw = Throw_Material();
                StartCoroutine(Throw);
            }

        }
        else
        {
            //grabObject = null;
        }

    }
    IEnumerator Throw_Material()
    {
        Gobj.GetComponent<GameManager>().GrabKey.SetActive(false);
        anim.SetTrigger("Throw");
        yield return new WaitForSeconds(0.01f);


        switch (view_position)
        {
            case 0:
                Vector3 di_d = new Vector3(followcam.forward.x, 0.1f, followcam.forward.z).normalized;
                Vector3 direction_dummy = di_d + transform.position;
                direction_dummy.y = transform.position.y;
                transform.LookAt(direction_dummy);
                yield return null;
                Grab_t.position = GrabPosition.position;
                if (isRun)
                    Grab_t.AddForce(di_d * 10, ForceMode.Impulse);
                else
                    Grab_t.AddForce(di_d * 70, ForceMode.Impulse);
                break;
            case 1:
                Ray ray = FollowCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit rayHit;
                if (Physics.Raycast(ray, out rayHit, 100))
                {
                    Vector3 nextVec = rayHit.point - transform.position;
                    nextVec.y = 1;
                    Grab_t.position = GrabPosition.position;
                    if (isRun)
                        Grab_t.AddForce(nextVec.normalized * 10, ForceMode.Impulse);
                    else
                        Grab_t.AddForce(nextVec.normalized * 70, ForceMode.Impulse);
                }   
            break;
        }
        EffectMusic.EffectPlay(4);
        //yield return new WaitForSeconds(0.1f);
        grabObject.transform.tag = "Can_Grab";
        if (grabObject.transform.parent.CompareTag( "Enemy" )&& grabObject.transform.parent.gameObject.GetComponent<EnemyState>().type == Type.M)
            GrabPosition.Translate(new Vector3(0, 0, +Grab_radius + 0.5f -3f));
        else
            GrabPosition.Translate(new Vector3(0, 0, +Grab_radius + 0.5f));

        grabObject = null; isGrab = false;
        WeaponIndex = grab_dummy_weapon;
        yield return new WaitForSeconds(0.5f);
        if (grab_dummy_weapon != -1)
            Grab_Swap = true;
        yield return null;
        isThrow = false;
    }
    

	//플레이어 주변 아이템 탐색 문
	void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("Item"))
        {
            Item_Script item = other.GetComponent<Item_Script>();
            switch (item.Type_I)
            {
                case Item_Script.Type.Ammo:
                    pAmmo += item.value;
                    if (pAmmo > MaxpAmmo)
                        pAmmo = MaxpAmmo;
                    EffectMusic.ItemPlay(Random.Range(0, 2)+4);
                    Gobj.GetComponent<GameManager>().ItemGet(2);
                    break;
                case Item_Script.Type.Boom:
                    hasGrenades += item.value;
                    if (hasGrenades > MaxpGrenades)
                        hasGrenades = MaxpGrenades;
                    EffectMusic.ItemPlay(Random.Range(0, 2) + 4);
                    Gobj.GetComponent<GameManager>().ItemGet(1);
                    break;
                case Item_Script.Type.heart:
                    pHealth += item.value;
                    if (pHealth > maxpHealth)
                        pHealth = maxpHealth;
                    EffectMusic.ItemPlay(Random.Range(0, 2) + 4);
                    Gobj.GetComponent<GameManager>().ItemGet(0);
                    break;
                case Item_Script.Type.ReinforceB:
                    EffectMusic.ItemPlay(Random.Range(0, 2) + 4);
                    isRein = true;
                    Gobj.GetComponent<GameManager>().ItemGet(4);
                    break;
                case Item_Script.Type.Coin:
                    Money += item.value;
                    EffectMusic.ItemPlay(Random.Range(0,4));
                    Gobj.GetComponent<GameManager>().CoinGetTxt(item.value);
                    break;
            }
            Destroy(other.gameObject);
        }
        else if (other.CompareTag( "EnemyBullet"))
        {
            if (!invincibility)
            {
                invincibility = true;
                //Vector3 reactvec = other.gameObject.transform.position - transform.position;
                Vector3 reactvec = transform.position - other.gameObject.transform.position;
                reactvec.y = 0.5f;
                Bullet_script enemyBullet = other.GetComponent<Bullet_script>();
                pHealth -= enemyBullet.damage;

                Damage = OnDamage(reactvec.normalized, false);
                StartCoroutine(Damage);
            }
            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);
        }
        else if (other.CompareTag("EnemyBullet_Renforcement"))
        {
            if (!invincibility)
            {
                invincibility = true;
                Vector3 reactvec = transform.position - other.gameObject.transform.position;
                reactvec.y = 0.5f;
                Bullet_script enemyBullet = other.GetComponent<Bullet_script>();
                pHealth -= enemyBullet.damage;

                Damage = OnDamage(reactvec.normalized, true);
                StartCoroutine(Damage);
            }
        }
        else if (other.CompareTag("Trap_dam"))
        {
            if (!invincibility)
            {
                Trap_range Trap_r = other.gameObject.GetComponent<Trap_range>();
                if (Trap_r.dotDam)
                    return;
                invincibility = true;
                Vector3 reactvec = transform.position - other.gameObject.transform.position;
                reactvec.y = 0.5f;
                pHealth -= Trap_r.Damage;

                Damage = OnDamage(reactvec.normalized, Trap_r.isNuck);
                StartCoroutine(Damage);
            }
        }
        else if (other.CompareTag("Portal"))
        {

            Gobj.GetComponent<GameManager>().PortalKey.SetActive(true);
            isPortal = true;
        }
    }


	public void HitByGrenade(Vector3 vec)
    {
        if (!invincibility)
        {
            pHealth -= 5;
            Damage = OnDamage(vec.normalized,true);
            StartCoroutine(Damage);
        }
    }

    IEnumerator OnDamage(Vector3 react, bool isStiffen)
    {
        invincibility = true;
        isDam = true;
        anim.SetBool("Run",false);
        EffectMusic.EffectPlay(5);
        if (pHealth > 0)
        {
            if (isStiffen)
            {
                isStun = true;
                anim.SetTrigger("Stun");
                player_r.AddForce(react * 20, ForceMode.Impulse);
                if (grabObject != null)
                {
                    grabObject.transform.tag = "Can_Grab";
                    if (grabObject.transform.parent.CompareTag("Enemy") && grabObject.transform.parent.gameObject.GetComponent<EnemyState>().type == Type.M)
                        GrabPosition.Translate(new Vector3(0, 0, +Grab_radius + 0.5f - 3f));
                    else
                        GrabPosition.Translate(new Vector3(0, 0, +Grab_radius + 0.5f));

                    grabObject = null;
                    isGrab = false;
                }
            }
            else
            {
                isStun = true;
                player_r.AddForce(react * 8, ForceMode.Impulse);
            }
            yield return new WaitForSeconds(0.5f);
            if (!isStiffen) isStun = false;

            yield return new WaitForSeconds(0.78f);
            if (isStiffen) anim.speed = 0.0f;

            yield return new WaitForSeconds(0.4f);
            if (isStiffen) anim.speed = 1.0f;
            isDam = false;
            if (isStiffen) isStun = false;
            yield return new WaitForSeconds(0.8f);

            invincibility = false;
        }
        
    }

    void OnDie()
    {
        anim.SetTrigger("Die");
        transform.tag = "Untagged";
        transform.gameObject.layer = 19;
        isStop = true;
        Gobj.GetComponent<GameManager>().GameOver();
    }


	void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Weapon") || other.CompareTag("Item") || other.CompareTag("Can_Grab"))
        {
            if (other.CompareTag("Weapon"))
            {
                Gobj.GetComponent<GameManager>().GrabKey_i.SetActive(true);
            }


            if (other.CompareTag("Can_Grab"))
            {
                if (grabObject == null)
                    Gobj.GetComponent<GameManager>().GrabKey_i.SetActive(true); 
            }
            nearObject = other.gameObject;
        }
        else if (other.CompareTag("Trap_dam")) 
        {
            Trap_range dummy = other.gameObject.GetComponent<Trap_range>();
            if (dummy.dotDam)
            {
                isBurn = true; isBurnOff = false;
                StopCoroutine(Burn);
                dot_dam += Time.deltaTime;
            }
            else if (!invincibility)
            {
                invincibility = true;
                Vector3 reactvec = transform.position - other.gameObject.transform.position;
                reactvec.y = 0.5f;
                pHealth -= dummy.Damage;

                Damage = OnDamage(reactvec.normalized, dummy.isNuck);
                StartCoroutine(Damage);
            }

        }
        
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weapon") || other.CompareTag("Item") || other.CompareTag("Can_Grab"))
        {
             Gobj.GetComponent<GameManager>().GrabKey_i.SetActive(false); 
            nearObject = null;
        }
        else if (other.CompareTag("Portal"))
        {

            Gobj.GetComponent<GameManager>().PortalKey.SetActive(false);
            Gobj.GetComponent<GameManager>().GrabKey.SetActive(false);
            Gobj.GetComponent<GameManager>().GrabKey_i.SetActive(false);

            isPortal = false;
        }
    }

    IEnumerator Damage_color()
    {
        if (isDam)
        {
            yield return new WaitForSeconds(0.05f);
            foreach (MeshRenderer mesh in meshs)
            { mesh.material.color = Color.gray; }
            yield return new WaitForSeconds(0.05f);
            foreach (MeshRenderer mesh in meshs)
            { mesh.material.color = Color.white; }
            isDamS = false;
        }
    }

    void Burn_dam()
    {
        if (isBurn)
        {
            dot_dam += Time.deltaTime;
            foreach (MeshRenderer mesh in meshs)
            { mesh.material.color = new Color(255 / 255f, 120 / 255f, 100 / 255f); }
            if (dot_dam >= 2f)
            {
                efsc.Select_effect(transform.position, 4);
                pHealth -= 1;
                dot_dam = 0;
            }
        }
        else if (!isBurn && dot_dam != 0)
        {
            dot_dam -= Time.deltaTime;
            if (dot_dam <= 0f)
                dot_dam = 0;
        }

        if (!isBurnOff && isBurn)
        { Burn = Burn_Off(); StartCoroutine(Burn); }
    }
    IEnumerator Burn_Off()
    {
        isBurnOff = true;
        yield return new WaitForSeconds(3f);
        isBurn = false;
        isBurnOff = false;
        foreach (MeshRenderer mesh in meshs)
        { mesh.material.color = Color.white; }
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
