using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum EnemyState { IDLE ,WALK, ATTACK};
[ExecuteInEditMode]
public class FoxController : MonoBehaviour
{
    #region Variables
    private Rigidbody2D rb;
    private Animator m_Anim;
    
    //Fox Variables
    public bool flip;
    public bool patrol;
    public bool insideRange;

    public float walkSpeed;
    public float overlapRadius;
    public float angle;
    public float rayCastDownDistance;
    public float rayCastUpDistance;
    public float dot;

    public LayerMask mask;
    public EnemyState enemyState;

    public Vector2 direction;
    public Vector2 origin;

    public Transform target;

    public RaycastHit2D[] hitInfoDown;
    public RaycastHit2D[] hitInfoForward;
    public List<Collider2D> colliders;
    public CircleCollider2D attackRange;
    private GameManager manager;
    #endregion


    private IEnumerator FoxPatrol()
    {
        while (true)
        {
            switch (enemyState)
            {
                case EnemyState.IDLE:
                    StartCoroutine(FoxIdle());

                    break;
                case EnemyState.WALK:

                    direction = GetDirectionVector2D(angle);
                    origin = transform.position;
                    hitInfoDown = Physics2D.RaycastAll(origin, direction, rayCastDownDistance, mask);
                    hitInfoForward = Physics2D.RaycastAll(origin, transform.right, rayCastUpDistance);


                    if(hitInfoDown.Length <=0 || hitInfoDown.Length >1 || hitInfoForward.Length > 0)
                    {
                        flip = true;
                    }
                    FoxWalk();
                    break;
            }

            yield return null;
        }
    }
    public IEnumerator FoxFlip()
    {
        while (true)
        {
            if (flip == true)
            {
                patrol = false;
                transform.Rotate(Vector3.up * 180);
                walkSpeed *= -1;

                angle = 180 - angle;
                patrol = true;
                flip = false;
            }
            yield return null;
        }
    }
    public void FoxWalk()
    {
        m_Anim.SetBool("Walk", true);
        rb.velocity = (Vector2)transform.TransformDirection(Vector2.right);
    }
    private IEnumerator FoxIdle()
    {
        m_Anim.SetBool("Walk", false);
        
        yield return new WaitForSeconds(4);

        enemyState = EnemyState.WALK;
        m_Anim.SetBool("Walk", true);
    }
    private IEnumerator FoxAttack()
    {
        while (insideRange)
        {
            m_Anim.SetBool("Attack", true);
            m_Anim.SetBool("Idle", false);

            yield return new WaitForSeconds(2);
            m_Anim.SetBool("Idle", true);
            m_Anim.SetBool("Attack", false);
        }
    }


    private IEnumerator FollowPlayer()
    {
        while (insideRange)
        {
            rb.velocity = new Vector2(Vector3.Distance(transform.position, target.position) * 0.5f * Time.deltaTime, rb.velocity.y);

            //if()

            yield return null;
        }
    }
    public Vector2 GetDirectionVector2D(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        m_Anim = GetComponent<Animator>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        patrol = true;
        enemyState = EnemyState.IDLE;

        StartCoroutine(FoxPatrol());
        StartCoroutine(FoxFlip());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && attackRange.IsTouching(collision))
        {
            Debug.Log("Inside the attacking range");

            enemyState = EnemyState.ATTACK;
            m_Anim.SetBool("Walk", false);
            StartCoroutine(FoxAttack());
        }

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Follow player enter range");
            insideRange = true;
            target = collision.gameObject.transform;
            StartCoroutine(FollowPlayer());

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !attackRange.IsTouching(collision))
        {
            Debug.Log("Outside the attacking range");

            enemyState = EnemyState.WALK;
            m_Anim.SetBool("Walk", true);
            StopCoroutine(FoxAttack());
        }

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Follow player exit range");
            insideRange = false;
            target = null;
            StopCoroutine(FollowPlayer());
        }
    }
    private void Update()
    {
        if (target != null)
        {
            dot = Vector3.Dot((target.position - transform.position).normalized, transform.right);
            if (dot < 0)
            {
                flip = true;
            }
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Debug.DrawRay(origin, direction * rayCastDownDistance, Color.green);
        Debug.DrawRay(transform.position, transform.right * rayCastUpDistance, Color.red);
    }
}
