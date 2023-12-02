using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : PlayerController
{/*
    private float nextTimeToFire = 0f;
    [SerializeField] private float fireRate;
    [SerializeField] private NetworkVariable<float> damage;
    [SerializeField] private float shotForce;
    [SerializeField] private float range;
    private PlayerControl playerControl;
    void Start()
    {
        playerControl = new PlayerControl();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if (playerControl.Player.Fire.IsPressed() == true)
        {
            Debug.Log("Pressed");
            if (IsServer && IsLocalPlayer) PlayerFire();
            else if (IsLocalPlayer) PlayerFireServerRpc();
        }
    }

    private void PlayerFire()
    {
        if (Time.time >= nextTimeToFire)
        {
            Debug.Log("Firing");
            nextTimeToFire = Time.time + 1f / fireRate;
            RaycastHit hit;
            if (Physics.Raycast(camTransform.transform.position, camTransform.transform.forward, out hit, range))
            {
                Damageable damageable = hit.transform.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.OnHit(damage, shotForce, -hit.normal);
                }
            }
        }
    }
    [ServerRpc]
    private void PlayerFireServerRpc()
    {
        PlayerFire();
    }*/
}
