using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
public enum EnemyPatrolAxis { VERTICAL, HORIZONTAL };

[ExecuteInEditMode]
public class AntController : MonoBehaviour
{
    [Space(1)] [Header("Ant Health")]
    public int health;

    [Space(1)] [Header("Ant Variables")]
    public Vector3 antPos;
    public float speed;
    public EnemyPatrolAxis patrolAxis;
    private Rigidbody2D rb;

    public Vector3 direction;
    public Vector3 origin;
    public float distance;
    RaycastHit2D hitInfo;

    public IEnumerator AntPatrol(EnemyPatrolAxis axis)
    {
        yield return new WaitForSeconds(1);
        switch (axis)
        {
            case EnemyPatrolAxis.VERTICAL:
                rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                rb.freezeRotation = true;
                AntFlip();

                while (true)
                {
                    Vector3 newPos = transform.position;
                    newPos.y = speed * Time.deltaTime;
                    rb.velocity = newPos;
                    yield return null;
                }
            case EnemyPatrolAxis.HORIZONTAL:
                rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                rb.freezeRotation = true;

                StartCoroutine(DirectionHandler());
                while (true)
                {
                    direction = transform.position;
                    direction.x = speed * Time.deltaTime;
                    rb.velocity = direction;
                    yield return null;
                }

                break;

        }
        yield return null;
    }

    public IEnumerator DirectionHandler()
    {
        while (true)
        {
            origin = transform.position;
            direction = Vector2.down;

            hitInfo = Physics2D.Raycast(transform.position, direction, distance);
            Debug.DrawRay(transform.position, direction * distance, Color.red);
            if (hitInfo.collider == null)
            {
                Debug.Log("Direction change");
                AntFlip();
            }
            yield return null;

        }
    }
    public void AntFlip()
    {
        speed *= -1;
    }
    public void ChangeDirection()
    {
        direction = -direction;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = 100;
        StartCoroutine(AntPatrol(patrolAxis));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {        
        AntFlip();
        Debug.Log("Flip the ant");
    }

    public void TakeDamage(int val)
    {
        if ((health - val) <= 0)
        {
            val = 0;
            health = 0;
        }
        else
        {
            health -= val;
        }
        
        Debug.Log("Health: " + health);
        CheckDestroy();
    }
    public void CheckDestroy()
    {
        if (health == 0)
            Destroy(this.gameObject);
    }

}
