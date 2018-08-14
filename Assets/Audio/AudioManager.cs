using System;
﻿using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public Sound[] sounds;
	public static AudioManager AM;
	public AudioMixer audioMixer;

	void Awake () {
		if (AM == null) {
			DontDestroyOnLoad(this.gameObject);
			AM = this;
		}

		foreach (Sound s in sounds) {
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.outputAudioMixerGroup = s.output;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
		}
	}

	public void SetVolume(string groupVolume, float volume) {
		audioMixer.SetFloat(groupVolume, volume);
		PlayerPrefs.SetFloat(groupVolume, volume);
	}

	public void Play(string name) {
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if (s == null) {
			Debug.LogWarning("sound not found");
			return;
		}
		s.source.Play();
	}

	public void Stop(string name) {
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if (s == null) {
			Debug.LogWarning("sound not found");
			return;
		}
		s.source.Stop();
	}

	public bool IsPlaying(string name) {
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if (s == null) {
			Debug.LogWarning("sound not found");
			return false;
		}
		return s.source.isPlaying;
	}

}
