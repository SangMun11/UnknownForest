using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//시간초에 따라 빨강 왔다갔다 구현
public class Grenade_script : MonoBehaviour
{
    Animator anim;
    public Rigidbody rigid;
    public float timer;
    public GameObject GrabRange;
    public bool isTime, isStart, isColorStart;
    //private MeshRenderer[] meshs;
    private SkinnedMeshRenderer mesh;
    private GameObject obj;
    public GameObject Effectmusic;
    public GameObject clockSound;
    private AudioSource dummy= null;
    void Start()
	{
        obj = GameObject.Find("Object_Manager");
        anim = GetComponentInChildren<Animator>();
        //meshs = GetComponentsInChildren<MeshRenderer>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
	}
	private void Update()
	{
        timer += Time.deltaTime;
        if (timer >= 3f && !isStart)
        { isStart = true; StartCoroutine("Explosion"); }

        if (timer >= 1.5f && !isColorStart&& !isTime)
        { isColorStart = true; StartCoroutine(ColorChange()); }

        rigid.angularVelocity = Vector3.zero;
        if (isTime)
            rigid.velocity = Vector3.zero;
    }

	IEnumerator Explosion()
    {
        anim.SetTrigger("attack01");
        Effectmusic.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.8f);
        
        rigid.velocity = Vector3.zero;
        rigid.isKinematic = true;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        dummy.Stop();
        isTime = true;
        GrabRange.SetActive(false);
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 10, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        RaycastHit[] rayHitsDes = Physics.SphereCastAll(transform.position, 10, Vector3.up, 0f, LayerMask.GetMask("Wall"));
        RaycastHit[] rayHitsplayer = Physics.SphereCastAll(transform.position, 10, Vector3.up, 0f, LayerMask.GetMask("Player"));
        RaycastHit[] rayhitsBomb = Physics.SphereCastAll(transform.position, 10, Vector3.up, 0f, LayerMask.GetMask("Can_Collision_Possible"));

        foreach (RaycastHit hitObject in rayHits)
        {hitObject.transform.GetComponent<Enemy_script>().HitByGrenade(transform.position); hitObject.transform.GetComponent<Enemy_script>().isBurn = true; }
        foreach (RaycastHit hitObject in rayHitsplayer)
        {
            Vector3 a =hitObject.transform.position - transform.position;
            hitObject.transform.GetComponent<player_script>().HitByGrenade(a);
            //hitObject.transform.GetComponent<Rigidbody>().AddForce(a.normalized * 20, ForceMode.Impulse);
        }

        foreach (RaycastHit hitObject in rayHitsDes)
        {
            
            obj.GetComponent<Debris_Create>().Breaking(hitObject.transform, 1000, 1);
            Destroy(hitObject.transform.gameObject);
        }

        foreach (RaycastHit hitBomb in rayhitsBomb)
        { if (hitBomb.transform.gameObject.CompareTag("Bomb")) hitBomb.transform.GetComponent<Grenade_script>().timer += 3f; }

        Destroy(gameObject, 1.5f);
    }

    IEnumerator ColorChange()
    {
        if (dummy != null)
            dummy.Stop();
        dummy = clockSound.GetComponent<AudioSource>();
        dummy.Play();
        mesh.materials[0].color = Color.red;
        yield return new WaitForSeconds(0.1f);
        transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
        yield return new WaitForSeconds(0.1f);
        transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
        yield return new WaitForSeconds(0.1f);
        transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
        yield return new WaitForSeconds(0.2f);
        transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
        //foreach (MeshRenderer mesh in meshs)
        //{ mesh.material.color = Color.white; }
        isColorStart = false;
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Close_Weapon")
		{
			Weapon_script weapon = other.GetComponent<Weapon_script>();
			Vector3 reactVec = transform.position - other.transform.position;

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 5, ForceMode.Impulse);
        }
	}
}
