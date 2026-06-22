using System;
using FishNet.Object;
using Steamworks;
using TMPro;
using UnityEngine;

public class LobbyPlayerCard : NetworkBehaviour
{
    [SerializeField] private GameObject lobbyPlayerPrefab;
    public static LobbyPlayerCard Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void SetupName()
    {
        LobbyPlayerData.Instance.PlayerNickName = SteamFriends.GetFriendPersonaName(LobbyPlayerData.Instance.PlayerSteamID);
    }

    public void SetupPlayerCard()
    {
        SetupName();
        CreatePlayerCardOnJoin();
    }

    [ServerRpc]
    void CreatePlayerCardOnJoin()
    {
        SyncCreatedPlayerCard();
    }

    [ObserversRpc]
    void SyncCreatedPlayerCard()
    {
        GameObject lobbyPlayer = Instantiate(lobbyPlayerPrefab, MainMenuObjects.Instance.lobbyPlayerPlace.transform);
        lobbyPlayer.name = SteamFriends.GetFriendPersonaName(LobbyPlayerData.Instance.PlayerSteamID);
        lobbyPlayer.GetComponent<LobbyPlayerCard>().SetupName();
    }

}
