using FishNet.Managing;
using Steamworks;
using UnityEngine;

[RequireComponent(typeof(SteamworksInitializer))]
public class SteamLobbyManager : MonoBehaviour
{
    public LobbyPlayerData LobbyPlayerData;
    public LobbyData LobbyData;
    
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
        LobbyPlayerData.IsHost = true;
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, MaxPlayers);
    }

    void JoinLobby(string lobbyID)
    {
        LobbyPlayerData.IsHost = false;
        if (LobbyData.LobbyID.ToString() == lobbyID)
        {
            SteamMatchmaking.JoinLobby(LobbyData.LobbyID);
        }
        Debug.LogError("Incorrect lobby code!");
    }
    
    void LeaveLobby()
    {
        if (LobbyData.LobbyID.IsValid())
        {
            SteamMatchmaking.LeaveLobby(LobbyData.LobbyID);
            LobbyData.LobbyID = CSteamID.Nil;
        }
        
        if (LobbyPlayerData.IsHost)
        {
            fishNetNetworkManager.ServerManager.StopConnection(true);
        }
        fishNetNetworkManager.ClientManager.StopConnection();
        LobbyPlayerData.IsHost = false;
        MainMenuManager.Instance.ShowCurrentPanel(MainMenuManager.MenuState.MainMenu, false);
    }

    void OnJoinRequested(GameLobbyJoinRequested_t result)
    {
        LobbyPlayerData.IsHost = false;
        LobbyData.LobbyID = result.m_steamIDLobby;
        SteamMatchmaking.JoinLobby(LobbyData.LobbyID);
        
        Debug.Log($"Joined lobby: {LobbyData.LobbyID}");
    }
    
    
    void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError($"Steam Lobby creation failed: {result.m_eResult}");
            return;
        }
        
        LobbyData.LobbyID = new CSteamID(result.m_ulSteamIDLobby);

        SteamMatchmaking.SetLobbyData(LobbyData.LobbyID,"Lobby","InLobby");
        Debug.Log($"Lobby created: {LobbyData.LobbyID}");

        fishNetNetworkManager.ServerManager.StartConnection();
        fishNetNetworkManager.ClientManager.StartConnection();
    }
    
    void OnLobbyEntered(LobbyEnter_t result)
    {
        if (LobbyPlayerData.IsHost)
        {
            LobbyPlayerData.HostSteamID = SteamMatchmaking.GetLobbyOwner(LobbyData.LobbyID);
            LobbyPlayerData.PlayerSteamID = LobbyPlayerData.HostSteamID;
            LobbyPlayerData.PlayerNickName = SteamFriends.GetFriendPersonaName(LobbyPlayerData.HostSteamID);
            LobbyData.IsPublic = false;
            
            MainMenuManager.Instance.ShowCurrentPanel(MainMenuManager.MenuState.MainMenu, true);
            LobbyPlayerCard.Instance.SetupPlayerCard();
            
            Debug.Log("Entered your own lobby.");
            return;
        }
        
        LobbyPlayerData.HostSteamID = SteamMatchmaking.GetLobbyOwner(LobbyData.LobbyID); 
        LobbyPlayerData.PlayerSteamID = SteamUser.GetSteamID();
        LobbyPlayerData.PlayerNickName = SteamFriends.GetFriendPersonaName(LobbyPlayerData.PlayerSteamID);
        
        SetFishySteamworksTargetId(LobbyPlayerData.HostSteamID.m_SteamID.ToString());
        fishNetNetworkManager.ClientManager.StartConnection();
        
        MainMenuManager.Instance.ShowCurrentPanel(MainMenuManager.MenuState.Lobby, false);
        LobbyPlayerCard.Instance.SetupPlayerCard();
        Debug.Log($"Player joined Host: {LobbyPlayerData.HostSteamID}, Lobby: {LobbyData.LobbyID}");
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
