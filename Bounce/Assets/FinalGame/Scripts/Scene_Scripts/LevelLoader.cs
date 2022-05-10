using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public int level;
    public GameManager manager;
    
    private void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            StartCoroutine(CheckGateOpen());
        }
    }

    private IEnumerator CheckGateOpen()
    {
        while (!manager.endPointOpened)
        {
            yield return null;
        }

        while (!manager.endPointReached)
        {
            yield return null;
        }

        if (manager.endPointOpened && manager.endPointReached)
        {
            manager.endPointOpened = false;
            manager.endPointReached = false;
            Animator anim = GetComponent<Animator>();

            Debug.Log("Next Level");
            manager.levelChangeTrigger?.Invoke();
            SceneManage.Instance.SceneChangeTrigger($"Level {level}");
        }

        yield return null;
    }
}
