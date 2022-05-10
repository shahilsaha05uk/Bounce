using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[CreateAssetMenu(fileName = "Shop", menuName = "Items", order = 1)]
public class SC_Objects : ScriptableObject
{
    public ObjectType objType;
    public Sprite image;
    public string objText;
    public string objCost;
    public GameObject obj;
}

public enum ObjectType
{
    SHIELD,
    FIRE,
    STICKY
}