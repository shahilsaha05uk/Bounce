using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Movement playerInput;
    private Rigidbody2D rb;
    public GameObject shootPrefab;
    public GameObject crosshair;
    public CircleCollider2D playerCollider;

    public bool canJump;
    public int normalSpeed;
    public int increasedSpeed;
    public Vector3 crosshairPos;

    public LayerMask jumpFloor;
    public LayerMask speedFloor;
    public LayerMask floatFloor;
    public Vector2 groundLevel;
    public static float jumpForceMultiplier;
    private RaycastHit2D hitInfo;

    [Space(1)][Header("Camera")]
    public CinemachineVirtualCamera playerCineCam;
    public bool viewPortCheck;

    [Space(1)][Header("Bool Checks")]
    public bool speedUp;
    public bool floatUp;
    public bool jumpUp;

    [Space(1)][Header("Gravity")]
    public float playerGravity;
    public float gravitiyIncrease;

    [Space(1)][Header("Jump Variables")]
    public float jumpStartDistance;
    public float jumpMaxHeight;
    public float airTime;
    public float jumpForce;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        playerInput = new Movement();
        playerInput.Enable();

        playerInput.Player.Jump.performed += Jump;
        playerInput.Player.Fire.performed += Shoot;

        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CircleCollider2D>();

        jumpForceMultiplier = 5f;
        canJump = true;

        StartCoroutine(PlayerControls());
        StartCoroutine(FloatOnWater());
    }
    private void OnDisable()
    {
        playerInput.Disable();
        playerInput.Player.Jump.performed -= Jump;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == GetLayerID(floatFloor))
        {
            rb.gravityScale = -1;
            gravitiyIncrease = rb.gravityScale;

            floatUp = true;
        }

        if (collision.CompareTag("CameraFlip"))
        {
            playerCineCam.enabled = false;
            //disable the camera follow
            //viewPortCheck = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == GetLayerID(floatFloor))
        {
            rb.gravityScale = 1;
            gravitiyIncrease = rb.gravityScale;
            floatUp = false;
        }

        if (!collision.CompareTag("CameraFlip"))
        {
            playerCineCam.enabled = true;

            //disable the camera follow
            //viewPortCheck = false;
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.layer == GetLayerID(speedFloor))
        {
            speedUp = true;
        }
        if (collision.gameObject.layer == GetLayerID(jumpFloor))
        {
            jumpUp = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer != GetLayerID(speedFloor.value))
        {
            speedUp = false;
        }
        if (collision.gameObject.layer != GetLayerID(jumpFloor.value))
        {
            jumpUp = false;
        }

    }
    private void OnBecameInvisible()
    {
        Debug.Log("Invisible");
        playerCineCam.enabled = true;

    }


    private IEnumerator PlayerControls()
    {
        

        while (true)
        {
            Movements();
            Aim();

            hitInfo = Physics2D.Raycast(transform.position, -Vector2.up, Mathf.Infinity);

            if(hitInfo.collider.CompareTag("Floor"))
            {
                groundLevel = hitInfo.point;
            }

            canJump = (airTime = Mathf.Abs(transform.position.y - groundLevel.y)) < jumpStartDistance;

            yield return null;
        }
    }
    public void Movements()
    {
        Vector2 inputMove = playerInput.Player.Move.ReadValue<Vector2>();

        if (speedUp)
        {
            rb.AddForce(Vector2.right * 20f * inputMove, ForceMode2D.Force);
        }
        else
        {
            rb.AddForce(Vector2.right * 10f * inputMove, ForceMode2D.Force);
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
            }
            else
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
        else
        {
            Debug.Log("Jumping still");
        }
    }
    public void Aim()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        crosshairPos = Camera.main.ScreenToWorldPoint(mousePos);
        crosshairPos.z = -2;

        crosshair.transform.position = crosshairPos;
    }
    public void Shoot(InputAction.CallbackContext obj)
    {
        Vector2 direction = crosshair.transform.position - transform.position;

        GameObject tempShoot = Instantiate(shootPrefab);
        tempShoot.GetComponent<Rigidbody2D>().velocity = direction * 5f;
        tempShoot.transform.position = transform.position;
        tempShoot.transform.Rotate(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        Destroy(tempShoot, 3f);
    }

    public int GetLayerID(LayerMask layer)
    {
        return (int)Mathf.Log(layer.value, 2);
    }
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

}
