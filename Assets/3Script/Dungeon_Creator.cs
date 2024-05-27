using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dungeon_Creator : MonoBehaviour
{
    public GameObject Dungeon_Maps_default;
    public GameObject[] Dungeon_Maps;
    private Vector3 Creator_spawn;
    private GameObject[] Map_Candidate;
    private GameObject gobj;
    //private int[,] Map_Create_History;
    private GameObject[,] Map_Create_History;
    private int Select_Num = 0, Num_Repetition, list_length;
    private int[,,] is_open;
    //private int[] Exclude_Map;
    private bool Can_Crea = true, Hole_;
    [SerializeField]
    private NavMeshSurface nav_d;
    [SerializeField]
    private GameObject[] Entrance_Exit, Dungeon_Wall;
    [SerializeField]
    private GameObject[] Traps, Enemies, Items, Gold;
    private int[] dup = new int[5];

    
    public int Enemy_Count, Trap_Count, Crate_Count;

    private This_Map_T Map_Tag, Dummy_t;

    private static GameObject Selected_Map;
    private GameObject dummy;
    private Effect_script efsc;
    private int Random_Height;
    private int StartWhere, FinishWhere, Stage;
    int[] ij = new int[2];

    public Transform Player_spawn;
    public GameObject[] settingmap, middleBossMap;
    public GameObject BossMap;

    void Awake()
	{
        efsc = GameObject.Find("Object_Manager").GetComponent<Effect_script>();
        gobj = GameObject.Find("GameManager");
        Map_Candidate = new GameObject[Dungeon_Maps.Length];
        //Exclude_Map = new int[Dungeon_Maps.Length];
        Creator_spawn = gobj.GetComponent<GameManager>().dunSpawn;

        Random_Height = Random.Range(4, 6);
        Map_Create_History = new GameObject[Random_Height, 4];
        is_open = new int[Random_Height,4,2];
        nav_d = GameObject.Find("Surface_Map_standard").GetComponent<NavMeshSurface>();
        Traps = efsc.Traps; Enemies = efsc.Enemies; Items = efsc.ItemCrate; Gold = efsc.Gold;
        Stage = gobj.GetComponent<GameManager>().stage+1;
        for (int i = 0; i < 5; i++)
            dup[i] = -1;
    }

	void Start()
    {
        if (Stage == 13)
        {
            settingmap[0].SetActive(false); settingmap[1].SetActive(false);
            Selected_Map = GameObject.Instantiate(BossMap) as GameObject;
            Selected_Map.transform.position = Creator_spawn;
            Selected_Map.transform.parent = transform;
            Player_spawn = Selected_Map.transform.Find("Player_Spawn");
            nav_d.BuildNavMesh();

        }
        else if (Stage == 3 || Stage == 7 || Stage == 10)
        {
            settingmap[0].SetActive(false); settingmap[1].SetActive(false);
            Selected_Map = GameObject.Instantiate(middleBossMap[Random.Range(0, 3)]) as GameObject;
            Selected_Map.transform.position = Creator_spawn;
            Selected_Map.transform.parent = transform;
            Player_spawn = Selected_Map.transform.Find("Player_Spawn");
        }
        else
        {

            Create_Way();

            Creator();
        }
    }

    void Create_Way()
    {
        bool is_finish = false , isup = true;
        //int start_num = Random.Range(0,4);

        ij[0] = 0; ij[1] = Random.Range(1, 3);  //ij�� 0 ���Ʒ�, 1 �¿�
        
        StartWhere = ij[1];

        while (true)
        {
            int ran = Random.Range(0,2);


            //ó������ �ö󰡰ų�, ó�������϶�.
            if (isup)
            {

                    is_open[ij[0], ij[1], 0] = (int)STANDARD.South + 1;

                switch (ran)
                {
                    //0�� ����
                    case 0:
                        if (ij[1] == 0) //���� �����ڸ��� �� �������� ���ٸ� �ٷ� �ö󰡱� �� �׷� ���������� ����
                        {
                            is_open[ij[0], ij[1], 1] = (int)STANDARD.East + 1;
                            ij[1]++;
                            is_open[ij[0], ij[1], 0] = (int)STANDARD.West + 1;
                            break;
                        }
                        else
                        {
                            is_open[ij[0], ij[1], 1] = (int)STANDARD.West + 1;
                            ij[1]--;
                            is_open[ij[0], ij[1], 0] = (int)STANDARD.East + 1;
                            break;
                        }

                    //1�� ������
                    case 1:
                        if (ij[1] == 3)
                        {
                            is_open[ij[0], ij[1], 1] = (int)STANDARD.West + 1;
                            ij[1]--;
                            is_open[ij[0], ij[1], 0] = (int)STANDARD.East + 1;

                            break;
                        }
                        else
                        {
                            is_open[ij[0], ij[1], 1] = (int)STANDARD.East + 1;
                            ij[1]++;
                            is_open[ij[0], ij[1], 0] = (int)STANDARD.West + 1;

                            break;
                        }

                }
                isup = false;
            }
            else
            {
                switch (ran)
                {
                    //0�� �¿�
                    case 0:
                        //������ ���� = �·� ������.
                        if (is_open[ij[0], ij[1], 0] == (int)STANDARD.East + 1)
                        {
                            //���� ������ �ִٸ�
                            if (ij[1] == 0)
                            {
                                isup = true;
                                is_open[ij[0], ij[1], 1] = (int)STANDARD.North + 1;
                                if (ij[0] != Random_Height - 1)
                                {
                                    ij[0]++;
                                    is_open[ij[0], ij[1], 0] = (int)STANDARD.South + 1;
                                }
                                else { is_finish = true; FinishWhere = ij[1]; }
                            }
                            else
                            {
                                is_open[ij[0], ij[1], 1] = (int)STANDARD.West + 1;
                                ij[1]--;
                                is_open[ij[0], ij[1], 0] = (int)STANDARD.East + 1;
                            }
                        }
                        else //������ ���� = ��� ������.
                        {
                            //���� ������ �ִٸ�
                            if (ij[1] == 3)
                            {
                                isup = true;
                                is_open[ij[0], ij[1], 1] = (int)STANDARD.North + 1;
                                if (ij[0] != Random_Height - 1)
                                {
                                    ij[0]++;
                                    is_open[ij[0], ij[1], 0] = (int)STANDARD.South + 1;
                                }
                                else { is_finish = true; FinishWhere = ij[1]; }
                            }
                            else
                            {
                                is_open[ij[0], ij[1], 1] = (int)STANDARD.East + 1;
                                ij[1]++;
                                is_open[ij[0], ij[1], 0] = (int)STANDARD.West + 1;
                            }
                        }
                        break;

                    //1�� �Ʒ���
                    case 1:
                        is_open[ij[0], ij[1], 1] = (int)STANDARD.North + 1;
                        if (ij[0] != Random_Height - 1)
                        {
                            isup = true;
                            ij[0]++;
                            is_open[ij[0], ij[1], 0] = (int)STANDARD.South + 1;
                        }
                        else {is_finish = true; FinishWhere = ij[1]; }
                        break;

                }

            }

            //Debug.Log(ij[0] + " " + ij[1] + " " + is_open[ij[0], ij[1], 0] + " " + is_open[ij[0], ij[1], 1]);
            if (is_finish)
                break;
        }
    }
    void Creator()
    {
        bool has_open;
        //i�� z��
        for (int i = 0; i < Random_Height; i++)
        {
            //j�� x��
            for (int j = 0; j < 4; j++)
            {
                Num_Repetition = 0;
                Map_Candidate = null;
                Map_Candidate = new GameObject[Dungeon_Maps.Length];

                has_open = false;
                for (int l = 0; l < 2; l++)
                {
                    if (is_open[i, j, l] != 0)
                    { has_open = true; }
                }
                for (int k = 0; k < Dungeon_Maps.Length; k++)
                {
                    Hole_ = false;
                    Can_Crea = true;
                    Map_Tag = Dungeon_Maps[k].GetComponent<This_Map_T>();


                    if (has_open)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            if (is_open[i, j, l] != 0)
                            {
                                if (!Map_Tag.Is_Open[is_open[i, j, l] - 1])
                                    Can_Crea = false;

                            }
                        }
                    }

                    if (Map_Tag.Stage > Stage)
                        Can_Crea = false;


                    //ù ������ canstart
                    if (((i == 0 && j == StartWhere)|| (i == Random_Height - 1 && j == FinishWhere)) && Can_Crea)
                    {
                        if (!Map_Tag.Can_Start)
                        {
                            Can_Crea = false; 
                        }
                    }




                    //������ �ʰ� Ȯ��
                    if (i != 0)
                    {
                        //Dummy_t = Dungeon_Maps[Map_Create_History[i - 1, j]].GetComponent<This_Map_T>();
                        Dummy_t = Map_Create_History[i - 1, j].GetComponent<This_Map_T>();

                        if (Dummy_t.Where.Equals(This_Map_T.Exist_Hole_Direction.North))
                        {

                            if (!Map_Tag.Where.Equals(This_Map_T.Exist_Hole_Direction.South))
                                Can_Crea = false;
                            else
                                Hole_ = true;
                        }
                        else
                        {
                            if (Map_Tag.Where.Equals(This_Map_T.Exist_Hole_Direction.South))
                                Can_Crea = false;

                            //i-1 �� ���� ������ �ִ���?
                            if (Dummy_t.Exist_Hole && Map_Tag.Exist_Hole)
                            { Can_Crea = false; }
                        }

                        if (j != 3 && Can_Crea)
                        {
                            This_Map_T Dummy_t2 = Map_Create_History[i - 1, j + 1].GetComponent<This_Map_T>();
                            if (Dummy_t2.Where.Equals(This_Map_T.Exist_Hole_Direction.North))
                            {
                                if (Map_Tag.Where.Equals(This_Map_T.Exist_Hole_Direction.East))
                                    Can_Crea = false;
                            }
                        }


                        //i-1 �� �ʰ� ������ �ߺ��� ��������?
                        if (Map_Create_History[i - 1, j] == Dungeon_Maps[k] && !Map_Tag.CanDuplicate && Can_Crea)
                        { Can_Crea = false; }

                        //i-1 �� ���� ����� �����ϸ�, ���� ���� ����� ��������?
                        if ((Dummy_t.Is_Open[(int)STANDARD.North] && !Map_Tag.Is_Open[(int)STANDARD.South] && Can_Crea && has_open))
                        { Can_Crea = false; }
                        //i-1 �� �ʰ� ���� ���� ���� ���� 3 ��������?
                        if ((Mathf.Abs(Dummy_t.Hole0[0] - Map_Tag.Hole2[0]) > 6 && Can_Crea) || (Mathf.Abs(Dummy_t.Hole0[1] - Map_Tag.Hole2[1]) > 6 && Can_Crea))
                        { Can_Crea = false; }
                    }
                    else if (Can_Crea)
                    {
                        if ( Map_Tag.Where.Equals(This_Map_T.Exist_Hole_Direction.South))
                        {
                            Can_Crea = false;
                        }
                    }



                    if (i == Random_Height-1 && Can_Crea)
                    {
                        if (Map_Tag.Where.Equals(This_Map_T.Exist_Hole_Direction.North))
                        {
                            Can_Crea = false;
                        }
                    }

                    if (j == 3 && Can_Crea)
                    {
                        if (Map_Tag.Where.Equals(This_Map_T.Exist_Hole_Direction.East))
                        {
                            Can_Crea = false;
                        }
                    }


                    //������ �ʰ� Ȯ��
                    if (j != 0)
                    {
                        Dummy_t = Map_Create_History[i, j - 1].GetComponent<This_Map_T>();

                        //�����ʿ� �̾����� ������ �ִ���
                        if (Dummy_t.Where.Equals(This_Map_T.Exist_Hole_Direction.East))
                        {
                            if (!Map_Tag.Where.Equals(This_Map_T.Exist_Hole_Direction.West))
                                Can_Crea = false;
                            else
                                Hole_ = true;
                        }
                        else
                        {
                            if (Map_Tag.Where.Equals(This_Map_T.Exist_Hole_Direction.West))
                                Can_Crea = false;
                            //j-1 �� ���� ������ �ִ���?
                            if (Dummy_t.Exist_Hole && Map_Tag.Exist_Hole)
                            { Can_Crea = false; }
                        }



                        //j-1 �� �ʰ� ������ �ߺ��� ��������?
                        if (Map_Create_History[i, j - 1] == Dungeon_Maps[k] && !Map_Tag.CanDuplicate && Can_Crea)
                        { Can_Crea = false; }
                        //j-1 �� ���� ����� �����ϸ�, ���� ���� ����� ��������?
                        if ((!Dummy_t.Is_Open[(int)STANDARD.East] && Map_Tag.Is_Open[(int)STANDARD.West] && Can_Crea && has_open) ||
                            (Dummy_t.Is_Open[(int)STANDARD.East] && !Map_Tag.Is_Open[(int)STANDARD.West] && Can_Crea && has_open))
                        { Can_Crea = false; }
                        //j-1 �� �ʰ� ���� ���� ���� ���� 3 ��������?
                        if ((Mathf.Abs(Dummy_t.Hole1[0] - Map_Tag.Hole3[0]) > 4 && Can_Crea) || (Mathf.Abs(Dummy_t.Hole1[1] - Map_Tag.Hole3[1]) > 4 && Can_Crea))
                        { Can_Crea = false; }
                    }
                    else if (Can_Crea)
                    {
                        if (Map_Tag.Where.Equals(This_Map_T.Exist_Hole_Direction.West))
                        {
                            Can_Crea = false;
                        }
                    }



                    if (Can_Crea || Hole_)
                    {
                        Map_Candidate[Num_Repetition++] = Dungeon_Maps[k];
                    }
                }

                Select_Num = Random.Range(0,Num_Repetition);

                

                if (Num_Repetition != 0)
                    Selected_Map = GameObject.Instantiate(Map_Candidate[Select_Num]) as GameObject;
                else if (Num_Repetition == 0)
                    Selected_Map = GameObject.Instantiate(Dungeon_Maps_default) as GameObject;


                //Selected_Map = GameObject.Instantiate(Map_Candidate[0]) as GameObject;
                Map_Tag = Selected_Map.GetComponent<This_Map_T>();
                Selected_Map.transform.position = Creator_spawn + new Vector3((int)STANDARD.STD_INTERVAL * j, 0, (int)STANDARD.STD_INTERVAL * i);

                /*
                //����׿� ��ã��
                if (has_open)
                {
                    Selected_Map.transform.position = Selected_Map.transform.position + new Vector3(0, 50, 0);
                    Debug.Log(i +","+ j+ "= "+is_open[i,j,0] +"   " +is_open[i,j,1]);
                }
                */
                Selected_Map.transform.parent = gameObject.transform;
                //Map_Create_History[i, j] = Select_Num;
                

                if (Num_Repetition != 0)
                    Map_Create_History[i, j] = Map_Candidate[Select_Num];
                else if (Num_Repetition == 0)
                    Map_Create_History[i, j] = Dungeon_Maps_default;
                
                //�� ��ȯ
                list_length = Map_Tag.Mob_spawn.Length;
                for (int len = 0; len < list_length; len++)
                {
                    if (Random.Range(0, 10) > 6)
                    {
                        GameObject dummy_game;
                        while (true)
                        {
                            int Num_enemy = efsc.Enemy_Select();
                            dummy_game = Enemies[Num_enemy];
                            if (dummy_game.GetComponent<EnemyState>().Stage < Stage)
                                break;
                        }

                        //dummy = GameObject.Instantiate(Enemies[Num_enemy]) as GameObject;
                        dummy = GameObject.Instantiate(dummy_game) as GameObject;
                        dummy.transform.position = Map_Tag.Mob_spawn[len].position + Vector3.up *1;
                        dummy.transform.parent = gameObject.transform;
                        Enemy_Count++;
                    }
                    
                }
                //���� ��ȯ
                list_length = Map_Tag.Trap_spawn.Length;
                for (int len = 0; len < list_length; len++)
                {
                    if (Random.Range(0, 10) > 5)
                    {
                        dummy = GameObject.Instantiate(Traps[Random.Range(0, 4)]) as GameObject;
                        dummy.transform.position = Map_Tag.Trap_spawn[len].position;
                        dummy.transform.parent = gameObject.transform;
                        Trap_Count++;
                    }
                }
                //������ ��ȯ
                list_length = Map_Tag.Item_spawn.Length;
                for (int len = 0; len < list_length; len++)
                {
                    if (Random.Range(0, 10) > 5)
                    {
                        dummy = GameObject.Instantiate(Items[0]) as GameObject;
                        dummy.transform.position = Map_Tag.Item_spawn[len].position + Vector3.up * 2;
                        dummy.transform.parent = gameObject.transform;
                        Crate_Count++;
                    }
                }
                //�� ��ȯ
                list_length = Map_Tag.Gold_spawn.Length;
                for (int len = 0; len < list_length; len++)
                {
                    if (Random.Range(0, 10) > 5)
                    {
                        int Num_enemy = efsc.Gold_Select();
                        if (Num_enemy != -1)
                        {
                            dummy = GameObject.Instantiate(Gold[Num_enemy]) as GameObject;
                            dummy.transform.position = Map_Tag.Gold_spawn[len].position + Vector3.up * 5;
                            dummy.transform.parent = gameObject.transform;
                        }
                    }
                }


                //������ �� ���Ա�����
                if ((i == 0 && j == StartWhere))
                {
                    GameObject entrance_map = GameObject.Instantiate(Entrance_Exit[0]) as GameObject;
                    entrance_map.transform.position = Creator_spawn + new Vector3((int)STANDARD.STD_INTERVAL * j + 30, 1, (int)STANDARD.STD_INTERVAL * i - 6);
                    entrance_map.transform.parent = gameObject.transform;
                    Player_spawn = entrance_map.transform.Find("Player_Spawn");
                    //nav_d = entrance_map.GetComponentInChildren<NavMeshSurface>();
                }
                else if (i == Random_Height - 1 && j == FinishWhere)
                {
                    GameObject entrance_map = GameObject.Instantiate(Entrance_Exit[1]) as GameObject;
                    entrance_map.transform.rotation = Quaternion.Euler(0, 180, 0);
                    entrance_map.transform.position = Creator_spawn + new Vector3((int)STANDARD.STD_INTERVAL * j + 24, 1, (int)STANDARD.STD_INTERVAL * i + 84);
                    entrance_map.transform.parent = gameObject.transform;
                }
                else if (i == 0)
                {
                    int dummy = Random.Range(0, Dungeon_Wall.Length-1);
                    GameObject dungeon_wall = GameObject.Instantiate(Dungeon_Wall[dummy]) as GameObject;
                    dungeon_wall.transform.position = Creator_spawn + new Vector3((int)STANDARD.STD_INTERVAL * j + 30, 1, (int)STANDARD.STD_INTERVAL * i - 6);
                    dungeon_wall.transform.parent = gameObject.transform;
                }
                else if (i == Random_Height - 1)
                {
                    int dummy = Random.Range(0, Dungeon_Wall.Length-1);
                    GameObject dungeon_wall = GameObject.Instantiate(Dungeon_Wall[dummy]) as GameObject;
                    dungeon_wall.transform.rotation = Quaternion.Euler(0, 180, 0);
                    dungeon_wall.transform.position = Creator_spawn + new Vector3((int)STANDARD.STD_INTERVAL * j + 24, 1, (int)STANDARD.STD_INTERVAL * i + 84);
                    dungeon_wall.transform.parent = gameObject.transform;
                }

                if (j == 0)
                {
                    int dummy = Random.Range(0, Dungeon_Wall.Length-1);
                    GameObject dungeon_wall = GameObject.Instantiate(Dungeon_Wall[dummy]) as GameObject;
                    dungeon_wall.transform.rotation = Quaternion.Euler(0, 90, 0);
                    dungeon_wall.transform.position = Creator_spawn + new Vector3((int)STANDARD.STD_INTERVAL * j - 18, 1, (int)STANDARD.STD_INTERVAL * i + 36);
                    dungeon_wall.transform.parent = gameObject.transform;
                }
                else if (j == 3)
                {
                    int dummy = Random.Range(0, Dungeon_Wall.Length-1);
                    GameObject dungeon_wall = GameObject.Instantiate(Dungeon_Wall[dummy]) as GameObject;
                    dungeon_wall.transform.rotation = Quaternion.Euler(0, -90, 0);
                    dungeon_wall.transform.position = Creator_spawn + new Vector3((int)STANDARD.STD_INTERVAL * j + 72, 1, (int)STANDARD.STD_INTERVAL * i + 42);
                    dungeon_wall.transform.parent = gameObject.transform;
                }

                if (i == 0 && j == 0)
                {
                    GameObject dungeon_wall = GameObject.Instantiate(Dungeon_Wall[Dungeon_Wall.Length - 1]) as GameObject;
                    dungeon_wall.transform.rotation = Quaternion.Euler(0, 0, 0);
                    dungeon_wall.transform.position = Creator_spawn + new Vector3((int)STANDARD.STD_INTERVAL * j - 54, 1, (int)STANDARD.STD_INTERVAL * i - 6);
                    dungeon_wall.transform.parent = gameObject.transform;
                }
                else if (i == 0 && j == 3)
                {
                    GameObject dungeon_wall = GameObject.Instantiate(Dungeon_Wall[Dungeon_Wall.Length - 1]) as GameObject;
                    dungeon_wall.transform.rotation = Quaternion.Euler(0, -90, 0);
                    dungeon_wall.transform.position = Creator_spawn + new Vector3((int)STANDARD.STD_INTERVAL * j + 72, 1, (int)STANDARD.STD_INTERVAL * i - 42);
                    dungeon_wall.transform.parent = gameObject.transform;
                }
                else if (i == Random_Height - 1 && j == 0)
                {
                    GameObject dungeon_wall = GameObject.Instantiate(Dungeon_Wall[Dungeon_Wall.Length - 1]) as GameObject;
                    dungeon_wall.transform.rotation = Quaternion.Euler(0, 90, 0);
                    dungeon_wall.transform.position = Creator_spawn + new Vector3((int)STANDARD.STD_INTERVAL * j - 18, 1, (int)STANDARD.STD_INTERVAL * i + 120);
                    dungeon_wall.transform.parent = gameObject.transform;
                }
                else if (i == Random_Height - 1 && j == 3)
                {
                    GameObject dungeon_wall = GameObject.Instantiate(Dungeon_Wall[Dungeon_Wall.Length - 1]) as GameObject;
                    dungeon_wall.transform.rotation = Quaternion.Euler(0, 180, 0);
                    dungeon_wall.transform.position = Creator_spawn + new Vector3((int)STANDARD.STD_INTERVAL * j + 108, 1, (int)STANDARD.STD_INTERVAL * i + 84);
                    dungeon_wall.transform.parent = gameObject.transform;
                }


                


            }
        }


        nav_d.BuildNavMesh();
    }

    public GameObject Instantiate_object(GameObject item, Vector3 vec)
    {
        GameObject dummy = Instantiate(item) as GameObject;
        dummy.transform.position = vec;
        dummy.transform.parent = gameObject.transform;
        return dummy;
    }

    int random_non_duplicated(int len, int[] dup)
    {
        int random_int;
        bool isdup = false ;
        while (true)
        {
            random_int = Random.Range(0, len);
            for (int i = 0; i < dup.Length; i++)
            {
                if (dup[i] == random_int)
                    isdup = true;
            }
            if (!isdup)
                break;
        }
        return random_int;
    }
}
