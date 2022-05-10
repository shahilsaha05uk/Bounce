using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Levels", menuName = "Level_Items", order = 2)]
public class SC_Levels : ScriptableObject
{
    public Levels_Num objType;
    public Sprite image;
    public string levelName;
}

public enum Levels_Num
{
    LEVEL_1,
    LEVEL_2,
    LEVEL_3,
}