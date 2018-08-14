using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

	public Slider soundsSlider;
	public Slider musicSlider;

	public void UpdateSoundsVolume(float volume) {
		AudioManager.AM.SetVolume("soundsVolume", volume);
	}

	public void UpdateMusicVolume(float volume) {
		AudioManager.AM.SetVolume("musicVolume", volume);
	}

	public void UpdateSliders() {
		soundsSlider.value = PlayerPrefs.GetFloat("soundsVolume");
		musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
	}
}
