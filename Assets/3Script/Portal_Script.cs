using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal_Script : MonoBehaviour
{
    private GameObject gobj, oobj, DungeonCreator;
	private int Stage;
	private GameObject curDunCreater;
	private GameObject Player;


	void Start()
	{
		gobj = GameObject.Find("GameManager");
		oobj = GameObject.Find("Object_Manager");

		DungeonCreator = oobj.GetComponent<Effect_script>().Dungeon_Creator;
		Player = GameObject.FindWithTag("Player");
	}

	public IEnumerator GotoNext()
	{
		yield return StartCoroutine(gobj.GetComponent<GameManager>().LoadingStart());
		Player = GameObject.FindWithTag("Player");
		Player.transform.position = new Vector3(0,100,0);

		//클리어
		if (gobj.GetComponent<GameManager>().stage == 13)
		{
			Destroy(curDunCreater);
			Player.GetComponent<Rigidbody>().isKinematic = true;
			Player.GetComponent<Rigidbody>().useGravity = false;
			Player.GetComponent<player_script>().isStop = true;
			gobj.GetComponent<GameManager>().GameClear();
		}
		else
		{
			if (gobj.GetComponent<GameManager>().stage == 0)
			{

				Destroy(GameObject.Find("StartMap_Package"));

				curDunCreater = Instantiate(DungeonCreator) as GameObject;
				curDunCreater.transform.position = gobj.GetComponent<GameManager>().dunSpawnPoint();
				Player.GetComponent<player_script>().Curdun = curDunCreater;

				//생성 후 몇 초 뒤 이동. 그러므로 이 기간동안 블랙스크린.
				yield return StartCoroutine(Player_Teleport());
			}
			else
			{
				Destroy(curDunCreater);

				curDunCreater = Instantiate(DungeonCreator) as GameObject;
				curDunCreater.transform.position = gobj.GetComponent<GameManager>().dunSpawnPoint();
				gobj.GetComponent<GameManager>().GrabKey.SetActive(false);
				gobj.GetComponent<GameManager>().GrabKey_i.SetActive(false);
				gobj.GetComponent<GameManager>().PortalKey.SetActive(false);


				Player.GetComponent<player_script>().Curdun = curDunCreater;
				yield return StartCoroutine(Player_Teleport());
			}
			StartCoroutine(gobj.GetComponent<GameManager>().LoadingOut());
		}
	}


	IEnumerator Player_Teleport()
	{
		yield return new WaitForSeconds(0.5f);
		//Vector3 spawn_point = curDunCreater.transform.FindChild("Player_Spawn").position;
		Vector3 spawn_point = curDunCreater.GetComponent<Dungeon_Creator>().Player_spawn.position;
		
		Player.transform.position = spawn_point;
		Player.GetComponent<player_script>().isTeleport = false;
		gobj.GetComponent<GameManager>().stage++;
	}
	

}
