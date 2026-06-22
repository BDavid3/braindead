using FishNet.Managing;
using Steamworks;
using UnityEngine;

[RequireComponent(typeof(SteamworksInitializer))]
public class SteamLobbyManager : MonoBehaviour
{
    private Callback<LobbyCreated_t> _onLobbyCreated;
    private Callback<GameLobbyJoinRequested_t> _onJoinRequested;
    private Callback<LobbyEnter_t> _onLobbyEntered;

    private const int MaxPlayers = 8;
    
    [SerializeField] private NetworkManager fishNetNetworkManager;
    public static SteamLobbyManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    private void OnEnable()
    {
        _onLobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        _onJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequested);
        _onLobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        MainMenuManager.OnHostLobbyRequested += HostLobby;
        MainMenuManager.OnJoinLobbyRequested += JoinLobby;
        MainMenuManager.OnExitLobbyRequested += LeaveLobby;
    }

    private void OnDisable()
    {
        _onLobbyCreated.Dispose();
        _onJoinRequested.Dispose();
        _onLobbyEntered.Dispose();
        
        MainMenuManager.OnHostLobbyRequested -= HostLobby;
        MainMenuManager.OnJoinLobbyRequested -= JoinLobby;
        MainMenuManager.OnExitLobbyRequested -= LeaveLobby;
    }
    
    void HostLobby()
    {
        LobbyPlayerData.Instance.IsHost = true;
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, MaxPlayers);
    }

    void JoinLobby(string lobbyID)
    {
        LobbyPlayerData.Instance.IsHost = false;
        if (LobbyData.Instance.LobbyID.ToString() == lobbyID)
        {
            SteamMatchmaking.JoinLobby(LobbyData.Instance.LobbyID);
        }
        Debug.LogError("Incorrect lobby code!");
    }
    
    void LeaveLobby()
    {
        if (LobbyData.Instance.LobbyID.IsValid())
        {
            SteamMatchmaking.LeaveLobby(LobbyData.Instance.LobbyID);
            LobbyData.Instance.LobbyID = CSteamID.Nil;
        }
        
        if (LobbyPlayerData.Instance.IsHost)
        {
            fishNetNetworkManager.ServerManager.StopConnection(true);
        }
        fishNetNetworkManager.ClientManager.StopConnection();
        LobbyPlayerData.Instance.IsHost = false;
        MainMenuManager.Instance.ShowCurrentPanel(MainMenuManager.MenuState.MainMenu, false);
    }

    void OnJoinRequested(GameLobbyJoinRequested_t result)
    {
        LobbyPlayerData.Instance.IsHost = false;
        LobbyData.Instance.LobbyID = result.m_steamIDLobby;
        SteamMatchmaking.JoinLobby(LobbyData.Instance.LobbyID);
        
        Debug.Log($"Joined lobby: {LobbyData.Instance.LobbyID}");
    }
    
    
    void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError($"Steam Lobby creation failed: {result.m_eResult}");
            return;
        }
        
        LobbyData.Instance.LobbyID = new CSteamID(result.m_ulSteamIDLobby);

        SteamMatchmaking.SetLobbyData(LobbyData.Instance.LobbyID,"Lobby","InLobby");
        Debug.Log($"Lobby created: {LobbyData.Instance.LobbyID}");

        fishNetNetworkManager.ServerManager.StartConnection();
        fishNetNetworkManager.ClientManager.StartConnection();
    }
    
    void OnLobbyEntered(LobbyEnter_t result)
    {
        if (LobbyPlayerData.Instance.IsHost)
        {
            LobbyPlayerData.Instance.HostSteamID = SteamMatchmaking.GetLobbyOwner(LobbyData.Instance.LobbyID);
            LobbyPlayerData.Instance.PlayerSteamID = SteamMatchmaking.GetLobbyOwner(LobbyData.Instance.LobbyID);
            LobbyPlayerData.Instance.PlayerNickName = SteamFriends.GetFriendPersonaName(LobbyPlayerData.Instance.HostSteamID);
            LobbyData.Instance.IsPublic = false;
            
            MainMenuManager.Instance.ShowCurrentPanel(MainMenuManager.MenuState.MainMenu, true);
            LobbyPlayerCard.Instance.SetupPlayerCard();
            
            Debug.Log("Entered your own lobby.");
            return;
        }
        
        LobbyPlayerData.Instance.HostSteamID = SteamMatchmaking.GetLobbyOwner(LobbyData.Instance.LobbyID); 
        LobbyPlayerData.Instance.PlayerSteamID = SteamUser.GetSteamID();
        LobbyPlayerData.Instance.PlayerNickName = SteamFriends.GetFriendPersonaName(LobbyPlayerData.Instance.PlayerSteamID);
        
        SetFishySteamworksTargetId(LobbyPlayerData.Instance.HostSteamID.m_SteamID.ToString());
        fishNetNetworkManager.ClientManager.StartConnection();
        
        MainMenuManager.Instance.ShowCurrentPanel(MainMenuManager.MenuState.Lobby, false);
        LobbyPlayerCard.Instance.SetupPlayerCard();
        Debug.Log($"Player joined Host: {LobbyPlayerData.Instance.HostSteamID}, Lobby: {LobbyData.Instance.LobbyID}");
    }
        
    void SetFishySteamworksTargetId(string hostSteamID)
    {
        var transport = fishNetNetworkManager.GetComponent<FishySteamworks.FishySteamworks>();
        
        if (transport != null)
        {
            transport.SetClientAddress(hostSteamID);
            return;
        }
        Debug.LogError("Could not find FishySteamworks transport. " + "Ensure it is on the same GameObject as the NetworkManager.");
    }

    private void OnApplicationQuit()
    {
        LeaveLobby();
    }
}
