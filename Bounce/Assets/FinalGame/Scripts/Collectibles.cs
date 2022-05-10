using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectibleType { RED_DIAMOND, GREEN_DIAMOND, RED_POLYGON, GREEN_POLYGON, SAVE_GAME, SHIELD };
public class Collectibles : MonoBehaviour
{
    public CollectibleType collectibleType;

    public CollectibleType GetCollectibleType() { return collectibleType; }
}
