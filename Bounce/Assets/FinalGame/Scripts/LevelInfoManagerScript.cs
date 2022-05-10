using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LevelInfoManagerScript : MonoBehaviour
{
    public bool smallBall;
    public bool bigBall;
    
    public bool isComplete;
    public int chances;

    public AudioSource levelSound;
    private GameManager manager;

    private void OnEnable()
    {
        StartCoroutine(LevelSoundSetup());
    }
    private IEnumerator LevelSoundSetup()
    {
        while ((manager = GameObject.Find("GameManager").GetComponent<GameManager>()) == null)
        {
            yield return null;
        }

        chances = 3;
        manager.UI.UpdateLives(chances);
        manager.LevelMusic = levelSound;
        manager.SoundPlay(manager.LevelMusic, false, true);
    }
}
