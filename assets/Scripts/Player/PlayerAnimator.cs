using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerAnimator : NetworkBehaviour
{
   // [SerializeField] private PlayerNetwork playerNetwork;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerNetwork playerNetwork;
    float direction;

    private void Update()
    {
        if (IsServer && IsLocalPlayer) setAnimation();
        if (IsClient && IsLocalPlayer) setAnimationServerRpc();
    }

    private void setAnimation()
    {
        direction = Mathf.Sqrt(Mathf.Pow(playerNetwork.moveDir.x, 2) + Mathf.Pow(playerNetwork.moveDir.z, 2));
        animator.SetFloat("direction", direction);
    }
    [ServerRpc]
    private void setAnimationServerRpc()
    {
        setAnimation();
    }
}
