using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public GameObject Music;
    public player_script player;
    //public boss boss;
    public int stage;
    public float playTime;
    public Vector3 dunSpawn;
    public bool isStart, isloading;

    public GameObject Sign;
    public GameObject BestTime_O, ClearTimes_O;

    public GameObject ClearButton, OverButton;

    public GameObject menuPanel, gamePanel, gameoverPanel, optionPanel, pausePanel, gameclearPanel;
    public GameObject PortalKey, GrabKey, GrabKey_i;
    public GameObject menuBack, pauseBack;
    public Text highScoreTxt, bestTimeTxt, ClearTimes;
    public Text scoreTxt, stageTxt, playTimeTxt;
    public Text playerHealthTxt, playerAmmo, equipAmmo, playerCoin, playerGrenade;
    public Text curScoreTxt, bestTxt, GVHighScoreTxt;
    public Text GV_Mob, GV_Coin, CoinGet;
    public GameObject[] Item, Weapon;
    public Image LoadingIM;

    private IEnumerator Itembar, coinIE;
    private AudioMixer Music_Mixer;
    private float fadecount;

    public GameObject GCBest;
    public Text GCBest_Time_Txt, GCBest_Score_Txt, GCMob_Score_Txt, GCCoin_Score_Txt, GCTime_Txt, GCScore_Txt;
    void Awake()
    {
        if (!PlayerPrefs.HasKey("HighScore"))
            PlayerPrefs.SetInt("HighScore", 0);
        if (!PlayerPrefs.HasKey("Clear_Times"))
            PlayerPrefs.SetInt("Clear_Times", 0);
        if (!PlayerPrefs.HasKey("BestTime"))
            PlayerPrefs.SetFloat("BestTime", 0);
        if (!PlayerPrefs.HasKey("MusicVolume"))
            PlayerPrefs.SetFloat("MusicVolume", -20f);
        if (!PlayerPrefs.HasKey("EffectVolume"))
            PlayerPrefs.SetFloat("EffectVolume", -20f);

        highScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("HighScore"));

        if (PlayerPrefs.GetInt("Clear_Times") > 0)
        {
            BestTime_O.SetActive(true); //ClearTimes_O.SetActive(true);
            int hour = (int)(PlayerPrefs.GetFloat("BestTime") / 3600);
            int minute = (int)((PlayerPrefs.GetFloat("BestTime") - hour * 3600) / 60);
            int second = (int)(PlayerPrefs.GetFloat("BestTime") % 60);

            bestTimeTxt.text = string.Format("{0:00}:{1:00}:{2:00}", hour, minute, second);
            //ClearTimes.text = string.Format("{0:00}회 클리어", PlayerPrefs.GetInt("Clear_Times"));
        }
        else
        {
            Sign.SetActive(true);
        }
    }
    void Start()
    {
        Music_Mixer = Music.GetComponent<Music_script>().mixer;
        StartCoroutine(LoadingOut());
        Invoke("Start_Music", 1.5f);

    }

    void Start_Music()
    {
        Music.GetComponent<Music_script>().PlayMusic(3);
    }


    public void GameStart()
    {
        StartCoroutine(Game_Start_for_Loading());
    }
    IEnumerator Game_Start_for_Loading()
    {
        yield return StartCoroutine(LoadingStart());
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        player.gameObject.SetActive(true);
        Music.GetComponent<Music_script>().PlayMusic(0);
        yield return StartCoroutine(LoadingOut());

    }

    public IEnumerator LoadingStart()
    {
        if (!isStart)
        {
            //player.GetComponent<player_script>().enabled = false;
            //Music_Mixer.SetFloat("MasterVolume", -10f);
            StartCoroutine(MusicDU(-10));
            isStart = true;
            LoadingIM.gameObject.SetActive(true);
            LoadingIM.color = new Color(0, 0, 0, 0);
            fadecount = 0f;
            while (fadecount < 1f)
            {
                yield return new WaitForSeconds(0.001f);
                fadecount += 0.01f;
                LoadingIM.color = new Color(0, 0, 0, fadecount);
            }
            isStart = false;
        }
    }
    public IEnumerator LoadingOut()
    {
        if (!isStart)
        {
            StartCoroutine(MusicDU(0));

            isStart = true;
            LoadingIM.gameObject.SetActive(true);
            LoadingIM.color = new Color(0, 0, 0, 1);
            fadecount = 1f;
            while (fadecount > 0f)
            {
                yield return new WaitForSeconds(0.001f);
                fadecount -= 0.01f;
                LoadingIM.color = new Color(0, 0, 0, fadecount);
            }
            LoadingIM.gameObject.SetActive(false);
            isStart = false;
            //Music_Mixer.SetFloat("MasterVolume", 0);
            //player.GetComponent<player_script>().enabled = true;

        }
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void Optionopen()
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(false);
        optionPanel.SetActive(true);
        menuBack.SetActive(true);
    }

    public void OptionOut()
    {
        menuPanel.SetActive(true);
        optionPanel.SetActive(false);
        menuBack.SetActive(false);
    }

    void Update()
    {
        if (stage > 0)
            playTime += Time.deltaTime;
    }

    void LateUpdate()
    {
        //점수
        scoreTxt.text = string.Format("{0:n0}", player.score);
        //스테이지
        if (stage != 0)
            stageTxt.text = "STAGE " + stage;
        else
            stageTxt.text = "STAGE -";

        //시간
        int hour = (int)(playTime / 3600);
        int minute = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);

        playTimeTxt.text = string.Format("{0:00}:{1:00}:{2:00}", hour, minute, second);

        //체력
        playerHealthTxt.text = player.pHealth.ToString();
        playerCoin.text = string.Format("{0:n0}", player.Money);

        if (player.pAmmo == 0)
            playerAmmo.text = "- / " + player.MaxpAmmo;
        else
            playerAmmo.text = player.pAmmo + " / " + player.MaxpAmmo;

        if (player.equipWeapon == null || player.equipWeapon.type != Weapon_script.Type.Long)
            equipAmmo.text = "- / -";
        else
        {
            if (player.equipWeapon.Ammo != 0)
                equipAmmo.text = player.equipWeapon.Ammo + " / " + player.equipWeapon.maxAmmo;
            else
                equipAmmo.text = "- / " + player.equipWeapon.maxAmmo;
        }

        if (player.hasGrenades == 0)
            playerGrenade.text = "- / " + player.MaxpGrenades;
        else
            playerGrenade.text = player.hasGrenades + " / " + player.MaxpGrenades;


    }

    public void GameOver()
    {
        //Music_Mixer.SetFloat("MasterVolume", -10f);
        StartCoroutine(MusicDU(-10));
        Cursor.lockState = CursorLockMode.None;
        gamePanel.SetActive(false);
        gameoverPanel.SetActive(true);
        GVHighScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("HighScore"));

        StartCoroutine(GameOver_Calc());

    }

    IEnumerator GameOver_Calc()
    {
        yield return new WaitForSeconds(1f);
        GV_Mob.text = string.Format("{0:n0}", player.score);
        Music.GetComponent<Music_script>().effects[0].GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1f);
        GV_Coin.text = string.Format("{0:n0}", player.Money);
        Music.GetComponent<Music_script>().effects[0].GetComponent<AudioSource>().Play();
        int Score_current = player.score + player.Money;

        yield return new WaitForSeconds(1f);
        curScoreTxt.text = string.Format("{0:n0}", Score_current);
        Music.GetComponent<Music_script>().effects[1].GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(0.5f);
        int maxScore = PlayerPrefs.GetInt("HighScore");
        if (Score_current > maxScore && PlayerPrefs.GetFloat("BestTime") == 0)
        {
            Music.GetComponent<Music_script>().effects[2].GetComponent<AudioSource>().Play();
            bestTxt.gameObject.SetActive(true);
            PlayerPrefs.SetInt("HighScore", Score_current);
            PlayerPrefs.Save();
        }
        OverButton.SetActive(true);
    }




    public void Data_Clear()
    {
        
        PlayerPrefs.SetInt("HighScore", 0);
        PlayerPrefs.SetInt("Clear_Times", 0);
        PlayerPrefs.SetFloat("BestTime", 0);
        PlayerPrefs.SetFloat("MusicVolume", -20f);
        PlayerPrefs.SetFloat("EffectVolume", -20f);
        Restart();
    }
    public void Restart()
    {
        Music.GetComponent<Music_script>().StopMusic();
        StartCoroutine(Restart_black());
    }
    IEnumerator Restart_black()
    {
        Time.timeScale = 1;
        yield return StartCoroutine(LoadingStart());
        SceneManager.LoadScene(0);
    }


    public void PausePan()
    {
        //StartCoroutine(MusicDU(-10));
        Music_Mixer.SetFloat("MasterVolume", -10f);


        Cursor.lockState = CursorLockMode.None;
        gamePanel.SetActive(false);
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        //Music_Mixer.SetFloat("MasterVolume", -10f);
    }

    public void PauseOut()
    {

        player.isPause = false;
        pausePanel.SetActive(false);
        gamePanel.SetActive(true);
        optionPanel.SetActive(false);
        Time.timeScale = 1;
        //Music_Mixer.SetFloat("MasterVolume", 0);
        StartCoroutine(MusicDU(0));
        switch (player.view_position)
        {
            case 0:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true;
                break;
            case 1:
                Cursor.lockState = CursorLockMode.Confined;
                break;
        }
    }
    public void PauseOption()
    {
        optionPanel.SetActive(true);
        pausePanel.SetActive(false);
        pauseBack.SetActive(true);
    }
    public void PauseOptionOut()
    {
        optionPanel.SetActive(false);
        pausePanel.SetActive(true);
        pauseBack.SetActive(false);
    }


    public Vector3 dunSpawnPoint()
    {
        if (stage != 6 && stage != 12 && stage != 2 && stage != 9)
            Music.GetComponent<Music_script>().PlayMusic(1);
        else 
            Music.GetComponent<Music_script>().PlayMusic(2);
        dunSpawn += new Vector3(1000, 0, 1000);
        return dunSpawn;
    }

    public void ItemGet(int num)
    {
        if (num == 0)
        {
            StartCoroutine(ItemBar(num));
        }
        else if (num >= 1 && num <= 3)
        {
            for (int i = 1; i < 4; i++)
            { Item[i].SetActive(false); }
            StartCoroutine(ItemBar(num));
        }
        else if (num >= 4 && num <= 7)
        {
            for (int i = 4; i < 7; i++)
            { Item[i].SetActive(false); }
            StartCoroutine(ItemBar(num));
        }
	}
    IEnumerator ItemBar(int num)
    {
        Item[num].SetActive(true);
        yield return new WaitForSeconds(2f);
        Item[num].SetActive(false);
    }
    public void WeaponIm(int num)
    {
        for (int i = 0; i < 3; i++)
        {
            Weapon[i].SetActive(false);
        }
        if (num != -1)
            Weapon[num].SetActive(true);
    }


    public void CoinGetTxt(int num)
    {
        CoinGet.gameObject.SetActive(false);
        if (coinIE != null)
            StopCoroutine(coinIE);
        CoinGet.text = "+ " + num;
        coinIE = coin_TF();
        StartCoroutine(coinIE);
    }
    IEnumerator coin_TF() {
        CoinGet.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        CoinGet.gameObject.SetActive(false);
    }


    public void GameClear()
    {
        Music.GetComponent<Music_script>().effects[3].GetComponent<AudioSource>().Play();
        //Music_Mixer.SetFloat("MasterVolume", -10f);
        StartCoroutine(MusicDU(-10));

        Cursor.lockState = CursorLockMode.None;
        gamePanel.SetActive(false);
        gameclearPanel.SetActive(true);
        GCBest_Score_Txt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("HighScore"));
        PlayerPrefs.SetInt("Clear_Times", PlayerPrefs.GetInt("Clear_Times") + 1);

        int hour = (int)(PlayerPrefs.GetFloat("BestTime") / 3600);
        int minute = (int)((PlayerPrefs.GetFloat("BestTime") - hour * 3600) / 60);
        int second = (int)(PlayerPrefs.GetFloat("BestTime") % 60);

        GCBest_Time_Txt.text = string.Format("{0:00}:{1:00}:{2:00}", hour, minute, second);

        StartCoroutine(GameClear_Calc());

    }

    IEnumerator GameClear_Calc()
    {
        yield return new WaitForSeconds(1f);
        GCMob_Score_Txt.text = string.Format("{0:n0}", player.score);
        Music.GetComponent<Music_script>().effects[0].GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1f);
        GCCoin_Score_Txt.text = string.Format("{0:n0}", player.Money);
        Music.GetComponent<Music_script>().effects[0].GetComponent<AudioSource>().Play();
        int Score_current = player.score + player.Money;



        yield return new WaitForSeconds(1f);
        GCScore_Txt.text = string.Format("{0:n0}", Score_current);
        Music.GetComponent<Music_script>().effects[1].GetComponent<AudioSource>().Play();


        int hour = (int)(playTime / 3600);
        int minute = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);

        GCTime_Txt.text = string.Format("{0:00}:{1:00}:{2:00}", hour, minute, second);

        yield return new WaitForSeconds(0.5f);
        int maxScore = PlayerPrefs.GetInt("HighScore");
        float maxTime = PlayerPrefs.GetFloat("BestTime");
        //if ((maxTime > playTime) || (Mathf.Abs(maxTime-playTime) < 600 && Score_current - maxScore >= 200000)||maxTime == 0)
        if (Score_current > maxScore)
        {
            Music.GetComponent<Music_script>().effects[2].GetComponent<AudioSource>().Play();
            GCBest.gameObject.SetActive(true);
            PlayerPrefs.SetInt("HighScore", Score_current);
            PlayerPrefs.SetFloat("BestTime", playTime);
            PlayerPrefs.Save();
        }
        ClearButton.SetActive(true);
    }


    IEnumerator MusicDU(int a)
    {
        if (a == 0)
        {
            float dummy = -10;
            for (int i = 0; i < 100; i++)
            {
                yield return new WaitForSeconds(0.01f);
                Music_Mixer.SetFloat("MasterVolume", dummy + (i * 0.1f));
            }
        }
        else if (a == -10)
        {
            float dummy = 0;
            for (int i = 0; i < 100; i++)
            {
                yield return new WaitForSeconds(0.01f);
                Music_Mixer.SetFloat("MasterVolume", dummy - (i * 0.1f));
            }
        }

    }
}
