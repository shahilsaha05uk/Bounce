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

    [Space(1)][Header("Canvas Bool Checks")]
    public static bool pauseCanvasOpened;

    private void OnEnable()
    {
        pauseCanvasOpened = false;

        _uiControls = new UIControls();
        _uiControls.Enable();
        _uiControls.KeyControls.PauseMenu.performed += OnPauseClick;

    }
    private void OnDisable()
    {
        _uiControls.KeyControls.PauseMenu.performed -= OnPauseClick;
        _uiControls.Disable();
    }
    private void Start()
    {
        HideCanvas(CanvasEnum.SETTINGS);
        HideCanvas(CanvasEnum.PAUSE);
        ShowCanvas(CanvasEnum.MAIN_MENU);
    }

    #region Keyboard UI Controls
    private void OnPauseClick(InputAction.CallbackContext obj)
    {
        if (SceneManage.Instance.GetCurrentScene().name != "MainMenu")
        {
            if (!pauseCanvasOpened)
            {
                SceneManage.Instance.AdditiveSceneTrigger("PauseScene", SceneStatus.LOAD);
                ShowCanvas(CanvasEnum.PAUSE);
                pauseCanvasOpened = true;
            }
            else
            {
                SceneManage.Instance.AdditiveSceneTrigger("PauseScene", SceneStatus.UNLOAD);
                HideCanvas(CanvasEnum.PAUSE);
                pauseCanvasOpened = false;
            }
        }
    }

    #endregion

    #region Button Clicks

    //Main Menu
    public void OnNewGameButtonClick()
    {
        Debug.Log("New Game");
        SceneManage.Instance.SceneChangeTrigger("Level 1");

        HideCanvas(CanvasEnum.SETTINGS);
        HideCanvas(CanvasEnum.PAUSE);
        HideCanvas(CanvasEnum.MAIN_MENU);

        pauseCanvasOpened = false;
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
        SceneManage.Instance.AdditiveSceneTrigger("PauseScene", SceneStatus.UNLOAD);
        HideCanvas(CanvasEnum.PAUSE);
        pauseCanvasOpened = false;
    }
    public void OnExitToMainMenuButtonClick()
    {

        SceneManage.Instance.AdditiveSceneTrigger("PauseScene", SceneStatus.UNLOAD);
        SceneManage.Instance.SceneChangeTrigger("MainMenu");
        ShowCanvas(CanvasEnum.MAIN_MENU);
        HideCanvas(CanvasEnum.SETTINGS);
        HideCanvas(CanvasEnum.PAUSE);
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
