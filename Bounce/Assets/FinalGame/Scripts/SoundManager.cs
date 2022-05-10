using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public static class SoundManager
{
    
    [Space(5)]
    //public Sprite SoundEnabled;
    //public Sprite SoundDisabled;

    [Space(5)]
    public static AudioSource audioSource;
    public static AudioClip audioClip;
    
    [Space(5)]
    public static bool isMute;

    private static void OnEnable()
    {
        isMute = false;
    }
    
    public static void SetupAudio(AudioSource source)
    {
        audioSource = source;
        audioClip = audioSource.clip;
    }

    public static void SetLoop(bool loop)
    {
        if (audioSource != null)
        {
            audioSource.loop = loop;
        }
    }
    public static void SetPlayOnAwake(bool playOnAwake)
    {
        if (audioSource != null)
        {
            audioSource.playOnAwake = playOnAwake;
        }
    }
    public static void PlayAudio()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public static void PlayAudioOneShot()
    {
        audioSource.PlayOneShot(audioClip);
    }
    
    public static void PauseAudio()
    {
        audioSource.Pause();
    }
    public static void StopAudio()
    {

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public static void MuteAudio()
    {
        isMute = true;
        AudioListener.volume = 0f;
    }
    public static void UnMuteAudio()
    {
        isMute = false;
        AudioListener.volume = 1f;
        
    }

}
