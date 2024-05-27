using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Musicmanager : MonoBehaviour
{
    public GameObject[] musics;
	public GameObject[] swing;
	public GameObject[] Item;
	private AudioSource dummy;

	public void EffectPlay(int num)
	{
		dummy = musics[num].GetComponent<AudioSource>();
		if (dummy.isPlaying)
			return;
		else
			dummy.Play();

	}

	public void EffectOut(int num)
	{
		dummy = musics[num].GetComponent<AudioSource>();
		if (dummy.isPlaying)
			dummy.Stop();
		else
			return;
	}

	public void EffectAllOut()
	{
		foreach (GameObject music in musics)
			music.GetComponent<AudioSource>().Stop();
		foreach (GameObject sw in swing)
			sw.GetComponent<AudioSource>().Stop();
	}

	public void SwingPlay(int num)
	{
		dummy = swing[num].GetComponent<AudioSource>();
		if (dummy.isPlaying)
			return;
		else
			dummy.Play();
	}
	public void SwingOut(int num)
	{
		dummy = swing[num].GetComponent<AudioSource>();
		if (dummy.isPlaying)
			dummy.Stop();
		else
			return;
	}
	public void ItemPlay(int num)
	{
		dummy = Item[num].GetComponent<AudioSource>();
		if (dummy.isPlaying)
			return;
		else
			dummy.Play();
	}
	public void ItemOut(int num)
	{
		dummy = Item[num].GetComponent<AudioSource>();
		if (dummy.isPlaying)
			dummy.Stop();
		else
			return;
	}
}
