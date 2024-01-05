using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour

{
    float moveSpeed = 3f;
    float jumpHeight = 4f;
    float gravity = -9.81f;
    float velocity;
    [SerializeField] private CharacterController controller;
    public Vector3 moveDir = new Vector3();

    void Update()
    {
        Debug.Log(moveDir.x);
        if (!IsOwner) return;
        velocity += gravity * 3f * Time.deltaTime;
        moveDir.z = Input.GetAxis("Vertical") * moveSpeed; 
        moveDir.x = Input.GetAxis("Horizontal") * moveSpeed;
       // if(controller.isGrounded == false) moveDir.y = velocity;


        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            if (IsServer && IsLocalPlayer) Jump(); return;
            if (IsClient && IsLocalPlayer) JumpServerRpc();
        }

        if(moveDir.y<0 && controller.isGrounded) 
        {
            if (IsServer && IsLocalPlayer) Land(); return;
            if (IsClient && IsLocalPlayer) LandServerRpc();
        }

        if (IsServer && IsLocalPlayer)
        {
            Move(moveDir, moveSpeed);
        }

        if(IsClient && IsLocalPlayer)
        {
            MoveServerRpc(moveDir, moveSpeed);
        }
    }
    private void Move(Vector3 moveDir, float moveSpeed)
    {
        controller.Move(moveDir * Time.deltaTime);
    }
    private void Jump() 
    {
        if (controller.isGrounded) 
        {
            moveDir.y = Mathf.Sqrt(jumpHeight * -2 * velocity);
        }
    }
    private void Land()
    {
        velocity = 0;
    }
    [ServerRpc]
    private void MoveServerRpc(Vector3 moveDir, float moveSpeed)
    {
        Move(moveDir, moveSpeed);
    }
    [ServerRpc]
    private void JumpServerRpc()
    {
        Jump();
    }
    [ServerRpc]
    private void LandServerRpc()
    {
        Land();
    }
}
