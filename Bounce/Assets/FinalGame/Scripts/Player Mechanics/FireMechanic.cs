using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FireMechanic : Mechanics
{
   // public GameObject crosshair;
    public PlayerMovement _player;
    public GameManager manager;
    public int bullets;

    public override void Activate()
    {
        Init();
        StartCoroutine(MechanicUpdate());
    }
    protected override void Init()
    {
        _player = GetComponent<PlayerMovement>();
        manager = _player.manager;
        manager.instantiatedPrefab = manager.FirePrefab;
        bullets = 4;
        _player.DestroyDuration = 3;
    }

    protected override IEnumerator MechanicUpdate()
    {        
        _player.EnableCrosshair();
        _player.playerInput.Player.Fire.performed += _player.Shoot;
        
        while (bullets !=0)
        {
            _player.Aim();

            if (_player.playerInput.Player.Fire.triggered)
            {
                bullets--;
            }

            yield return null;
        }

        manager.instantiatedPrefab = null;
        _player.playerInput.Player.EnableCrosshair.performed -= _player.Shoot;
        _player.DisableCrosshair();
    }
}
