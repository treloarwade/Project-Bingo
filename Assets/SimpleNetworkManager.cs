using Unity.Netcode;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

public class SimpleNetworkManager : MonoBehaviour
{
    private string hostIP = "192.168.1.245";  // Host IP (this PC)
    private string myIP;

    void Start()
    {
        StartHosting();
    }

    // Get the local IP address of this machine
    private string GetLocalIPAddress()
    {
        string localIP = "";
        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.OperationalStatus == OperationalStatus.Up)
            {
                foreach (var ipAddress in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (ipAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ipAddress.Address.ToString();
                        break;
                    }
                }
            }
        }
        return localIP;
    }

    // Start hosting the game on this machine
    private void StartHosting()
    {
        // Start hosting using Unity Netcode
        NetworkManager.Singleton.StartHost();
    }

    // Connect to the host machine as a client
    private void ConnectAsClient()
    {
        // Connect to the host using the IP address
        NetworkManager.Singleton.StartClient();
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(hostIP); // Store host IP for client
    }

    // Callback to handle when a client connects
    public void OnClientConnected(ulong clientId)
    {
        Debug.Log("A client has connected with ID: " + clientId);
    }

    // Callback to handle when the client disconnects
    public void OnClientDisconnected(ulong clientId)
    {
        Debug.Log("Client with ID " + clientId + " has disconnected.");
    }
}
