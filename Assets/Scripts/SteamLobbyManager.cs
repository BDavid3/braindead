using System;
using System.Collections;
using FishNet;
using FishNet.Managing;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SteamworksInitializer))]
public class SteamLobbyManager : MonoBehaviour
{
    private const int MaxPlayers = 8;
    public CSteamID currentLobbyID;
    
    public bool isHost;
    public bool isPublic;
    
    private NetworkManager _fishNetManager;

    private Callback<LobbyCreated_t> _onLobbyCreated;
    private Callback<GameLobbyJoinRequested_t> _onJoinRequested;
    private Callback<LobbyEnter_t> _onLobbyEntered;
    
    public static SteamLobbyManager Instance { get; private set; }
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        if (_fishNetManager == null)
        {
            _fishNetManager = FindFirstObjectByType<NetworkManager>();
        }
        
    }
    
    public void HostLobby()
    {
        isHost = true;
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, MaxPlayers); // Send it to Steam
    }

    public void JoinLobby()
    {
        isHost = false;
        if (MainMenuManager.Instance.JoinInputField != null)
        {
            if (MainMenuManager.Instance.JoinInputField.text == currentLobbyID.ToString())
            {
                SteamMatchmaking.JoinLobby(currentLobbyID);
                return;
            }
        }
        Debug.LogError("Incorrect lobby code!");
    }
    
    
    private void OnEnable()
    {
        _onLobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        _onJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequested);
        _onLobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    private void OnDisable()
    {
        _onLobbyCreated?.Dispose();
        _onJoinRequested?.Dispose();
        _onLobbyEntered?.Dispose();
    }


    void LeaveLobby()
    {
        if (currentLobbyID.IsValid())
        {
            SteamMatchmaking.LeaveLobby(currentLobbyID);
            currentLobbyID = CSteamID.Nil;
        }
    }
    

    void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError($"Steam Lobby creation failed: {result.m_eResult}");
            MainMenuManager.Instance.DefaultMainMenuState();
            return;
        }
        
        currentLobbyID = new CSteamID(result.m_ulSteamIDLobby);

        SteamMatchmaking.SetLobbyData(currentLobbyID,"Lobby","InLobby");
        Debug.Log($"Lobby created: {currentLobbyID}");

        _fishNetManager.ServerManager.StartConnection();
        _fishNetManager.ClientManager.StartConnection();
    }

    void OnJoinRequested(GameLobbyJoinRequested_t result)
    {
        isHost = false;
        
        currentLobbyID = result.m_steamIDLobby;
        SteamMatchmaking.JoinLobby(currentLobbyID);
        
        Debug.Log($"Joined lobby: {currentLobbyID}");
    }

    void OnLobbyEntered(LobbyEnter_t result)
    {
        if (isHost)
        {
            // Only restrict if successful creation
            MainMenuManager.Instance.OnlyCurrentPanel(MainMenuManager.Instance.LobbyPanel);
            MainMenuManager.Instance.OnlyChosenButtonListen(MainMenuManager.Instance.ExitLobbyButton,
                MainMenuManager.Instance.PrivacyButton,
                MainMenuManager.Instance.ReadyUpButton,
                MainMenuManager.Instance.StartGameButton
            );
            Debug.Log("Entered your own lobby.");
            return;
        }
        
        CSteamID hostSteamID = SteamMatchmaking.GetLobbyOwner(currentLobbyID);
        Debug.Log($"Player joined Host: {hostSteamID}, Lobby: {currentLobbyID}");
        
        SetFishySteamworksTargetId(hostSteamID.m_SteamID.ToString());
        _fishNetManager.ClientManager.StartConnection();
        MainMenuManager.Instance.OnlyCurrentPanel(MainMenuManager.Instance.LobbyPanel);
        MainMenuManager.Instance.OnlyChosenButtonListen(MainMenuManager.Instance.ExitLobbyButton,
            MainMenuManager.Instance.ReadyUpButton);
    }

    public void OnLobbyLeave()
    {
        SteamMatchmaking.LeaveLobby(currentLobbyID);
        currentLobbyID = CSteamID.Nil;

        if (isHost)
        {
            _fishNetManager.ServerManager.StopConnection(true);
        }
        _fishNetManager.ClientManager.StopConnection();
        isHost = false;
        
        MainMenuManager.Instance.DefaultMainMenuState();
    }

    void SetFishySteamworksTargetId(string steamId)
    {
        var transport = _fishNetManager.GetComponent<FishySteamworks.FishySteamworks>();
        
        if (transport != null)
        {
            transport.SetClientAddress(steamId);
            return;
        }
        
        Debug.LogWarning("Could not find FishySteamworks transport. " +
                         "Ensure it is on the same GameObject as the NetworkManager.");
    }

    private void OnApplicationQuit()
    {
        OnLobbyLeave();
    }
}
