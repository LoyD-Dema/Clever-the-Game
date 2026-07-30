using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public enum LeaveType
{
    Left,
    Kicked,
    HostLeft,
}

public class LobbyManager : MonoBehaviour
{
    // Singleton
    public static LobbyManager I { get; private set; }

    // Events
    public event Action<Lobby> OnLobbyCreatedOrJoined;
    public event Action<Lobby> OnLobbyUpdate;
    public event Action<LeaveType> OnLeave;


    private string username;
    private string lobbyCode;

    private Lobby joinedLobby;
    public Lobby JoinedLobby { get { return joinedLobby; } private set { } }

    // Timers
    private float hearthbeatTimer;
    private float lobbyUpdateTimer;

    // Const
    private const int MAX_PLAYERS = 4;
    private const float HEARTBEAT_TIMER_MAX = 15.0f;
    private const float LOBBY_UPDATE_TIMER_MAX = 1.1f;
    public const string USERNAME_KEY = "Username";

    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(I);
    }

    private void Start()
    {
        hearthbeatTimer = HEARTBEAT_TIMER_MAX;
    }

    private void Update()
    {
        HandleLobbyHearthBeat();
        HandleLobbyPollForUpdates();
    }

    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void HandleLobbyHearthBeat()
    {
        if (!IsLobbyHost())
            return;

        hearthbeatTimer -= Time.deltaTime;
        if (hearthbeatTimer < 0.0f)
        {
            hearthbeatTimer = HEARTBEAT_TIMER_MAX;
            await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
        }
    }

    private async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby == null)
            return;

        lobbyUpdateTimer -= Time.deltaTime;
        if (lobbyUpdateTimer < 0.0f)
        {
            try
            {
                lobbyUpdateTimer = LOBBY_UPDATE_TIMER_MAX;
                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                OnLobbyUpdate?.Invoke(joinedLobby);
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.LobbyNotFound)
                {
                    joinedLobby = null;
                    OnLeave?.Invoke(LeaveType.HostLeft);
                }

                Debug.LogError(e);
            }
        }
    }


    // Services methonds
    public void ChangeUsername(string username)
    {
        this.username = username;
    }

    public void SetCode(string code)
    {
        lobbyCode = code;
    }


    // Lobby methos


    public async void CreateLobby()
    {
        try
        {
            string lobbyName = $"{username}_Lobby";

            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = true,
                Player = GetPlayer()
            };

            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYERS, lobbyOptions);
            OnLobbyCreatedOrJoined?.Invoke(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void JoinLobby()
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            OnLobbyCreatedOrJoined?.Invoke(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            // Display messages

        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby == null)
            return;

        try
        {
            if (AuthenticationService.Instance.PlayerId == joinedLobby.HostId)
            {
                // If the host leave, remove all players
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
            }
            else
            {
                // Remove just the player who left
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            }

            OnLeave?.Invoke(LeaveType.Left);
            joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            // Display messages

        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<String, PlayerDataObject>
                    {
                        {USERNAME_KEY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, username) }
                    }
        };
    }
}
