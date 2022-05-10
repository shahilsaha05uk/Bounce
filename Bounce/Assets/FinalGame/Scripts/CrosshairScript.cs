using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairScript : MonoBehaviour
{
    public bool isTouchingEnemies;
    public GameObject enemy;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            isTouchingEnemies = true;
            enemy = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
        {
            isTouchingEnemies = false;
            enemy = null;
        }
    }
}
