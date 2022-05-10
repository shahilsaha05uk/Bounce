using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelsMenu_OnLoad : MonoBehaviour
{
    [SerializeField]private Transform LevelPanel;
    [SerializeField]private Level_Btn_Script lvl_btn_prefab;
    

    [SerializeField]private List<SC_Levels> levelsList;
    [SerializeField]private List<GameObject> StarList;
    [SerializeField]private Level_Btn_Script[] lvl_btn_prefab_Array;
    public GameManager manager;
    public UIScript UI;

    private IEnumerator FindManager()
    {
        while ((manager = GameObject.Find("GameManager").GetComponent<GameManager>())== null)
        {
            yield return null;
        }

        while (manager.levelStatus.Count <=0)
        {
            yield return null;
        }
    }
    
    private void OnEnable()
    {
        StartCoroutine(FindManager());
        lvl_btn_prefab_Array = new Level_Btn_Script[levelsList.Count];

        for (int i = 0; i < lvl_btn_prefab_Array.Length; i++)
        {
            lvl_btn_prefab_Array[i] = Instantiate(lvl_btn_prefab);
            lvl_btn_prefab_Array[i].transform.SetParent(LevelPanel);
            lvl_btn_prefab_Array[i].Init(levelsList[i], manager.levelStatus[levelsList[i].levelName]);
            lvl_btn_prefab_Array[i].onLevelButtonClick += InvokeLevel;
        }   
    }
    public void OnDisable()
    {
        for (int i = lvl_btn_prefab_Array.Length - 1; i >= 0; i--)
        {
            lvl_btn_prefab_Array[i].onLevelButtonClick -= InvokeLevel;
            Destroy(lvl_btn_prefab_Array[i].gameObject);
        }
    }

    public void InvokeLevel(SC_Levels levelInfo, Level_Btn_Script button)
    {
        if (!button.isLocked())
        {
            UI.OnLoadingLevel(levelInfo.levelName);
        }
    }
}
