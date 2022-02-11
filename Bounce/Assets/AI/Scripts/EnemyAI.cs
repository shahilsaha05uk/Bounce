using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum EnemyState { IDLE ,WALK, ATTACK};

[ExecuteInEditMode]
public class EnemyAI : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator m_Anim;
    
    public bool flip;
    public bool patrol;
    public bool insideRange;

    public float walkSpeed;
    public float overlapRadius;
    public float angle;
    public float dot;

    public LayerMask mask;
    public EnemyState enemyState;

    public Vector2 direction;
    public Vector2 origin;

    public Transform target;

    public RaycastHit2D[] hitInfo;
    public List<Collider2D> colliders;
    public CircleCollider2D attackRange;

    private IEnumerator EnemyPatrol()
    {
        while (true)
        {
            switch (enemyState)
            {
                case EnemyState.IDLE:
                    StartCoroutine(Idle());

                    break;
                case EnemyState.WALK:

                    direction = GetDirectionVector2D(angle);
                    origin = transform.position;
                    hitInfo = Physics2D.RaycastAll(origin, direction, 40, mask);

                    if(hitInfo.Length <=0 || hitInfo.Length >1)
                    {
                        flip = true;
                    }
                    //flip = !Physics2D.OverlapCircle(groundCheckObject.transform.position, overlapRadius, groundLayer);

                   // Patrol();
                    break;
                case EnemyState.ATTACK:


                    break;
            }

            yield return null;
        }
    }

    public IEnumerator Flip()
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
    public void Patrol()
    {
        m_Anim.SetBool("Walk", true);
        rb.velocity = new Vector2(walkSpeed * Time.deltaTime, rb.velocity.y);
    }
    private IEnumerator Idle()
    {
        m_Anim.SetBool("Walk", false);
        
        yield return new WaitForSeconds(4);

        enemyState = EnemyState.WALK;
        m_Anim.SetBool("Walk", true);
    }
    private IEnumerator Attack()
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

        patrol = true;

        enemyState = EnemyState.IDLE;

        StartCoroutine(EnemyPatrol());
        StartCoroutine(Flip());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && attackRange.IsTouching(collision))
        {
            Debug.Log("Inside the attacking range");

            enemyState = EnemyState.ATTACK;
            m_Anim.SetBool("Walk", false);
            StartCoroutine(Attack());
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
            StopCoroutine(Attack());
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
                Flip();
            }
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Debug.DrawRay(origin, direction * 10, Color.green);
    }
}
