using Unity.Netcode;
using UnityEngine;

public class InWaterScript : NetworkBehaviour
{
    [Header("References")]
    public Collider2D waterTrigger; // Assign your BoxCollider2D trigger here
    private NetworkedAppearance networkedAppearance;

    private void Awake()
    {
        // Ensure we have a trigger collider
        if (waterTrigger != null)
        {
            waterTrigger.isTrigger = true;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            networkedAppearance = GetComponent<NetworkedAppearance>();
            if (networkedAppearance == null)
            {
                Debug.LogError("NetworkedAppearance component not found!", this);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

            // Check if the entering object is a player
        var player = other.GetComponent<NetworkedAppearance>();

        if (player != null && other.CompareTag("Player") && other.GetComponent<NetworkObject>().IsOwner)
        {
            // Set the inWater state on the player's NetworkedAppearance
            player.SetInWaterState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {

        // Check if the exiting object is a player
        var player = other.GetComponent<NetworkedAppearance>();
        if (player != null && other.CompareTag("Player") && other.GetComponent<NetworkObject>().IsOwner)
        {
            // Set the inWater state on the player's NetworkedAppearance
            player.SetInWaterState(false);
        }
    }
}