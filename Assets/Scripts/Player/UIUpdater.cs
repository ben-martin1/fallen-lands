using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;

public class UIUpdater : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoTextMesh;
    [SerializeField] private TextMeshProUGUI hpTextMesh;

    void Start()
    {
        /*if (IsOwner)
        {
            ammoTextMesh = GameObject.Find("Canvas/ammo/ammoText").GetComponent<TextMeshProUGUI>();
            ammoTextMesh = GameObject.Find("Canvas/status/hpText").GetComponent<TextMeshProUGUI>();
        }*/
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) gameObject.SetActive(false);
    }
    public void UpdateAmmoUI(int currentAmmo, int maxAmmo)
    {
        ammoTextMesh.text = currentAmmo + " / " + maxAmmo;
    }
}
