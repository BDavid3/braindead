using FishNet.Managing;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;

namespace UseScripts
{
    [RequireComponent(typeof(SteamworksInitializer))]
    [RequireComponent(typeof(NetworkManager))]
    public class SteamLobbyManager : MonoBehaviour
    {
        public static SteamLobbyManager Instance { get; private set; }
        
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
            TempData.Instance.TempIsHost = true;
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, 8);
        }

        void JoinLobby(string lobbyID)
        {
            if (ulong.TryParse(lobbyID, out ulong id))
            {   
                TempData.Instance.TempIsHost = false;
                SteamMatchmaking.JoinLobby(new CSteamID(id));
                return;
            }
            Debug.LogError("Incorrect lobby code!");
        }
    
        void LeaveLobby()
        {
            if (LobbyData.Instance.CurrentLobbyID.IsValid())
            {
                SteamMatchmaking.LeaveLobby(LobbyData.Instance.CurrentLobbyID);
                LobbyData.Instance.Clear();
            }
            LobbyPlayerData.Instance.Clear();
            
            if (LobbyPlayerData.Instance.IsHost)
            {
                _fishNetNetworkManager.ServerManager.StopConnection(true);
            }
            else
            {
                _fishNetNetworkManager.ClientManager.StopConnection();    
            }
            
            MainMenuManager.Instance.SwitchState(MainMenuManager.MenuState.MainMenu);
        }

        void OnJoinRequested(GameLobbyJoinRequested_t result)
        {
            LobbyPlayerData.Instance.IsHost = false;
            SteamMatchmaking.JoinLobby(result.m_steamIDLobby);
        }
        
        void OnLobbyCreated(LobbyCreated_t result)
        {
            if (result.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogError($"Steam Lobby creation failed: {result.m_eResult}");
                LobbyData.Instance.Clear();
                LobbyPlayerData.Instance.Clear();
                MainMenuManager.Instance.SwitchState(MainMenuManager.MenuState.MainMenu);
                return;
            }
            
            LobbyData.Instance.InitializeLobbyData(new CSteamID(result.m_ulSteamIDLobby), SteamMatchmaking.GetLobbyOwner(LobbyData.Instance.CurrentLobbyID), false,8);
            LobbyPlayerData.Instance.InitializeLobbyPlayerData(SteamUser.GetSteamID(), SteamFriends.GetPersonaName(), true);
            
            SteamMatchmaking.SetLobbyData(LobbyData.Instance.CurrentLobbyID,"Lobby","InLobby");
            Debug.Log($"Lobby created: {LobbyData.Instance.CurrentLobbyID}");

            _fishNetNetworkManager.ServerManager.StartConnection();
            _fishNetNetworkManager.ClientManager.StartConnection();
        }
    
        void OnLobbyEntered(LobbyEnter_t result)
        {
            if (LobbyPlayerData.Instance.IsHost)
            {
                MainMenuManager.Instance.SwitchState(MainMenuManager.MenuState.Lobby);
                Debug.Log("Entered your own lobby.");
                return;
            }
            
            LobbyPlayerData.Instance.InitializeLobbyPlayerData(SteamUser.GetSteamID(), SteamFriends.GetPersonaName(), false);
        
            SetFishySteamworksTargetId(LobbyData.Instance.HostID.m_SteamID.ToString());
            _fishNetNetworkManager.ClientManager.StartConnection();
        
            MainMenuManager.Instance.SwitchState(MainMenuManager.MenuState.Lobby);
            Debug.Log($"Player joined Host: {LobbyData.Instance.HostID}, Lobby: {LobbyData.Instance.CurrentLobbyID}");
        }
        
        void SetFishySteamworksTargetId(string hostSteamID)
        {
            var transport = _fishNetNetworkManager.GetComponent<FishySteamworks.FishySteamworks>();
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
}
