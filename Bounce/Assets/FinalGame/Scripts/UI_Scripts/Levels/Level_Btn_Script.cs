using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Level_Btn_Script : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI btn_text;
    
    [SerializeField] private Sprite btn_Image;
    [SerializeField]private Sprite lockImg;
    [SerializeField] private Sprite YellowStar;
    
    [SerializeField]private List<GameObject> StarList;

    private SC_Levels level;
    public Action<SC_Levels, Level_Btn_Script> onLevelButtonClick;


    public void Init(SC_Levels levelInfo, bool isComplete)
    {
        level = levelInfo;

        if (!isComplete)
        {
            btn_Image = lockImg;
        }
        else
        {
            btn_Image = level.image;
            
            int starCount = PlayerPrefs.GetInt("StarCount");

            if (starCount > GetHighestStar())
            {
                Update_Star_Rating(starCount);
            }
        }
        btn_text.text = level.levelName;
        GetComponent<Image>().sprite = btn_Image;
    }

    public bool isLocked()
    {
        if (GetComponent<Image>().sprite == lockImg)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public void Update_Star_Rating(int stars)
    {
        for (int i = 0; i < stars; i++)
        {
            StarList[i].GetComponent<Image>().sprite = YellowStar;
        }
    }
    public int GetHighestStar()
    {
        int count = 0;
        for (int i = 0; i < StarList.Count; i++)
        {
            if (StarList[i].GetComponent<Image>().sprite == YellowStar)
            {
                count++;
            }
        }
        return count;
    }
    public void OnLevelButtonClick()
    {
        onLevelButtonClick?.Invoke(level, this);
    }

}
