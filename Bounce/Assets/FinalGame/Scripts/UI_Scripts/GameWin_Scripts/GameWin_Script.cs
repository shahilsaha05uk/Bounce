using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameWin_Script : MonoBehaviour
{
    [SerializeField] private GameManager manager;
    [SerializeField] private UIScript UI;
    
    [SerializeField] private GameObject[] Stars;
    
    [SerializeField] private Sprite WhiteStar;
    [SerializeField] private Sprite YellowStar;

    
    private void OnEnable()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        StarRating();
    }
    public void StarRating()
    {
        int no_of_yellowStar = manager.starCount;
        
        for (int i = 0; i < no_of_yellowStar; i++)
        {
            Stars[i].GetComponent<Image>().sprite = YellowStar;
        }
    }
    
}
