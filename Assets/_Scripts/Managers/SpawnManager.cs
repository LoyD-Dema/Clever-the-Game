using Unity.Netcode;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] NetworkObject networkObject;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(networkObject, position: spawnPoints[obj].position);
    }
}
