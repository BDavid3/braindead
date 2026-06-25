using System.Linq;
using FishNet.Managing;
using FishNet.Connection;
using FishNet.Transporting;
using FishNet.Managing.Scened;
using Steamworks;
using UnityEngine;
using FishNet;

public class SteamLobbyManager : MonoBehaviour
{
    public static SteamLobbyManager Instance { get; private set; }
    public ulong TempLobbyID { get; private set; }
    public ulong TempHostID { get; private set; }
    public string TempHostIDToStr { get; private set; }

    [SerializeField] private GameObject playerDataPrefab;
    [SerializeField] private  GameObject lobbyDataPrefab;
    
    private Callback<LobbyCreated_t> _onLobbyCreated;
    private Callback<GameLobbyJoinRequested_t> _onJoinRequested;
    private Callback<LobbyEnter_t> _onLobbyEntered;

    private NetworkManager _fishNetNetworkManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _fishNetNetworkManager = GetComponent<NetworkManager>();
    }

    private void OnEnable()
    {
        _onLobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        _onJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequested);
        _onLobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        
        _fishNetNetworkManager.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
    }

    private void OnDisable()
    {
        _onLobbyCreated.Dispose();
        _onJoinRequested.Dispose();
        _onLobbyEntered.Dispose();
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, 8);
    }

    public void JoinLobby(string lobbyID)
    {
        if (ulong.TryParse(lobbyID, out ulong id))
        {
            SteamMatchmaking.JoinLobby(new CSteamID(id));
            return;
        }
        Debug.LogError("Incorrect lobby code!");
    }

    public void LeaveLobby()
    {
        SteamMatchmaking.LeaveLobby(new CSteamID(TempLobbyID));
        
        if (_fishNetNetworkManager.IsServerStarted)
            _fishNetNetworkManager.ServerManager.StopConnection(true);
        
        _fishNetNetworkManager.ClientManager.StopConnection();
        
        UIManager.Instance.SwitchState(UIManager.CurrentState.MainMenu);
    }

    void OnJoinRequested(GameLobbyJoinRequested_t result)
    {
        SteamMatchmaking.JoinLobby(result.m_steamIDLobby);
    }

    void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError($"Steam Lobby creation failed (SKILL ISSUE): {result.m_eResult}");
            UIManager.Instance.SwitchState(UIManager.CurrentState.MainMenu);
            return;
        }
        
        TempLobbyID = result.m_ulSteamIDLobby;
        TempHostID = SteamUser.GetSteamID().m_SteamID;
        TempHostIDToStr = SteamUser.GetSteamID().m_SteamID.ToString();
        
        SetFishySteamworksTargetId(TempHostIDToStr);
        
        _fishNetNetworkManager.ServerManager.StartConnection();
        _fishNetNetworkManager.ClientManager.StartConnection();
        
        var lobbyDataGo = Instantiate(lobbyDataPrefab);
        _fishNetNetworkManager.ServerManager.Spawn(lobbyDataGo);

        SpawnPlayerDatabase(_fishNetNetworkManager.ClientManager.Connection, true);
    }
    

    void OnLobbyEntered(LobbyEnter_t result)
    {
        CSteamID mySteamID = SteamUser.GetSteamID();
        CSteamID hostSteamID = SteamMatchmaking.GetLobbyOwner((CSteamID)result.m_ulSteamIDLobby);
        
        if (mySteamID == hostSteamID)
        {
            UIManager.Instance.SwitchState(UIManager.CurrentState.Lobby);
            Debug.Log("Entered your own lobby.");
            return;
        }
        
        SetFishySteamworksTargetId(hostSteamID.m_SteamID.ToString());
        _fishNetNetworkManager.ClientManager.StartConnection();

        UIManager.Instance.SwitchState(UIManager.CurrentState.Lobby);
    }
    private void OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (conn == _fishNetNetworkManager.ClientManager.Connection)
        {
            return;    
        }
        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            SpawnPlayerDatabase(conn, isHost: false);
        }
        else if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            if (LobbyDataBase.Instance == null) return;
            
            ulong steamId = GetSteamIDForConnection(conn);
            var player = LobbyDataBase.Instance.Players
                .FirstOrDefault(p => p.PlayerSteamID.Value == steamId && !p.IsHost.Value);

            if (player != null)
                LobbyDataBase.Instance.UnRegisterPlayer(player);
        }
    }
    
    void SpawnPlayerDatabase(NetworkConnection connection, bool isHost)
    {
        ulong steamID = GetSteamIDForConnection(connection);
        string name = SteamFriends.GetFriendPersonaName((CSteamID)steamID);
        
        var playerDataGo = Instantiate(playerDataPrefab);
        _fishNetNetworkManager.ServerManager.Spawn(playerDataGo, connection);

        var playerData = playerDataGo.GetComponent<PlayerDataBase>();
        playerData.Initialize(name, steamID, isHost);
        
        LobbyDataBase.Instance.RegisterPlayer(playerData);
    }

    private ulong GetSteamIDForConnection(NetworkConnection connection)
    {
        if (connection == _fishNetNetworkManager.ClientManager.Connection)
            return SteamUser.GetSteamID().m_SteamID;
        
        CSteamID lobbyId = new CSteamID(TempLobbyID);
        int memberCount = SteamMatchmaking.GetNumLobbyMembers(lobbyId);

        for (int i = 0; i < memberCount; i++)
        {
            CSteamID memberId = SteamMatchmaking.GetLobbyMemberByIndex(lobbyId, i);
            
            if (memberId.m_SteamID == TempHostID) continue;
            
            if (LobbyDataBase.Instance.Players
                .Any(p => p.PlayerSteamID.Value == memberId.m_SteamID)) continue;

            return memberId.m_SteamID;
        }

        Debug.LogWarning("Could not determine SteamID for connection, falling back to local.");
        return SteamUser.GetSteamID().m_SteamID;
    }

    void SetFishySteamworksTargetId(string hostSteamID)
    {
        var transport = _fishNetNetworkManager.GetComponent<FishySteamworks.FishySteamworks>();
        if (transport != null)
        {
            transport.SetClientAddress(hostSteamID);
            return;
        }

        Debug.LogError("Could not find FishySteamworks transport. " +
                       "Ensure it is on the same GameObject as the NetworkManager.");
    }
    
}   