using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    private Vector2 moveInput;
    private NetworkVariable<Vector3> serverPosition = new NetworkVariable<Vector3>(
        writePerm: NetworkVariableWritePermission.Server
    );

    public float moveSpeed = 5f;


    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            if (moveInput.sqrMagnitude > 0.001f)
            {
                SendMovementRequestServerRpc(moveInput);
            }
        }
        //Vector3 move = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Time.deltaTime;
        ///transform.Translate(move);
        transform.position = serverPosition.Value; // going to pull from network variable
    }

    [ServerRpc]
    void SendMovementRequestServerRpc(Vector2 input)
    {
        Vector3 move = new Vector3(input.x, 0, input.y) * moveSpeed * Time.deltaTime;
        serverPosition.Value += move;
    }
}
