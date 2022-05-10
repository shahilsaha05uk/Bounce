using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem.EnhancedTouch;

public enum CanvasEnum { MAIN_MENU, SETTINGS, PAUSE, GAMEPLAY, SHOP, LEVELS, GAMEWIN, GAMELOSE};

public class UIScript : MonoBehaviour
{
    private float AnimationTime = 0.5f;

    public CanvasGroup MainMenu;
    public CanvasGroup Settings;
    public CanvasGroup Pause;
    public CanvasGroup Gameplay;
    public CanvasGroup Shop;
    public CanvasGroup Levels;
    public CanvasGroup GameWin;
    public CanvasGroup GameLose;

    [Space(1)][Header("Gameplay Value Updates")]
    public TextMeshProUGUI GoldTextValue;
    public TextMeshProUGUI LivesTextValue;
    public TextMeshProUGUI PowerUpsTextValue;
    public TextMeshProUGUI RingsTextValue;
    public TextMeshProUGUI TimerTextValue;
    
    [Space(1)] [Header("UI Objects")]
    public Slider HealthBar;
    public Button SoundBtn;
    public Button SoundControlBtn;

    public Sprite UnmuteSprite;
    public Sprite MuteSprite;
    public UIControls _uiControls;
    private GameManager manager;
    
    [Space(1)][Header("Canvas Bool Checks")]
    public static bool pauseCanvasOpened;
    public static bool buyCanvasOpened;

    private void OnEnable()
    {
        HealthBar.wholeNumbers = true;
        HealthBar.interactable = false;
        HealthBar.minValue = 0;
        HealthBar.maxValue = 100;
        HealthBar.value = 100;
        
        pauseCanvasOpened = false;
        buyCanvasOpened = false;
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        _uiControls = new UIControls();
        _uiControls.Enable();
        EnhancedTouchSupport.Enable();
        
        _uiControls.KeyControls.PauseMenu.performed += OnPauseClick;
        _uiControls.KeyControls.BuyMenu.performed += OnBuyClick;
        _uiControls.UI.SoundControl.performed += OnSoundButtonClick;
    }
    private void OnDisable()
    {
        _uiControls.KeyControls.PauseMenu.performed -= OnPauseClick;
        _uiControls.Disable();
        EnhancedTouchSupport.Disable();
    }
    private void Start()
    {
        HideCanvas(CanvasEnum.SETTINGS);
        HideCanvas(CanvasEnum.PAUSE);
        HideCanvas(CanvasEnum.GAMEPLAY);
        HideCanvas(CanvasEnum.SHOP);
        HideCanvas(CanvasEnum.LEVELS);
        HideCanvas(CanvasEnum.GAMEWIN);
        HideCanvas(CanvasEnum.GAMELOSE);
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
                HideCanvas(CanvasEnum.GAMEPLAY);
                pauseCanvasOpened = true;
            }
            else
            {
                SceneManage.Instance.AdditiveSceneTrigger("PauseScene", SceneStatus.UNLOAD);
                HideCanvas(CanvasEnum.PAUSE);
                ShowCanvas(CanvasEnum.GAMEPLAY);
                pauseCanvasOpened = false;
            }
        }
    }
    private void OnBuyClick(InputAction.CallbackContext obj)
    {
        if (buyCanvasOpened)
        {
            
            HideCanvas(CanvasEnum.SHOP);
            ShowCanvas(CanvasEnum.GAMEPLAY);
            buyCanvasOpened = false;
        }
        else
        {
            HideCanvas(CanvasEnum.GAMEPLAY);
            ShowCanvas(CanvasEnum.SHOP);
            buyCanvasOpened = true;

        }
    }

    private void OnSoundButtonClick(InputAction.CallbackContext obj)
    {
        Sprite currentSprite = SoundBtn.GetComponent<Image>().sprite;
        
        if (currentSprite == UnmuteSprite)
        {
            SoundBtn.GetComponent<Image>().sprite = MuteSprite;
            manager.SoundMute();
        }
        else
        {
            SoundBtn.GetComponent<Image>().sprite = UnmuteSprite;
            manager.SoundUnMute();
        }
    }
    #endregion
    
    #region Button Clicks

    //Main Menu
    public void OnNewGameButtonClick()
    {
        OnLoadingLevel("Level 1");
    }
    public void OnSettingsButtonClick()
    {
        //HideCanvas(CanvasEnum.MAIN_MENU);
        ShowCanvas(CanvasEnum.SETTINGS);
    }
    public void OnLevelsButtonClick()
    {
        ShowCanvas(CanvasEnum.LEVELS);
    }
    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    // Level Menu
    public void OnLevels_BackButtonClick()
    {
        HideCanvas(CanvasEnum.LEVELS);
    }
    
    //Pause Menu
    public void OnResumeButtonClick()
    {
        SceneManage.Instance.AdditiveSceneTrigger("PauseScene", SceneStatus.UNLOAD);
        HideCanvas(CanvasEnum.PAUSE);
        ShowCanvas(CanvasEnum.GAMEPLAY);
        pauseCanvasOpened = false;
    }
    public void OnExitToMainMenuButtonClick()
    {
        manager.Trigger_DeRegister();
        SceneManage.Instance.AdditiveSceneTrigger("PauseScene", SceneStatus.UNLOAD);
        SceneManage.Instance.SceneChangeTrigger("MainMenu");
        ShowCanvas(CanvasEnum.MAIN_MENU);
        HideCanvas(CanvasEnum.SETTINGS);
        HideCanvas(CanvasEnum.GAMEPLAY);
        HideCanvas(CanvasEnum.GAMEWIN);
        HideCanvas(CanvasEnum.PAUSE);

        Destroy(manager.instantiatedPlayer.gameObject);
        Destroy(manager.instantiatedPlayerCineCam.gameObject);
    }

    // GameWin Page
    public void OnNextLevelButtonClick()
    {
        manager.endPointReached = true;

        HideCanvas(CanvasEnum.GAMEWIN);
        HideCanvas(CanvasEnum.GAMELOSE);
        GameWin.gameObject.SetActive(false);
    }
    public void OnRestartLevelButtonClick()
    {
        string levelName = SceneManager.GetActiveScene().name;
        OnLoadingLevel(levelName);
        manager.sceneChangeTrigger?.Invoke();
        HideCanvas(CanvasEnum.GAMEWIN);
        HideCanvas(CanvasEnum.GAMELOSE);
    }
   
    // Settings Page
    public void OnSoundControlButtonClick()
    {
        Sprite currentSprite = SoundControlBtn.GetComponent<Image>().sprite;
        
        if (currentSprite == UnmuteSprite)
        {
            SoundControlBtn.GetComponent<Image>().sprite = MuteSprite;
            manager.SoundMute();
        }
        else
        {
            SoundControlBtn.GetComponent<Image>().sprite = UnmuteSprite;
            manager.SoundUnMute();
        }
    }
    public void On_Settings_BackButton_Click()
    {
        HideCanvas(CanvasEnum.SETTINGS);
        ShowCanvas(CanvasEnum.MAIN_MENU);
    }

    #endregion

    #region UI Controls
    public void UpdateHealth(int val)
    {
        HealthBar.value = val;
    }
    public void UpdateRingsCount(int val)
    {
        RingsTextValue.text = val.ToString();
    }
    public void UpdateTimer(float val)
    {
        TimerTextValue.text = val.ToString();
    }

    public void UpdateGold(int val)
    {
        GoldTextValue.text = val.ToString();
    }
    public void UpdateLives(int val)
    {
        LivesTextValue.text = val.ToString();
    }

    #endregion

    public void OnLoadingLevel(string levelName)
    {
        Debug.Log("New Game");

        manager.smallBallCheck = true;
        manager.bigBallCheck = false;
        
        manager.initialSetup = true;
        manager.levelChangeTrigger?.Invoke();
        SceneManage.Instance.SceneChangeTrigger(levelName);
        manager.initialSetup = false;

        Levels.gameObject.SetActive(false);
        GameWin.gameObject.SetActive(false);

        HideCanvas(CanvasEnum.SETTINGS);
        HideCanvas(CanvasEnum.PAUSE);
        HideCanvas(CanvasEnum.MAIN_MENU);
        HideCanvas(CanvasEnum.LEVELS);
        HideCanvas(CanvasEnum.GAMEWIN);
        HideCanvas(CanvasEnum.GAMELOSE);
        ShowCanvas(CanvasEnum.GAMEPLAY);

        StartCoroutine(manager.Trigger_Register());

        pauseCanvasOpened = false;

    }
    public void ShowCanvas(CanvasEnum canvasToShow)
    {
        float target = 1.0f;
        switch (canvasToShow)
        {
            case CanvasEnum.MAIN_MENU:
                MainMenu.gameObject.SetActive(true);
                StartCoroutine(CanvasControl(MainMenu, target));
                manager.SoundPlay(manager.BackgroundMusic,false, true);
                
                break;
            case CanvasEnum.SETTINGS:
                StartCoroutine(CanvasControl(Settings, target));
                Settings.gameObject.SetActive(true);
                break;
            case CanvasEnum.PAUSE:
                Pause.gameObject.SetActive(true);
                StartCoroutine(CanvasControl(Pause, target));
                break;
            case CanvasEnum.GAMEPLAY:
                Gameplay.gameObject.SetActive(true);
                StartCoroutine(CanvasControl(Gameplay, target));
                break;
            case CanvasEnum.SHOP:
                Shop.gameObject.SetActive(true);
                StartCoroutine(CanvasControl(Shop, target));
                break;
            case CanvasEnum.LEVELS:
                Levels.gameObject.SetActive(true);
                StartCoroutine(CanvasControl(Levels, target));
                break;
            case CanvasEnum.GAMEWIN:
                GameWin.gameObject.SetActive(true);
                StartCoroutine(CanvasControl(GameWin, target));
                break;
            case CanvasEnum.GAMELOSE:
                GameLose.gameObject.SetActive(true);
                StartCoroutine(CanvasControl(GameLose, target));
                break;
        }
    }
    public void HideCanvas(CanvasEnum canvasToShow)
    {
        float target = 0.0f;
        switch (canvasToShow)
        {
            case CanvasEnum.MAIN_MENU:
                MainMenu.gameObject.SetActive(false);
                StartCoroutine(CanvasControl(MainMenu, target));
                
                manager.SoundStop(manager.BackgroundMusic);
                break;
            case CanvasEnum.SETTINGS:
                Settings.gameObject.SetActive(false);
                StartCoroutine(CanvasControl(Settings, target));
                break;
            case CanvasEnum.PAUSE:
                Pause.gameObject.SetActive(false);
                StartCoroutine(CanvasControl(Pause, target));
                break;
            case CanvasEnum.GAMEPLAY:
                Gameplay.gameObject.SetActive(false);
                StartCoroutine(CanvasControl(Gameplay, target));
                break;
            case CanvasEnum.SHOP:
                Shop.gameObject.SetActive(false);
                StartCoroutine(CanvasControl(Shop, target));
                break;
            case CanvasEnum.LEVELS:
                Levels.gameObject.SetActive(false);
                StartCoroutine(CanvasControl(Levels, target));
                break;
            case CanvasEnum.GAMEWIN:
                GameWin.gameObject.SetActive(false);
                StartCoroutine(CanvasControl(GameWin, target));
                break;
            case CanvasEnum.GAMELOSE:
                GameLose.gameObject.SetActive(false);
                StartCoroutine(CanvasControl(GameLose, target));
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
