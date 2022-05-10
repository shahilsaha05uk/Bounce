using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMechanic : Mechanics
{    
    public float shieldTimer;
    public float timer = 0;
    private PlayerMovement _playerScript;
    public GameObject _shield;
    public GameManager manager;
    public static bool shieldTurnedOn;

    public override void Activate()
    {

        if (timer > 0f)
        {
            timer = 0;
            StartCoroutine(MechanicUpdate());
        }
        else
        {
            Init();
            StartCoroutine(MechanicUpdate());
        }

    }

    protected override void Init()
    {
        _playerScript = GetComponent<PlayerMovement>();
        manager = _playerScript.manager;
    }

    protected override IEnumerator MechanicUpdate()
    {
        _shield.SetActive(true);
        shieldTurnedOn = true;
        while (timer <shieldTimer)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("Timer: " + timer);
            manager.UI.UpdateTimer(timer);
            timer++;
        }
        
        manager.instantiatedPrefab = null;
        timer = 0f;
        shieldTurnedOn = false;
        _shield.SetActive(false);
        yield return null;
    }
}
