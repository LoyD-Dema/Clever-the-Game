using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using UnityEngine.EventSystems;

public class LobbyUI : MonoBehaviour
{
    // Parents GO
    private GameObject currentParent;
    [SerializeField] private GameObject mainMenuParent;
    [SerializeField] private GameObject lobbyParent;

    [Header("MainMenu")]
    // For lobby
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField codeInputField;

    // Username
    [SerializeField] private Button editNameButton;
    [SerializeField] private TMP_InputField nameInputField;


    [Header("Lobby")]
    [SerializeField] TextMeshProUGUI lobbyNameTMPro;
    [SerializeField] TextMeshProUGUI numOfPlayersTMPro;
    [SerializeField] Button leaveButton;
    [SerializeField] TMP_InputField lobbyCodeTMPro;

    [SerializeField] Transform playerLobbyTemplate;
    [SerializeField] Transform playersContainer;


    private void OnEnable()
    {
        // Host button
        hostButton.onClick.AddListener(LobbyManager.I.CreateLobby);

        // Join button
        joinButton.onClick.AddListener(LobbyManager.I.JoinLobby);

        // Code InputField
        codeInputField.onSubmit.AddListener(LobbyManager.I.SetCode);
        codeInputField.onDeselect.AddListener(LobbyManager.I.SetCode);

        // Edit Name button
        editNameButton.onClick.AddListener(EditUsername);

        // Username InputField
        nameInputField.onSubmit.AddListener(ResetNameInputField);
        nameInputField.onDeselect.AddListener(ResetNameInputField);

        // Leave button
        leaveButton.onClick.AddListener(LobbyManager.I.LeaveLobby);


        // LobbyManager
        LobbyManager.I.OnLobbyCreatedOrJoined += LobbyManager_OnLobbyCreatedOrJoined;
        LobbyManager.I.OnLobbyUpdate += LobbyManager_OnLobbyUpdate;
        LobbyManager.I.OnLeave += LobbyManager_OnLeave;

    }    



    private void OnDisable()
    {
        hostButton.onClick.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        // Setupping parents
        currentParent = lobbyParent;
        SwitchParent();

        nameInputField.text = "Usarname_" + Random.Range(0, 1000);
        LobbyManager.I.ChangeUsername(nameInputField.text);
        ServicesManager.I.Authenticate(nameInputField.text);
    }

    private void SwitchParent()
    {
        // Switch the parent group to show
        currentParent.SetActive(false);
        currentParent = currentParent == mainMenuParent ? lobbyParent : mainMenuParent;
        currentParent.SetActive(true);
    }

    private void EditUsername()
    {
        if (EventSystem.current.currentSelectedGameObject == nameInputField)
            return;
     
        nameInputField.interactable = true;
        nameInputField.Select();
        nameInputField.ActivateInputField();
    }

    private void ResetNameInputField(string text)
    {
        nameInputField.interactable = false;
        editNameButton.Select();
        LobbyManager.I.ChangeUsername(nameInputField.text);
    }


    // LobbyManager
    private void LobbyManager_OnLobbyCreatedOrJoined(Lobby lobby)
    {
        lobbyNameTMPro.text = lobby.Name;
        numOfPlayersTMPro.text = $"{lobby.Players.Count} / {lobby.MaxPlayers}";
        lobbyCodeTMPro.text = $"{lobby.LobbyCode}";
        
        SwitchParent();
    }

    private void LobbyManager_OnLobbyUpdate(Lobby lobby)
    {
        numOfPlayersTMPro.text = $"{lobby.Players.Count} / {lobby.MaxPlayers}";

        ClearLobby();
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Data[LobbyManager.USERNAME_KEY].Value);
            Transform playerSingleTransform = Instantiate(playerLobbyTemplate, playersContainer);
            playerSingleTransform.gameObject.SetActive(true);

            if(AuthenticationService.Instance.PlayerId == player.Id)
            {
                Color color = Color.green;
                color.a = 0.2f;
                playerSingleTransform.GetComponent<Image>().color = color;
            }

            playerSingleTransform.GetComponentInChildren<TextMeshProUGUI>().text = player.Data[LobbyManager.USERNAME_KEY].Value;
        }

    }

    private void LobbyManager_OnLeave(LeaveType type)
    {
        SwitchParent();
        Debug.Log("Left cuz of " + type);
    }

    private void ClearLobby()
    {
        foreach (Transform child in playersContainer)
        {
            if (child == playersContainer) continue;
            Destroy(child.gameObject);
        }
    }
}
