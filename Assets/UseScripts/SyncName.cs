using System;
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

        void Awake()
        {
            _playerSpawn = MainMenuManager.Instance.LobbyUIObjects.playerPlace.GetComponent<Transform>();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            if (IsClientInitialized)
            {
                SendNameToServer(LobbyPlayerData.Instance.PlayerNickName);
            }
        }
        
        [ServerRpc(RequireOwnership = false)] 
        void SendNameToServer(string playerNickName)
        {
            CreatePlayer(playerNickName);
        }
        
        [ObserversRpc]
        void CreatePlayer(string playerNickName)
        {
            if (playerPrefab != null)
            {
                GameObject newPlayerPrefab = Instantiate(playerPrefab, _playerSpawn);
                newPlayerPrefab.name = playerNickName;
                newPlayerPrefab.GetComponentInChildren<TMP_Text>().text = playerNickName;
                return;
            }
            Debug.LogError("Prefab empty!");
        }
    }
}
