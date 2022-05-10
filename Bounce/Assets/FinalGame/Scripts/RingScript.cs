using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingScript : MonoBehaviour
{
    public bool ringCleared = false;
    public bool RingCleared()
    {
        return ringCleared;
    }
    public void DisableRing()
    {
        SpriteRenderer[] ringSprites = GetComponentsInChildren<SpriteRenderer>();

        foreach (var item in ringSprites)
        {
            item.color = Color.grey;
        }
        ringCleared = true;
    }

    public void EnableRing()
    {
        SpriteRenderer[] ringSprites = GetComponentsInChildren<SpriteRenderer>();

        foreach (var item in ringSprites)
        {
            item.color = Color.yellow;
        }
        ringCleared = false;
    }
}
