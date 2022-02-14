using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    public static SceneManage Instance;

    [SerializeField] private GameObject[] taggedObjs;
    
    public List<string> sceneNames;
    public string dontDestroyTag;
    public float sceneLoadProgress;


    private void OnEnable()
    {
        DontDestroyOnLoad(this.gameObject);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //SceneChangeTrigger("MainMenu");
       // SceneManager.UnloadSceneAsync("EmptyScene");
    }
    public void SceneChangeTrigger(string sceneName)
    {
        StartCoroutine(SceneLoader(sceneName));
    }

    private IEnumerator SceneLoader(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            GetListOfOpenedScenes();

            AsyncOperation sceneToLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            sceneToLoad.allowSceneActivation = false;

            while (sceneToLoad.progress < 0.9f)
            {
                sceneLoadProgress = sceneToLoad.progress;
                yield return null;
            }
            sceneToLoad.allowSceneActivation = true;

            Scene scene = SceneManager.GetSceneByName(sceneName);

            while (!scene.isLoaded)
            {
                yield return null;
            }

            if (scene.IsValid())
            {
                SceneManager.SetActiveScene(scene);
                ShiftItems(SceneManager.GetActiveScene());
            }
            CloseOpenedScenes();

            yield return null;
        }
    }

    #region SceneLoader Supporting Functions
    public void GetListOfOpenedScenes()
    {
        int sceneCount = SceneManager.sceneCount;

        Scene[] loadedScenes = new Scene[sceneCount];

        sceneNames = new List<string>();

        for (int i = 0; i < loadedScenes.Length; i++)
        {
            sceneNames.Add(SceneManager.GetSceneAt(i).name);
        }

    }
    public void CloseOpenedScenes()
    {
        foreach (var item in sceneNames)
        {
            SceneManager.UnloadSceneAsync(item);
        }
    }
    public void ShiftItems(Scene s)
    {
        taggedObjs = GameObject.FindGameObjectsWithTag(dontDestroyTag);

        foreach (var item in taggedObjs)
        {
            SceneManager.MoveGameObjectToScene(item.gameObject, s);
        }
    }
    #endregion
}
