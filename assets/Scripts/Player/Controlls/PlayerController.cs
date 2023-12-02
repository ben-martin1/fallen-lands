using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private Vector2 minMaxRotation;
    [SerializeField] private Transform cam;

    private float nextTimeToFire = 0f;
    [SerializeField] private float fireRate;
    [SerializeField] private float damage;
    [SerializeField] private float shotForce;
    [SerializeField] private float range = 100f;
    [SerializeField] protected int maxClipSize;
    [SerializeField] protected int currentAmmoInClip;
    [SerializeField] private GameObject hitParticles;
    [SerializeField] private ParticleSystem gunFlash;

    [SerializeField] private List<Debuff> playerAppliableDebuffs;

    private CharacterController characterController;
    private PlayerControl playerControl;
    private float cameraAngle;

    private UIUpdater uiUpdater;

    public override void OnNetworkSpawn()
    {
        CinemachineVirtualCamera cinemachineVirtual = cam.GetComponentInChildren<CinemachineVirtualCamera>();
        currentAmmoInClip = maxClipSize;

        if (IsOwner)
        {
            cinemachineVirtual.Priority = 1;
        }
        else
        {
            cinemachineVirtual.Priority = 0;
        }
    }
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerControl = new PlayerControl();
        playerControl.Enable();

        uiUpdater = GetComponentInChildren<UIUpdater>();
        uiUpdater.UpdateAmmoUI(maxClipSize, maxClipSize);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!IsOwner) return;
        Vector2 movementInput = playerControl.Player.Move.ReadValue<Vector2>();
        Vector2 lookInput = playerControl.Player.Look.ReadValue<Vector2>();

        if (IsServer && IsLocalPlayer)
        {
            MovePlayer(movementInput);
            RotatePlayer(lookInput);
            RotateCamera(lookInput);
        }
        else if (IsLocalPlayer)
        {
            PlayerMovementServerRpc(movementInput, lookInput);
            RotateCamera(lookInput);
        }
        if (playerControl.Player.Fire.inProgress)
        {
            if (Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + (1f / fireRate);
                if (IsServer && IsLocalPlayer)
                {
                    PlayerFire();
                }
                else if (IsLocalPlayer)
                {
                    LocalPlayerRaycast();
                    PlayerFireServerRpc();
                }
            }
        }
    }
    private void LocalPlayerRaycast()
    {

        if (currentAmmoInClip > 0)
        {
            gunFlash.Play();
            currentAmmoInClip--;
            uiUpdater.UpdateAmmoUI(currentAmmoInClip, maxClipSize);
            RaycastHit hit;
            if (Physics.Raycast(cam.position, cam.forward, out hit, range))
            {
                if (hit.collider != null)
                {
                    Debug.Log(hit.collider);
                }
            }
        }
    }
    private void PlayerFire()
    {
        if (currentAmmoInClip > 0)
        {
            gunFlash.Play();
            currentAmmoInClip--;
            uiUpdater.UpdateAmmoUI(currentAmmoInClip, maxClipSize);
            RaycastHit hit;
            Debug.DrawRay(cam.position, cam.forward, Color.red, 100f, false);
            if (Physics.Raycast(cam.position, cam.forward, out hit, range))
            {
                GameObject hitParticlesGO = Instantiate(hitParticles, hit.transform);
                //Destroy(hitParticlesGO, 3);
                //hitParticlesGO.GetComponent<NetworkObject>().Spawn(true);
                Damageable damageable = hit.transform.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.OnHit(this.OwnerClientId, damage, shotForce, -hit.normal);
                  if (playerAppliableDebuffs.Count > 0) damageable.AddActiveDebuffsServerRpc(playerAppliableDebuffs.ToArray());
                }
            }
        }
    }
    [ServerRpc]
    private void PlayerFireServerRpc()
    {
        if (currentAmmoInClip > 0)
        {
            gunFlash.Play();
            currentAmmoInClip--;
            uiUpdater.UpdateAmmoUI(currentAmmoInClip, maxClipSize);
            RaycastHit hit;
            Debug.DrawRay(cam.position, cam.forward, Color.red, 100f, false);
            if (Physics.Raycast(cam.position, cam.forward, out hit, range))
            {
                GameObject hitParticlesGO = Instantiate(hitParticles, hit.transform);
                hitParticlesGO.transform.parent = null;
                Damageable damageable = hit.transform.GetComponent<Damageable>();
                if (damageable != null)
                {
                    Debug.Log("PlayerServerRPC hit damageable");
                    damageable.OnHitServerRpc(this.OwnerClientId, damage, shotForce, -hit.normal);
                }
            }
        }
    }
    private void MovePlayer(Vector2 movementInput)
    {
        Vector3 movement = movementInput.x * cam.right + movementInput.y * cam.forward;
        movement.y = -9.8f * Time.deltaTime;
        characterController.Move(movement * speed * Time.deltaTime);
    }
    private void RotatePlayer(Vector2 lookInput)
    {
        transform.RotateAround(transform.position, transform.up, lookInput.x * turnSpeed * Time.deltaTime);
    }
    private void RotateCamera(Vector2 lookInput)
    {

        cameraAngle = Vector3.SignedAngle(transform.forward, cam.forward, cam.right);
        float cameraRotationAmount = lookInput.y * turnSpeed * Time.deltaTime;
        float newCameraAngle = cameraAngle - cameraRotationAmount;
        if (newCameraAngle <= minMaxRotation.x && newCameraAngle > minMaxRotation.y)
        {
            cam.RotateAround(cam.position, cam.right, -lookInput.y * turnSpeed * Time.deltaTime);
        }
    }

    [ServerRpc]
    private void PlayerMovementServerRpc(Vector2 movementInput, Vector2 lookInput)
    {
        RotatePlayer(lookInput);
        MovePlayer(movementInput);
        RotateCamera(lookInput);
    }
}
