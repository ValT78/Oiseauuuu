using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : Singleton<AudioManager>
{

    public AudioSource soundsSource;
    public AudioSource musicSource;
    public AudioSource ambiantSource;
    public AudioSource waveSource;

    [Header("Musique")]
    public AudioClip musique;
    [Header("Sons")]
    public AudioClip menuNavigation;
    public AudioClip placement;
    public AudioClip seagull_low;
    public AudioClip seagull_medium;
    public AudioClip seagull_high;
    public AudioClip selection;
    public AudioClip wave;


    private void Start()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        musicSource.clip = musique;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayWave()
    {
        waveSource.Play();
    }

    public void StopWave()
    {
        waveSource.Stop();
    }

    

    public void PlayMenuNavigation()
    {
        soundsSource.PlayOneShot(menuNavigation);
    }

    public void PlayPlacement()
    {
        soundsSource.PlayOneShot(placement);
    }

    public void PlaySelection()
    {
        soundsSource.PlayOneShot(selection);
    }

    public void PlaySeagullLow()
    {
        ambiantSource.PlayOneShot(seagull_low);
    }

    public void PlaySeagullMedium()
    {
        ambiantSource.PlayOneShot(seagull_medium);
    }

    public void PlaySeagullHigh()
    {
        ambiantSource.PlayOneShot(seagull_high);
    }


}
