using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioView : MonoBehaviour
{
    private void Awake() {
        DontDestroyOnLoad(gameObject);
        audio_slider.value = PlayerPrefs.GetFloat("SoundVolume");
        sountrack_slider.value = PlayerPrefs.GetFloat("SoundtrackVolume");
    }
    public AudioMixer mixer;

    public AudioController audio_controller;
    public Slider audio_slider;
    public void ChangeAudioVolume() {
        PlayerPrefs.SetFloat("SoundVolume",audio_slider.value);
        mixer.SetFloat("SoundVolume",  Mathf.Log(audio_slider.value)*20);
    }

    public SoundtrackController soundtrack_controller;
    public Slider sountrack_slider;
    public void ChangeSountrackVolume() {
        PlayerPrefs.SetFloat("SountrackVolume",sountrack_slider.value);
        mixer.SetFloat("SoundtrackVolume",  Mathf.Log(sountrack_slider.value)*20);
    }
}
