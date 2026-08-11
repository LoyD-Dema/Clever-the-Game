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
        Debug.Log("SpawnManager spawned!");

        if (!IsServer)
            return;

        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        foreach(var client in NetworkManager.ConnectedClients)
        {
            // Nel caso id non dovesse essere lineare
            int index = nextSpawnIndex % spawnPoints.Length;
            nextSpawnIndex++;

            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(networkObject, client.Key, isPlayerObject: true, position: spawnPoints[index].position,
                                                                                                                       rotation: spawnPoints[index].rotation);
        }
    }
}