using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SplatterScript : MonoBehaviour
{
    public float stickAntSpeed;
    public float stickFoxSpeed;
    public float defaultAntSpeed;
    public float defaultFoxSpeed;
    public GameObject enemy;
    public EnemyType enemyType;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(ReduceSpeed(other.gameObject));
            transform.SetParent(other.transform);
        }
    }
    private IEnumerator ReduceSpeed(GameObject enemy)
    {
        EnemyType type = enemy.GetComponent<EnemyTypeScript>().returnType();

        this.enemyType = type;
        this.enemy = enemy;
        
        switch (type)
        {
            case EnemyType.ANT:
                defaultAntSpeed = enemy.GetComponent<AntController>().speed;
                enemy.GetComponent<AntController>().speed = stickAntSpeed;
                break;
            case EnemyType.FOX:
                
                defaultFoxSpeed = enemy.GetComponent<FoxController>().walkSpeed;
                enemy.GetComponent<FoxController>().walkSpeed = stickFoxSpeed;
                break;
        }
        yield return null;
    }
    private void OnDestroy()
    {
        switch (enemyType)
        {
            case EnemyType.ANT:
                enemy.GetComponent<AntController>().speed = defaultAntSpeed;
                break;
            case EnemyType.FOX:
                enemy.GetComponent<FoxController>().walkSpeed = defaultFoxSpeed;
                break;
        }
        
    }
}
