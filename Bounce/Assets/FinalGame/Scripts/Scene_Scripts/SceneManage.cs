using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneStatus { LOAD, UNLOAD}

public class SceneManage : MonoBehaviour
{
    public static SceneManage Instance;

    [SerializeField] private GameObject[] taggedObjs;
    private GameManager manager;
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

        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        SceneChangeTrigger("MainMenu");
        if(SceneManager.GetActiveScene().name == "EmptyScene")
        {
            SceneManager.UnloadSceneAsync("EmptyScene");
        }
    }
    public void SceneChangeTrigger(string sceneName)
    {
        StartCoroutine(SceneLoader(sceneName));
    }
    public void AdditiveSceneTrigger(string sceneName, SceneStatus sceneStatus)
    {
        StartCoroutine(AdditiveSceneLoader(sceneName, sceneStatus));
    }
    private IEnumerator AdditiveSceneLoader(string sceneName, SceneStatus sceneStatus)
    {
        if (SceneManager.GetSceneByName(sceneName).IsValid())
        {
            switch (sceneStatus)
            {
                case SceneStatus.LOAD:
                    SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                    break;
                case SceneStatus.UNLOAD:
                    SceneManager.UnloadSceneAsync(sceneName);

                    break;
            }
        }
        yield return null;
    }
    private IEnumerator SceneLoader(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            GetListOfOpenedScenes();
            // Load the new Scene

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


            // Shift all the items to the new scene and unload the previous scenes

            if (scene.IsValid())
            {
                SceneManager.SetActiveScene(scene);
                ShiftItems(SceneManager.GetActiveScene());
            }

            yield return StartCoroutine(CloseOpenedScenes());
            manager.sceneChangeTrigger.Invoke();

        }
    }
    public Scene GetCurrentScene()
    {
        return SceneManager.GetActiveScene();
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
    public IEnumerator CloseOpenedScenes()
    {
        foreach (var item in sceneNames)
        {
           SceneManager.UnloadSceneAsync(item);
        }
        yield return null;
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
