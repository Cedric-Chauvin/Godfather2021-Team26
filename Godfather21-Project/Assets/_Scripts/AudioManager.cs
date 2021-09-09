using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer soundMixer;
    [SerializeField] AudioMixer musicMixer;

    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundSlider;
    private void Start()
    {
        if (PlayerPrefs.GetInt("FirstLanuch") == 0)
        {
            PlayerPrefs.SetInt("FirstLanuch", 1);
            PlayerPrefs.SetFloat("SoundVolume", 0);
            PlayerPrefs.SetFloat("MusicVolume", 0);
            PlayerPrefs.Save();
        }
        else
        {
            soundMixer.SetFloat("SoundVolume", PlayerPrefs.GetFloat("SoundVolume"));
            musicMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        }
    }


    public void OpenOptions()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume");
    }


    public void UpdateVolume()
    {
        soundMixer.SetFloat("SoundVolume", soundSlider.value);
        musicMixer.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.Save();
    }
}
