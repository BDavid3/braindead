using System;
using FishNet.Object;
using Steamworks;
using TMPro;
using UnityEngine;

public class PrivacyStatus : NetworkBehaviour
{
    LobbyData _lobbyData;
    LobbyPlayerData _lobbyPlayerData;
    MainMenuObjects _mainMenuObjects;

    void OnEnable()
    {
        MainMenuManager.OnPrivacyButtonRequested += PrivacyButton;
    }

    void OnDisable()
    {
        MainMenuManager.OnPrivacyButtonRequested -= PrivacyButton;
    }

    void PrivacyButton()
    {
        if (!_lobbyPlayerData.IsHost) return;
        TogglePrivacy();
    }

    [ServerRpc]
    void TogglePrivacy()
    {
        if (!_lobbyData.IsPublic)
        {
            SteamMatchmaking.SetLobbyType(_lobbyData.LobbyID, ELobbyType.k_ELobbyTypePublic);
            _lobbyData.IsPublic = true;
            SyncPrivacy(true);
            return;
        }

        SteamMatchmaking.SetLobbyType(_lobbyData.LobbyID, ELobbyType.k_ELobbyTypePrivate);
        _lobbyData.IsPublic = false;
        SyncPrivacy(false);
    }

    [ObserversRpc]
    void SyncPrivacy(bool isPublic)
    {
        _lobbyData.IsPublic = isPublic;
        _mainMenuObjects.privacyButtonText.text = _lobbyData.IsPublic ? "Public" : "Private";
    }

}



