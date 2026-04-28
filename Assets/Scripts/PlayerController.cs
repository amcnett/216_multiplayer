using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    private Vector2 moveInput;
    private Slider healthSlider;

    public float moveSpeed = 5f;
    public Material player1Material;
    public Material player2Material;

    private NetworkVariable<int> materialIndex = new NetworkVariable<int>(
        writePerm: NetworkVariableWritePermission.Server);

    private NetworkVariable<Vector3> serverPosition = new NetworkVariable<Vector3>(
        writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<int> Health = new NetworkVariable<int>( 
        100, // starting health
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Vector3 spawnPosition = new Vector3(0, 0.5f, 0);
            transform.position = spawnPosition;
            serverPosition.Value = spawnPosition;

            int playerNumber = NetworkManager.Singleton.ConnectedClientsList.Count;

            // Decide which material this player gets
            if (playerNumber == 1)
                materialIndex.Value = 0;   // Player 1
            else
                materialIndex.Value = 1;   // Player 2
        }

        // Apply immediately
        ApplyMaterial(materialIndex.Value);

        // Apply whenever it changes
        materialIndex.OnValueChanged += (oldVal, newVal) =>
        {
            ApplyMaterial(newVal);
        };

        if (IsClient)
        {
            Health.OnValueChanged += OnHealthChanged;
            healthSlider = GameObject.FindWithTag("HealthBar").GetComponent<Slider>();
            healthSlider.maxValue = Health.Value;
            healthSlider.value = Health.Value;
        }
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        // Update UI here (client-side)
        Debug.Log($"Health changed: {newValue}");
        if (IsOwner)
        {
            healthSlider.value = newValue;
        }
    }

    // Just another way of saying ServerRPC (equivalent to ServerRpc)
    // Second part controls who is allowed to call this RPC (here any client can)
    // This will allow our hazard sphere to damage the player

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void ApplyDamageServerRpc(int amount)
    {
        Health.Value -= amount;
        if (Health.Value <= 0)
        {
            Debug.Log("Someone died!");
        }
    }

    private void ApplyMaterial(int index)
    {
        var renderer = GetComponent<Renderer>();

        if (index == 0)
            renderer.material = player1Material;
        else
            renderer.material = player2Material;

    }

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
