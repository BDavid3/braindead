using FishNet.Object;
using TMPro;
using UnityEngine;

namespace UseScripts
{
    public class SyncName : NetworkBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        private TMP_Text _playerNameText;
        private Transform _playerSpawn;
        
        public override void OnStartClient()
        {
            base.OnStartClient();
        
            if (IsOwner)
            {
                _playerNameText = GetComponentInChildren<TMP_Text>();
                _playerSpawn = MainMenuManager.Instance.LobbyUIObjects.playerPlace.GetComponent<Transform>();
                
                SendNameToServer(LobbyPlayerData.Instance.PlayerNickName);
            }
        }
        
        [ServerRpc] 
        void SendNameToServer(string playerNickName)
        {
            SyncMyName(playerNickName);
            CreatePlayer(playerNickName);
        }

        [ObserversRpc] 
        void SyncMyName(string playerNickName)
        {
            _playerNameText.text = playerNickName;
        }

        [ObserversRpc]
        void CreatePlayer(string playerNickName)
        {
            if (playerPrefab != null)
            {
                GameObject newPlayer = Instantiate(playerPrefab, _playerSpawn);
                newPlayer.name = playerNickName;
                newPlayer.GetComponentInChildren<TMP_Text>().text = playerNickName;
                return;
            }
            Debug.LogError("Prefab empty!");
        }
    }
}
