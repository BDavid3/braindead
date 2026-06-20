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
    private CSteamID _currentLobbyID;
    
    private bool _isHost;
    private NetworkManager _fishNetManager;

    private Callback<LobbyCreated_t> _onLobbyCreated;
    private Callback<GameLobbyJoinRequested_t> _onJoinRequested;
    private Callback<LobbyEnter_t> _onLobbyEntered;
    private Callback<LobbyChatUpdate_t> _onLobbyChatUpdate;
    
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
        _isHost = true;
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, MaxPlayers); // Send it to Steam
    }

    public void JoinLobby()
    {
        _isHost = false;
        if (MainMenuManager.Instance.JoinInputField != null)
        {
            if (MainMenuManager.Instance.JoinInputField.text == _currentLobbyID.ToString())
            {
                SteamMatchmaking.JoinLobby(_currentLobbyID);
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
        if (_currentLobbyID.IsValid())
        {
            SteamMatchmaking.LeaveLobby(_currentLobbyID);
            _currentLobbyID = CSteamID.Nil;
        }
    }
    

    void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError($"Steam Lobby creation failed: {result.m_eResult}");
            MainMenuManager.Instance.StartListening();
            return;
        }
        
        _currentLobbyID = new CSteamID(result.m_ulSteamIDLobby);

        SteamMatchmaking.SetLobbyData(_currentLobbyID,"Lobby","InLobby");
        Debug.Log($"Lobby created: {_currentLobbyID}");

        _fishNetManager.ServerManager.StartConnection();
        _fishNetManager.ClientManager.StartConnection();
    }

    void OnJoinRequested(GameLobbyJoinRequested_t result)
    {
        _isHost = false;
        
        _currentLobbyID = result.m_steamIDLobby;
        SteamMatchmaking.JoinLobby(_currentLobbyID);
        
        Debug.Log($"Joined lobby: {_currentLobbyID}");
    }

    void OnLobbyEntered(LobbyEnter_t result)
    {
        if (_isHost)
        {
            MainMenuManager.Instance.OnlyCurrentPanel(MainMenuManager.Instance.LobbyPanel);
            Debug.Log("Entered your own lobby.");
            return;
        }
        
        CSteamID hostSteamID = SteamMatchmaking.GetLobbyOwner(_currentLobbyID);
        Debug.Log($"Player joined Host: {hostSteamID}, Lobby: {_currentLobbyID}");
        
        SetFishySteamworksTargetId(hostSteamID.m_SteamID.ToString());
        _fishNetManager.ClientManager.StartConnection();
        MainMenuManager.Instance.OnlyCurrentPanel(MainMenuManager.Instance.LobbyPanel);
    }
    

    private void SetFishySteamworksTargetId(string steamId)
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
}
