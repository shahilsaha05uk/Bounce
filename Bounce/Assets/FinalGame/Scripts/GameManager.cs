using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Action sceneChangeTrigger;

    public List<GameObject> ringList;
    public int ringsCleared;
    public int starCount;
    public int ringCountDivision;
    private void Start()
    {
        sceneChangeTrigger += OnSceneChange;

    }

    private void OnSceneChange()
    {
        StartCoroutine(GameplayManager());
    }
    public void ListClear()
    {
        for (int i = 0; i < ringList.Count; i++)
        {
            ringList[i] = null;

        }
        ringList.Clear();
    }

    public int GetLayerID(LayerMask layer)
    {
        return (int)Mathf.Log(layer.value, 2);
    }

    private IEnumerator GameplayManager()
    {
        StartCoroutine(ScoreManager());

        ListClear();
        ringList = GameObject.FindGameObjectsWithTag("Rings").ToList();
        Debug.Log("Scene Change");


        yield return null;
    }



    // Score Manager
    private IEnumerator ScoreManager()
    {
        ringCountDivision = ringList.Count / 3;


        /* 
         * if the rings cleared is == no of rings than 3 stars
         * else if rings cleared is less than total rings but more than ringdivision than 2 stars
         * else 1 star
         */

        if (ringsCleared >= ringList.Count)
        {
            starCount = 3;
        }
        else if (ringsCleared < ringList.Count && ringsCleared > ringCountDivision)
        {
            starCount = 2;
        }
        else
        {
            starCount = 1;
        }

        yield return null;
    }


    // Collectible Manager
    public IEnumerator Collectible(Collider2D collider)
    {
        Collectibles collectibles;
        bool b = collider.gameObject.TryGetComponent<Collectibles>(out collectibles);

        if (b)
        {
            switch (collectibles.GetCollectibleType())
            {
                case CollectibleType.RED_DIAMOND:
                    Debug.Log("Red Diamond");
                    break;
                case CollectibleType.GREEN_DIAMOND:
                    Debug.Log("Green Diamond");
                    break;
                case CollectibleType.RED_POLYGON:
                    Debug.Log("Red Polygon");
                    break;
                case CollectibleType.GREEN_POLYGON:
                    Debug.Log("Green Polygon");
                    break;
                case CollectibleType.SAVE_GAME:
                    Debug.Log("Save Game");
                    break;
            }
        }

        Destroy(collider.gameObject, 1f);
        yield return null;
    }


}
