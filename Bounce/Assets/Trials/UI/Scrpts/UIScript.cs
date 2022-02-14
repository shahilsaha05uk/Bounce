using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum CanvasEnum { MAIN_MENU, SETTINGS, PAUSE};

public class UIScript : MonoBehaviour
{
    private float AnimationTime = 0.5f;

    public CanvasGroup MainMenu;
    public CanvasGroup Settings;
    public CanvasGroup Pause;

    private UIControls _uiControls;

    private void OnEnable()
    {
        _uiControls = new UIControls();

        _uiControls.Enable();
        _uiControls.UI.First_Scene.started += FirstScene;
        _uiControls.UI.Second_Scene.started += SecondScene;
        _uiControls.UI.Third_Scene.started += ThirdScene;
    }
    private void OnDisable()
    {
        _uiControls.UI.First_Scene.performed -= FirstScene;
        _uiControls.UI.Second_Scene.performed -= SecondScene;
        _uiControls.UI.Third_Scene.performed -= ThirdScene;
        _uiControls.Disable();
    }
    private void Start()
    {
        HideCanvas(CanvasEnum.SETTINGS);
        HideCanvas(CanvasEnum.PAUSE);
        ShowCanvas(CanvasEnum.MAIN_MENU);
    }

    private void FirstScene(InputAction.CallbackContext obj)
    {
        SceneManage.Instance.SceneChangeTrigger("MainMenu");

        HideCanvas(CanvasEnum.SETTINGS);
        HideCanvas(CanvasEnum.PAUSE);
        ShowCanvas(CanvasEnum.MAIN_MENU);
    }
    private void SecondScene(InputAction.CallbackContext obj)
    {
        SceneManage.Instance.SceneChangeTrigger("Settings");

        HideCanvas(CanvasEnum.MAIN_MENU);
        HideCanvas(CanvasEnum.PAUSE);
        ShowCanvas(CanvasEnum.SETTINGS);
    }
    private void ThirdScene(InputAction.CallbackContext obj)
    {
        SceneManage.Instance.SceneChangeTrigger("UI");

        HideCanvas(CanvasEnum.MAIN_MENU);
        HideCanvas(CanvasEnum.SETTINGS);
        ShowCanvas(CanvasEnum.PAUSE);
    }



    #region Button Clicks

    //Main Menu
    public void OnNewGameButtonClick()
    {
        Debug.Log("New Game");
        SceneManage.Instance.SceneChangeTrigger("Level 1");

        HideCanvas(CanvasEnum.SETTINGS);
        HideCanvas(CanvasEnum.PAUSE);
        HideCanvas(CanvasEnum.MAIN_MENU);

    }
    public void OnSettingsButtonClick()
    {
        Debug.Log("Settings");

    }
    public void OnSaveGameButtonClick()
    {
        Debug.Log("Save Game");
    }
    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    //Pause Menu
    public void OnResumeButtonClick()
    {

    }
    public void OnExitToMainMenuButtonClick()
    {

    }

    #endregion


    public void ShowCanvas(CanvasEnum canvasToShow)
    {
        float target = 1.0f;
        switch (canvasToShow)
        {
            case CanvasEnum.MAIN_MENU:
                StartCoroutine(CanvasControl(MainMenu, target));
                break;
            case CanvasEnum.SETTINGS:
                StartCoroutine(CanvasControl(Settings, target));
                break;
            case CanvasEnum.PAUSE:
                StartCoroutine(CanvasControl(Pause, target));
                break;
        }
    }
    public void HideCanvas(CanvasEnum canvasToShow)
    {
        float target = 0.0f;
        switch (canvasToShow)
        {
            case CanvasEnum.MAIN_MENU:
                StartCoroutine(CanvasControl(MainMenu, target));
                break;
            case CanvasEnum.SETTINGS:
                StartCoroutine(CanvasControl(Settings, target));
                break;
            case CanvasEnum.PAUSE:
                StartCoroutine(CanvasControl(Pause, target));
                break;
        }
    }
    private IEnumerator CanvasControl(CanvasGroup group, float target)
    {
        if (group != null)
        {
            float startAlpha = group.alpha;
            float t = 0.0f;

            group.interactable = target >= 1.0f;
            group.blocksRaycasts = target >= 1.0f;

            while (t < AnimationTime)
            {
                t = Mathf.Clamp(t + Time.deltaTime, 0.0f, AnimationTime);
                group.alpha = Mathf.SmoothStep(startAlpha, target, t / AnimationTime);
                yield return null;
            }
        }
    }

}
