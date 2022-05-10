using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;

public enum SoundSystem {PLAY, PLAY_ONE_SHOT, PAUSE, STOP, MUTE, UNMUTE}
public class GameManager : MonoBehaviour
{
    public Action sceneChangeTrigger;
    public Action levelChangeTrigger;
    public Action healthChangeTrigger;
    public Action<GameObject> ringChangeTrigger;
    public Action<GameObject> collectibleChangeTrigger;
    public Action endPointTrigger;
    public Action gameWinTrigger;
    public Action gameLoseTrigger;

    [Space(5)][Header("List Refs")] 
    public List<GameObject> ringList;
    public List<GameObject> collectibleList;
    public IDictionary<string, bool> levelStatus;
    
    [Space(5)][Header("Manager Refs")] 
    public UIScript UI;
    public LevelInfoManagerScript LevelInfo;

    
    [Header("Int Vals")] 
    public int ringsCleared;
    public int totalRings;
    public int starCount;
    public int gold;
    [Space(4)]
    public int totalLevelCount;
    
    [Space(4)]
    public int totalCollectibles;
    public int collectibleCleared;
    public int collectibleCountDivision;
    
    [Space(4)][Header("Bool Check")]
    public bool initialSetup;
    public bool endPointReached;
    public bool endPointOpened;
    
    public bool smallBallCheck;
    public bool bigBallCheck;

    [Space(1)][Header("References")]
    public GameObject m_Player;
    public GameObject playerSpawnPoint;
    public GameObject m_Crosshair;
    public CinemachineVirtualCamera playerCineCam;

    [Space(1)]
    public GameObject FirePrefab;
    public GameObject StickyPrefab;
    public GameObject ShieldPrefab;

    [Space(5)] [Header("Sounds Ref")] 
    public AudioSource BackgroundMusic;
    public AudioSource LevelMusic;
    
    
    [Space(1)][Header("Instantiated Objects")]
    public GameObject instantiatedPlayer;
    public GameObject instantiatedCrosshair;
    public GameObject instantiatedBomb;
    public GameObject instantiatedPrefab;
    public Camera MainCamera;
    public CinemachineVirtualCamera instantiatedPlayerCineCam;
    public ObjectType instantiatedPrefabType;

    public Transform lastCheckPoint;

    #region Unity Methods
    
    void Update()
    {
    }

    private void OnEnable()
    {
        levelStatus = new Dictionary<string, bool>();
        for (int i = 0; i < totalLevelCount; i++)
        {
            levelStatus.Add(new KeyValuePair<string, bool>($"Level {i+1}", false));
        }

        gold = 400;
        UI.UpdateGold(gold);
        
        initialSetup = true;
        StartCoroutine(Trigger_Register());
    }
    private void OnDisable()
    {
        Trigger_DeRegister();
    }
    

    #endregion

    #region Trigger Methods
    public IEnumerator Trigger_Register()
    {
        sceneChangeTrigger += OnSceneChange;

        while (!SceneManage.Instance.GetCurrentScene().name.Contains("Level"))
        {
            yield return null;
        }

        endPointTrigger += OnEndPointTrigger;
        ringChangeTrigger += OnRingsValueChanged;
        collectibleChangeTrigger += OnCollectibleCleared;
        gameWinTrigger += OnLevelWinTriggered;
        gameLoseTrigger += OnLevelLoseTriggered;
        levelChangeTrigger += OnLevelChangeTrigger;
        healthChangeTrigger += OnHealthChanged;
    }
    public void Trigger_DeRegister()
    {        
        sceneChangeTrigger -= OnSceneChange;
        healthChangeTrigger -= OnHealthChanged;
        ringChangeTrigger -= OnRingsValueChanged;
        gameWinTrigger -= OnLevelWinTriggered;
        levelChangeTrigger -= OnLevelChangeTrigger;
    }
    #endregion

    #region OnTriggered Methods

    public void OnLevelChangeTrigger()
    {
        endPointOpened = false;
        endPointReached = false;
        StartCoroutine(BallScaler());
    }
    public void OnEndPointTrigger()
    {
        GameObject endPoint = GameObject.FindGameObjectWithTag("EndPoint");
        Animator endPointAnim;

        if (endPoint != null)
        {
            bool b = endPoint.TryGetComponent<Animator>(out endPointAnim);
            if(b)
            {
                endPointAnim.SetBool("OpenGate", true);
                endPointOpened = true;
            }
        }
    }
    public void OnHealthChanged()
    {
        StartCoroutine(UpdateHealth());
    }
    public void OnCollectibleCleared(GameObject lastCollectibleCleared)
    {
        ScoreManager();
        Destroy(lastCollectibleCleared.gameObject);
    }
    public void OnRingsValueChanged(GameObject lastRingCleared)
    {
        int ringsVal = totalRings - ringsCleared;

        UI.UpdateRingsCount(ringsVal);
        if (totalRings - ringsCleared == 0)
        {
            endPointTrigger?.Invoke();
        }
    }
    private void OnSceneChange()
    {
        StartCoroutine(SetUp());
    }
    private void OnLevelWinTriggered()
    {
        UI.ShowCanvas(CanvasEnum.GAMEWIN);
    }
    private void OnLevelLoseTriggered()
    {
        UI.ShowCanvas(CanvasEnum.GAMELOSE);
    }
    
    #endregion

    #region Supporting Methods

    public void ListClear()
    {
        for (int i = 0; i < ringList.Count; i++)
        {
            ringList[i] = null;
        }
        for (int i = 0; i < collectibleList.Count; i++)
        {
            collectibleList[i] = null;
        }
        ringList.Clear();
        collectibleList.Clear();
    }
    public int GetLayerID(LayerMask layer)
    {
        return (int)Mathf.Log(layer.value, 2);
    }
    public IEnumerator BallScaler()
    {

        while (instantiatedPlayer == null)
        {
            yield return null;
        }

        while ((LevelInfo = GameObject.Find("LevelInfoManager").GetComponent<LevelInfoManagerScript>()) == null)
        {
            yield return null;
        }

        smallBallCheck = LevelInfo.smallBall;
        bigBallCheck = LevelInfo.bigBall;
        instantiatedPlayer.GetComponent<PlayerMovement>().BallScaler();
    }

    #endregion
    
    private IEnumerator SetUp()
    {
        // Instantiate the ball at the spawnpoint
        playerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint");
        if (playerSpawnPoint != null && instantiatedPlayer == null)
        {
            instantiatedPlayer = Instantiate(m_Player, playerSpawnPoint.transform.position, playerSpawnPoint.transform.rotation);
        }
        else if(playerSpawnPoint!= null && instantiatedPlayer!= null)
        {
            instantiatedPlayer.transform.position = playerSpawnPoint.transform.position;
        }
        while (instantiatedPlayer == null)
        {
            yield return null;
        }
        instantiatedPlayer.GetComponent<PlayerMovement>().ResetHealth();

        ScoreManager();

        ListClear();
        ringList = GameObject.FindGameObjectsWithTag("Rings").ToList();
        collectibleList = GameObject.FindGameObjectsWithTag("Collectible").ToList();
        
        for (int i = 0; i < ringList.Count; i++)
        {
            ringList[i].gameObject.GetComponent<RingScript>().EnableRing();
        }
        totalRings = ringList.Count;
        totalCollectibles = collectibleList.Count;
        ringsCleared = 0;
        collectibleCleared = 0;
        int ringsVal = totalRings - ringsCleared;

        UI.UpdateRingsCount(ringsVal);
 

        yield return null;
    }

    private void ScoreManager()
    {
        collectibleCountDivision = collectibleList.Count / 3;
        
        if (collectibleCleared >= totalCollectibles)
        {
            starCount = 3;
        }
        else if (collectibleCleared < totalCollectibles && collectibleCleared > collectibleCountDivision)
        {
            starCount = 2;
        }
        else
        {
            starCount = 1;
        }
        PlayerPrefs.SetInt("StarCount", starCount);
    }
    public void Collectible(Collider2D collider, GameObject player)
    {
        Collectibles collectibles;
        bool b = collider.gameObject.TryGetComponent<Collectibles>(out collectibles);

        switch (collectibles.GetCollectibleType())
        {
            case CollectibleType.RED_DIAMOND:

                if (b)
                {
                    Debug.Log("Red Diamond");
                }

                break;
            case CollectibleType.GREEN_DIAMOND:

                if (b)
                {
                    Debug.Log("Green Diamond");
                }

                break;
            case CollectibleType.RED_POLYGON:
                if (b)
                {
                    Debug.Log("Red Polygon");
                }

                break;
            case CollectibleType.GREEN_POLYGON:
                if (b)
                {
                    Debug.Log("Green Polygon");
                }

                break;
            case CollectibleType.SAVE_GAME:
                if (b)
                {
                    Debug.Log("Save Game");
                }

                break;
            case CollectibleType.SHIELD:
                Debug.Log("Shield");
                Destroy(collider.gameObject);
                player.GetComponent<ShieldMechanic>().Activate();
                break;
        }
    }
    public IEnumerator UpdateHealth()
    {
        int getHealth = 0;

        while (instantiatedPlayer == null)
        {
            yield return null;
        }
        getHealth = instantiatedPlayer.GetComponent<PlayerMovement>().GetHealth();

        if(getHealth<=0)
        {
            Destroy(instantiatedPlayerCineCam.gameObject);
            Destroy(instantiatedPlayer.gameObject);

            UI.UpdateHealth(getHealth);
            StartCoroutine(RestartAtCheckpoint(lastCheckPoint));
            StartCoroutine(instantiatedPlayer.GetComponent<PlayerMovement>().Explosion());
        }
        else
        {
            UI.UpdateHealth(getHealth);
        }
    }

    public IEnumerator RestartAtCheckpoint(Transform checkPoint)
    {
        Transform cp = checkPoint;
        yield return new WaitForSeconds(2);

        while (instantiatedPlayer != null)
        {
            yield return null;
        }

        if (cp == null)
        {
            instantiatedPlayer = Instantiate(m_Player, playerSpawnPoint.transform.position, playerSpawnPoint.transform.rotation);
            LevelInfo.chances--;
        }
        
        if (!endPointReached && cp != null && LevelInfo.chances >0)
        {
            instantiatedPlayer = Instantiate(m_Player, cp.transform.position, cp.transform.rotation);
            LevelInfo.chances--;
            cp = null;
        }
        
        if (LevelInfo.chances <= 0)
        {
            gameLoseTrigger?.Invoke();
            Debug.Log("Game Over");
        }
        
        UI.UpdateLives(LevelInfo.chances);
        yield return null;
    }


    #region Sound Methods

    public void SoundPlay(AudioSource source, bool playOnAwake, bool loop)
    {
        SoundManager.SetPlayOnAwake(playOnAwake);
        SoundManager.SetLoop(loop);
        SoundManager.SetupAudio(source);
        SoundManager.PlayAudio();
    }

    public void SoundPlayOneShot(AudioSource source)
    {
        SoundManager.SetupAudio(source);
        SoundManager.PlayAudioOneShot();
    }

    public void SoundStop(AudioSource source)
    {
        SoundManager.SetupAudio(source);
        SoundManager.StopAudio();
    }
    public void SoundPause(AudioSource source)
    {
        SoundManager.SetupAudio(source);
        SoundManager.PauseAudio();
    }
    public void SoundMute()
    {
        SoundManager.MuteAudio();
    }
    public void SoundUnMute()
    {
        SoundManager.UnMuteAudio();
    }
    #endregion
}
