using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : NetworkBehaviour
{
    private int totalPlayers;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        NetworkManager.Singleton.OnClientConnectedCallback += ChangeScene;
    }

    public void OnDisable()
    {
        if (!IsServer)
            return;

        NetworkManager.Singleton.OnClientConnectedCallback += ChangeScene;
    }

    private void ChangeScene(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClients.Count == totalPlayers)
        {
            SceneHandler.LoadScene(SceneType.PlayScene);
        }
    }

    public async Task<string> CreateRelay(int totPlayers)
    {
        totalPlayers = totPlayers;

        string joinCode = null;

        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(totPlayers - 1);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }

        return joinCode;
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }
}
