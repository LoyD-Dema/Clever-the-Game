using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public async Task<string> CreateRelay(int totPlayers)
    {
        string joinCode = null;

        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(totPlayers - 1);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
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
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }
}
