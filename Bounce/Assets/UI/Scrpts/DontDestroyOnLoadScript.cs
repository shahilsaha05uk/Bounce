using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoadScript : MonoBehaviour
{
    [SerializeField]private GameObject[] taggedObjs;
    private static DontDestroyOnLoadScript instance;
    public string dontDestroyTag;

    private void OnEnable()
    {
        Parent();
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Parent()
    {
        if (transform.childCount == 0)
        {
            taggedObjs = GameObject.FindGameObjectsWithTag(dontDestroyTag);

            foreach (var item in taggedObjs)
            {
                Debug.Log("Tagged objects: " + item.name);
                item.transform.SetParent(this.transform);
            }
        }
    }
    public void UnParent()
    {
        if (transform.childCount > 0)
        {
            taggedObjs = GameObject.FindGameObjectsWithTag(dontDestroyTag);

            foreach (var item in taggedObjs)
            {
                item.transform.parent = null;
            }
        }
    }

}
