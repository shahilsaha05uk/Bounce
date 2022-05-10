using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class StickyMechanic : Mechanics
{
    public PlayerMovement _player;
    public GameManager manager;
    public int stickyBullet;
    
    public override void Activate()
    {
        Init();
        StartCoroutine(MechanicUpdate());
    }

    protected override void Init()
    {
        _player = GetComponent<PlayerMovement>();
        manager = _player.manager;
        manager.instantiatedPrefab = manager.StickyPrefab;
        
        stickyBullet = 1;
    }

    protected override IEnumerator MechanicUpdate()
    {
        _player.EnableCrosshair(manager.StickyPrefab, 10);
        _player.playerInput.Player.Fire.performed += _player.Shoot;
        
        while (stickyBullet !=0)
        {
            _player.Aim();

            if (_player.playerInput.Player.Fire.triggered)
            {
                stickyBullet--;
            }

            yield return null;
        }
        manager.instantiatedPrefab = null;
        _player.playerInput.Player.EnableCrosshair.performed -= _player.Shoot;
        _player.DisableCrosshair();

    }
}
