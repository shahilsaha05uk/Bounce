using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.SceneManagement;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [HideInInspector]public Movement playerInput;
    private Rigidbody2D rb;
    public Animator BlastAnim;
    [HideInInspector]public GameManager manager;
    public GameObject shootPrefab;

    public int normalSpeed;
    public int increasedSpeed;

    public LayerMask jumpFloor;
    public LayerMask speedFloor;
    public LayerMask floatFloor;
    public Vector2 groundLevel;
    private RaycastHit2D hitInfo;

    public static float jumpForceMultiplier;
    public int DestroyDuration { get; set; }

    [Space(1)][Header("Crosshair")]
    public Vector3 crosshairPos;
    public GameObject crosshair;
    public bool crosshairEnabled;
    public Vector2 pointerPos;
    
    [Space(1)][Header("Camera")]
    public bool viewPortCheck;

    [Space(1)][Header("Bool Checks")]
    public bool speedUp;
    public bool floatUp;
    public bool touchingObstacles;
    public bool touchingEnemies;
    public bool insideHighJumpArena;
    
    [Space(1)][Header("Health")]
    public int health;
    public int armour;

    [Space(1)][Header("Gravity")]
    public float playerGravity;
    public float gravitiyIncrease;

    [Space(1)][Header("Ball")]
    public Sprite smallBall;
    public Sprite bigBall;

    public Animator anim;
    public CircleCollider2D playerCollider;

    public float smallBallRadius;
    public float bigBallRadius;
    public float speedUpDuration;
    public float moveSpeed;

    Vector2 inputMove;

    [Space(1)] [Header("Jump Variables")]
    public float jumpStartDistance;
    public float jumpMaxHeight;
    public float airTime;
    public float jumpForce;
    public bool jumpUp;
    public bool canJump;
    #endregion

    public AudioClip jump;
    public AudioClip blastSound;
    public AudioClip shieldSound;
    public AudioClip glueSound;
    public AudioClip collectibleSound;
    public AudioSource audioSource;
    

    #region Unity Methods

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        audioSource = GetComponent<AudioSource>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        PlayerSetup();
        CameraSetup();
        
        if (manager.instantiatedCrosshair == null)
        {
            manager.instantiatedCrosshair = Instantiate(manager.m_Crosshair, transform.position, Quaternion.identity);
            manager.instantiatedCrosshair.SetActive(false);
        }

        StartCoroutine(PlayerControls());
        StartCoroutine(FloatOnWater());
        manager.levelChangeTrigger?.Invoke();
    }
    private void OnDisable()
    {
        playerInput.Disable();
        playerInput.Player.Jump.performed -= Jump;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == manager.GetLayerID(floatFloor) && manager.bigBallCheck && !manager.smallBallCheck)
        {
            rb.gravityScale = -1;
            gravitiyIncrease = rb.gravityScale;
            floatUp = true;
        }

        if ((collision.CompareTag("CameraFlip") && floatUp) || (collision.CompareTag("CameraFlip")))
        {
            manager.instantiatedPlayerCineCam.enabled = false;

            viewPortCheck = true;
        }
        
        if (collision.CompareTag("High_Jump"))
        {
            insideHighJumpArena = true;
            StartCoroutine(HighJump());
        }

        if (collision.CompareTag("ringTrigger") && !collision.transform.parent.GetComponent<RingScript>().RingCleared())
        {
            collision.transform.parent.GetComponent<RingScript>().DisableRing();

            GameObject ringParent = collision.transform.parent.gameObject;

            Debug.Log("Ring Parent: " + ringParent);
            manager.ringsCleared++;
            manager.ringChangeTrigger?.Invoke(ringParent);
        }

        if (collision.CompareTag("Collectible"))
        {
            //audioSource.clip = collectibleSound;
           // manager.SoundPlayOneShot(audioSource);
            GameObject collectibleParent = collision.gameObject;
            Debug.Log(collectibleParent.gameObject.name);
            manager.collectibleCleared++;

            manager.gold += 100;
            manager.UI.UpdateGold(manager.gold);

            manager.collectibleChangeTrigger?.Invoke(collectibleParent);
        }

        if (collision.CompareTag("LastCheckpoint"))
        {
            manager.lastCheckPoint = collision.gameObject.transform;
            collision.gameObject.SetActive(false);
            Debug.Log("After Position: "+ manager.lastCheckPoint.position);
            Debug.Log("After Position: "+ manager.lastCheckPoint.position);
        }
        if (collision.CompareTag("Shield"))
        {
            manager.Collectible(collision, this.gameObject);
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == manager.GetLayerID(floatFloor))
        {
            rb.gravityScale = 1;
            gravitiyIncrease = rb.gravityScale;
            floatUp = false;
        }
        if (!collision.CompareTag("High_Jump"))
        {
            insideHighJumpArena = false;
            StopCoroutine(HighJump());

        }

        if ((!collision.CompareTag("CameraFlip") && !floatUp) || (!collision.CompareTag("CameraFlip")))
        {
            manager.instantiatedPlayerCineCam.enabled = true;
            viewPortCheck = false;
        }

    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("SpeedObject"))
        {
            speedUp = true;
        }
        if (collision.gameObject.layer == manager.GetLayerID(jumpFloor))
        {
            jumpUp = true;
        }

        if (collision.collider.CompareTag("Obstacles") && !ShieldMechanic.shieldTurnedOn)
        {
            touchingObstacles = true;
            Debug.Log("Health Reduce");
            SetHealth(health - 20);
            manager.healthChangeTrigger?.Invoke();
        }

        if (collision.collider.CompareTag("Enemy") && !ShieldMechanic.shieldTurnedOn)
        {
            Debug.Log("Health Reduce");
            SetHealth(health - 40);
            manager.healthChangeTrigger?.Invoke();
        }

        if(collision.collider.CompareTag("SmallScaler"))
        {
            manager.smallBallCheck = true;
            manager.bigBallCheck = false;
            BallScaler();
        }
        if(collision.collider.CompareTag("BigScaler"))
        {
            manager.smallBallCheck = false;
            manager.bigBallCheck = true;
            BallScaler();
        }
        if (collision.collider.CompareTag("EndPoint") && manager.endPointOpened)
        {
            Debug.Log("Next Level");
            if (manager.levelStatus.Keys.Contains(SceneManager.GetActiveScene().name))
            {
                manager.levelStatus[SceneManager.GetActiveScene().name] = true;
            }
            manager.gameWinTrigger?.Invoke();
            ResetHealth();
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer != manager.GetLayerID(speedFloor.value))
        {
            speedUp = false;
        }
        if (collision.gameObject.layer != manager.GetLayerID(jumpFloor.value))
        {
            jumpUp = false;
        }

        if(!collision.collider.CompareTag("Obstacles"))
        {
            touchingObstacles = false;
        }
    }
    
    private void OnBecameInvisible()
    {
        if (viewPortCheck)
        {
            manager.instantiatedPlayerCineCam.enabled = true;
        }
    }
    private void OnBecameVisible()
    {
        if (viewPortCheck)
        {
            manager.instantiatedPlayerCineCam.enabled = false;
        }
    }
    
    #endregion

    #region Control Methods
    public IEnumerator Movements()
    {
        while (true)
        {
            Aim();
            if (speedUp)
            {
                float time = 0f;
                while ((time += Time.deltaTime) < speedUpDuration)
                {
                    inputMove = playerInput.Player.Move.ReadValue<Vector2>();

                    rb.AddForce(Vector2.right * moveSpeed*2 * inputMove * Time.deltaTime, ForceMode2D.Force);
                    
                    Debug.Log("Time: " + time);
                    yield return null;
                }
                Debug.Log("Time Up");
                speedUp = false;
            }
            else
            {
                rb.AddForce(Vector2.right * moveSpeed * inputMove, ForceMode2D.Force);
            }
            yield return null;
        }
    }
    public void Jump(InputAction.CallbackContext obj)
    {
        if (canJump && !floatUp)
        {
            if (jumpUp)
            {
                if(jumpForceMultiplier < jumpMaxHeight)
                {
                    jumpForceMultiplier += 5f;
                }
                else if(jumpForceMultiplier >= jumpMaxHeight)
                {
                    jumpForceMultiplier = jumpMaxHeight;
                }
                Debug.Log("Jump Force: " + jumpForceMultiplier);
                rb.AddForce(Vector2.up * jumpForceMultiplier, ForceMode2D.Impulse);
                audioSource.clip = jump;
                manager.SoundPlayOneShot(audioSource);

            }
            else
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                audioSource.clip = jump;
                manager.SoundPlayOneShot(audioSource);

            }
        }
        else
        {
            Debug.Log("Jumping still");
        }
    }
    public void Shoot(InputAction.CallbackContext obj)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (crosshair.activeInHierarchy && crosshair != null)
        {
            Vector3 crosshairPos = new Vector3(crosshair.transform.position.x, crosshair.transform.position.y, -5f);
            GameObject tempPrefab = Instantiate(manager.instantiatedPrefab, crosshairPos, Quaternion.identity);

            GameObject enemy = crosshair.GetComponent<CrosshairScript>().enemy;
            switch (manager.instantiatedPrefabType)
            {
                case ObjectType.SHIELD:
                    break;
                case ObjectType.FIRE:
                    if (enemy != null && touchingEnemies)
                    {
                        EnemyDamageHandler(enemy.GetComponent<EnemyTypeScript>().returnType(), enemy);
                    }

                    break;
                case ObjectType.STICKY:
                    break;
            }
            Destroy(tempPrefab, DestroyDuration);
        }
    }
    public void Aim()
    {
        if (crosshairEnabled)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //pointerPos = Mouse.current.position.ReadValue();
                crosshairPos = manager.MainCamera.ScreenToWorldPoint(pointerPos);
                crosshairPos.z = -2;

                crosshair.transform.position = crosshairPos;
            }
        }
    }

    public void TouchInput_CrosshairPos(InputAction.CallbackContext obj)
    {
        if (EnhancedTouchSupport.enabled && crosshairEnabled && !EventSystem.current.IsPointerOverGameObject())
        {
            pointerPos = Touchscreen.current.position.ReadValue();
            Debug.Log(pointerPos);
        }
    }

    #endregion

    #region Health and Armour Methods

    public int GetHealth() { return health; }
    public void SetHealth(int val) { health = val; }
    public void ResetHealth() { health = 100; manager.healthChangeTrigger?.Invoke(); }


    public int GetArmour() { return armour; }
    public void SetArmour(int val) { armour = val; }


    #endregion
    
    #region Player Methods in Different Areas

    private IEnumerator FloatOnWater()
    {
        while (true)
        {
            if (floatUp)
            {
                rb.gravityScale += Time.deltaTime * -0.2f;
                gravitiyIncrease = rb.gravityScale;
            }
            yield return null;
        }
    }
    public IEnumerator Explosion()
    {
        Animator blast = Instantiate(BlastAnim, transform.position, Quaternion.identity);
        while (blast.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        Destroy(blast.gameObject);
    }
    public IEnumerator HighJump()
    {
        while (insideHighJumpArena)
        {
            if (canJump)
            {
                yield return new WaitForSeconds(5);
                Debug.Log("Waited for 5 secs");
            }
            else
            {
                Debug.Log("Jumped before 5 secs");
            }
        }
        yield return null;
    }

    #endregion

    #region Supporting Methods

    public void EnableCrosshair()
    {
        EnhancedTouchSupport.Enable();
        crosshair = manager.instantiatedCrosshair;
        crosshair.SetActive(true);
        crosshairEnabled = true;
    }
    public void EnableCrosshair(GameObject prefab, int DestroyDuration)
    {
        EnhancedTouchSupport.Enable();
        playerInput.Player.EnableCrosshair.performed += TouchInput_CrosshairPos;

        crosshair = manager.instantiatedCrosshair;

        crosshair.SetActive(true);
        crosshairEnabled = true;
        manager.instantiatedPrefab = prefab;
        this.DestroyDuration = DestroyDuration;
    }
    public void DisableCrosshair()
    {
        EnhancedTouchSupport.Disable();
        playerInput.Player.EnableCrosshair.performed -= TouchInput_CrosshairPos;
        crosshair.SetActive(false);
        crosshairEnabled = false;
        manager.instantiatedPrefab = null;
    }
    public void BallScaler()
    {
        //Small Ball
        if (manager.smallBallCheck == true && manager.bigBallCheck == false)
        {
            //Debug.Log("Small Ball");
            GetComponent<SpriteRenderer>().sprite = smallBall;
            playerCollider.radius = smallBallRadius;
            jumpStartDistance = transform.localScale.y;

            manager.smallBallCheck = true;
            manager.bigBallCheck = false;
            anim.SetBool("isSmall", true);

        }
        //Big Ball
        else if (manager.smallBallCheck == false && manager.bigBallCheck == true)
        {
            Debug.Log("Big Ball");
            GetComponent<SpriteRenderer>().sprite = bigBall;
            playerCollider.radius = bigBallRadius;

            jumpStartDistance = transform.localScale.y;

            manager.smallBallCheck = false;
            manager.bigBallCheck = true;
            anim.SetBool("isSmall", false);

        }
    }
    
    #endregion
    
    
    public void PlayerSetup()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = new Movement();
        playerCollider = GetComponent<CircleCollider2D>();

        health = 100;
        armour = 100;
        jumpForceMultiplier = 5f;
        canJump = true;

        manager.healthChangeTrigger?.Invoke();
        playerInput.Enable();

        playerInput.Player.Jump.performed += Jump;
        playerInput.Player.EnableCrosshair.performed += TouchInput_CrosshairPos;
    }
    public void CameraSetup()
    {
        manager.instantiatedPlayerCineCam = Instantiate(manager.playerCineCam);
        manager.instantiatedPlayerCineCam.Follow = this.transform;
        manager.instantiatedPlayerCineCam.LookAt = this.transform;
    }
    
    public IEnumerator ReadInput()
    {
        while (true)
        {
            inputMove = playerInput.Player.Move.ReadValue<Vector2>();
            yield return null;
        }
    }
    private IEnumerator PlayerControls()
    {
        StartCoroutine(ReadInput());
        StartCoroutine(Movements());
        while (true)
        {
            Aim();
            hitInfo = Physics2D.Raycast(transform.position, -Vector2.up, Mathf.Infinity);
            if (crosshair != null && crosshair.activeInHierarchy)
            {
                touchingEnemies = crosshair.GetComponent<CrosshairScript>().isTouchingEnemies;
                Debug.Log("Crosshair touching ENEMIES");
            }

            if (hitInfo.collider.CompareTag("Floor"))
            {
                groundLevel = hitInfo.point;
            }
            canJump = ((airTime = Mathf.Abs(transform.position.y - groundLevel.y)) < jumpStartDistance) || (touchingObstacles);
            yield return null;
        }
    }
    private void EnemyDamageHandler(EnemyType type, GameObject enemy)
    {
        switch (type)
        {
            case EnemyType.ANT:
                AntController ant = enemy.GetComponent<AntController>();
                ant.TakeDamage(30);
                break;
            case EnemyType.FOX:
                FoxController fox = enemy.GetComponent<FoxController>();
                fox.TakeDamage(15);
                break;
        }
    }

}   
