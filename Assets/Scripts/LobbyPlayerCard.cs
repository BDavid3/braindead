using System;
using FishNet.Object;
using Steamworks;
using TMPro;
using UnityEngine;

public class LobbyPlayerCard : NetworkBehaviour
{
    [SerializeField] private GameObject lobbyPlayerPrefab;
    public static LobbyPlayerCard Instance { get; private set; }

    LobbyPlayerData _lobbyPlayerData;
    MainMenuObjects _mainMenuObjects;

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
        _lobbyPlayerData.PlayerNickName = SteamFriends.GetFriendPersonaName(_lobbyPlayerData.PlayerSteamID);
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
        GameObject lobbyPlayer = Instantiate(lobbyPlayerPrefab, _mainMenuObjects.lobbyPlayerPlace.transform);
        lobbyPlayer.name = SteamFriends.GetFriendPersonaName(_lobbyPlayerData.PlayerSteamID);
        lobbyPlayer.GetComponent<LobbyPlayerCard>().SetupName();
    }

}
