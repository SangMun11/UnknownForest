using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Music_script : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]private GameObject[] musics;
	public AudioMixer mixer;
	private int curnum = -1;
	public GameObject[] effects;
	private bool isMute;

	private void Awake()
	{
		//musics = GameObject.FindGameObjectsWithTag("Music");


		//foreach(GameObject music in musics)
		DontDestroyOnLoad(this.gameObject);
		audioSource = musics[0].GetComponent<AudioSource>();
	}

	private void Start()
	{
		if (PlayerPrefs.GetFloat("MusicVolume")== -40f)
			mixer.SetFloat("MusicVolume", -80f);
		else
			mixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
		if (PlayerPrefs.GetFloat("EffectVolume") == -40f)
			mixer.SetFloat("MusicVolume", -80f);
		else
			mixer.SetFloat("EffectVolume", PlayerPrefs.GetFloat("EffectVolume"));
	}

	void Update()
	{
		if ( !isMute&&PlayerPrefs.GetFloat("MusicVolume") == -40f)
		{
			foreach (GameObject music in musics)
			{
				music.GetComponent<AudioSource>().mute = true;
				isMute = true;
			}
		}
		else if (isMute && PlayerPrefs.GetFloat("MusicVolume") != -40f)
		{
			foreach (GameObject music in musics)
			{
				music.GetComponent<AudioSource>().mute = false;
				isMute = false;
			}
		}
	}


	public void PlayMusic(int num)
	{
		if (curnum != num)
		{
			if (audioSource.isPlaying)
				StopMusic();
			curnum = num;
			audioSource = musics[num].GetComponent<AudioSource>();
			audioSource.Play();
		}

	}

	

	public void StopMusic()
	{
		audioSource.Stop();
		foreach (GameObject mu in musics)
			mu.GetComponent<AudioSource>().Stop();
	}
}
