using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider slider;
	public int MusicNum;

	private void Start()
	{
		switch (MusicNum) {
			case 0:
				slider.value = PlayerPrefs.GetFloat("MusicVolume");
				break;
			case 1:
				slider.value = PlayerPrefs.GetFloat("EffectVolume");
				break;
		}
	}
	public void SetLevel(float sliderValue)
	{
		//mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
		//PlayerPrefs.SetFloat("MusicVolume", sliderValue);
		float sound = slider.value;
		switch (MusicNum) {
			case 0:
				PlayerPrefs.SetFloat("MusicVolume", sound);
				if (sound <= -40f) mixer.SetFloat("MusicVolume", -80);
				else mixer.SetFloat("MusicVolume", sound);
				break;
			case 1:
				PlayerPrefs.SetFloat("EffectVolume", sound);
				if (sound <= -40f) mixer.SetFloat("EffectVolume", -80);
				else mixer.SetFloat("EffectVolume", sound);
				break;
		}
	}
}
