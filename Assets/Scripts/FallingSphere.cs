using Unity.Netcode;
using UnityEngine;

public class FallingSphere : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Only the server is allowed to destroy networked objects
        if (!IsServer)
            return;

        if (other.CompareTag("Ground"))
        {
            NetworkObject.Despawn();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {   
            other.gameObject.GetComponent<PlayerController>().ApplyDamageServerRpc(20);
        }
    }
}