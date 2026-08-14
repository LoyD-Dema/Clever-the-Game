using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class TEST_StartConnection : NetworkBehaviour
{

    private RelayManager relayManager;
    private ServicesManager servicesManager;
    private SpawnManager spawnManager;
    private bool isHost;

    [SerializeField] private string joinCode;


    private void Awake()
    {
        relayManager = GetComponent<RelayManager>();
        servicesManager = GetComponent<ServicesManager>();
        spawnManager = GetComponent<SpawnManager>();
    }

    private void Start()
    {
        Authenticate();
    }

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsHost)
            NetworkManager.Singleton.OnClientConnectedCallback += (ulong id) => spawnManager.SpawnPlayer(id);
    }


    public async void CreateRelay()
    {
        string code = await relayManager.CreateRelay(4);
        Debug.Log("JOIN CODE: " + code);
        isHost = true;
    }

    public void JoinRelay()
    {
        relayManager.JoinRelay(joinCode);
        isHost = false;
    }

    public void Authenticate()
    {
        servicesManager.Authenticate("Username_" + Random.Range(0, 1000));
        Debug.Log("Authentication done");
    }
}

[CustomEditor(typeof(TEST_StartConnection))]
public class StartConnectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Start Host (relay)"))
        {
            TEST_StartConnection startConnection = (TEST_StartConnection)target;
            Debug.Log("Creating...");
            startConnection.CreateRelay();

        }
        else if (GUILayout.Button("Start Client (relay)"))
        {
            TEST_StartConnection startConnection = (TEST_StartConnection)target;
            Debug.Log("Joining...");
            startConnection.JoinRelay();
            Debug.Log("Joined");
        }
    }
}
