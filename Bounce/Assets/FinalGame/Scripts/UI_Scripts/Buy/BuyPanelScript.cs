using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuyPanelScript : MonoBehaviour
{
    [SerializeField] private Transform panelTransform;
    [SerializeField] private Button_Script buttonPrefab;
    
    [SerializeField] private SC_Objects[] scList;
    private Button_Script[] buttonList;
    public static bool onItemBought;
    public UIScript shopController;
    public GameManager manager;

    public void OnEnable()
    {
        buttonList = new Button_Script[scList.Length];
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i] = Instantiate(buttonPrefab);
            buttonList[i].transform.SetParent(panelTransform);
            
            buttonList[i].Init(scList[i]);
            buttonList[i].onItemBought += OnButtonPressed;
        }

    }

    public void OnDisable()
    {
        for (int i = buttonList.Length - 1; i >= 0; i--)
        {
            buttonList[i].onItemBought -= OnButtonPressed;
            Destroy(buttonList[i].gameObject);
        }
    }

    private void OnButtonPressed(SC_Objects obj)
    {
        Debug.Log("I bought the item: "+ obj.objText.ToString());

        switch (obj.objType)
        {
            case ObjectType.SHIELD:
                manager.instantiatedPrefabType = ObjectType.SHIELD;
                manager.instantiatedPlayer.GetComponent<ShieldMechanic>().Activate();
                break;
            case ObjectType.FIRE:
                manager.instantiatedPrefabType = ObjectType.FIRE;
                manager.instantiatedPlayer.GetComponent<FireMechanic>().Activate();
                break;
            case ObjectType.STICKY:
                manager.instantiatedPrefabType = ObjectType.STICKY;
                manager.instantiatedPlayer.GetComponent<StickyMechanic>().Activate();
                break;
                
        }
        
        
        UIScript.buyCanvasOpened = false;
        shopController.HideCanvas(CanvasEnum.SHOP);
        shopController.ShowCanvas(CanvasEnum.GAMEPLAY);
    }
}
