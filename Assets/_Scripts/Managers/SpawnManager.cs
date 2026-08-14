using System;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] NetworkObject networkObject;
    int nextSpawnIndex;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        
        Debug.LogWarning("SpawnManger setted for onyly test");
        // Decommentare per il funzionamento non di test
        //SpawnPlayers();
    }
    
    public void SpawnPlayer(ulong id)
    {
        // Nel caso id non dovesse essere lineare
        int index = nextSpawnIndex % spawnPoints.Length;
        nextSpawnIndex++;

        NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(networkObject, id, isPlayerObject: true, position: spawnPoints[index].position,
                                                                                                           rotation: spawnPoints[index].rotation);
    }
    private void SpawnPlayers()
    {
        foreach(var client in NetworkManager.ConnectedClients)
        {
            SpawnPlayer(client.Key);
        }
    }
}