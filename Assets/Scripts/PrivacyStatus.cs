using System;
using FishNet.Object;
using Steamworks;
using TMPro;
using UnityEngine;

public class PrivacyStatus : NetworkBehaviour
{
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
        if (!LobbyPlayerData.Instance.IsHost) return;
        TogglePrivacy();
    }

    [ServerRpc]
    void TogglePrivacy()
    {
        if (!LobbyData.Instance.IsPublic)
        {
            SteamMatchmaking.SetLobbyType(LobbyData.Instance.LobbyID, ELobbyType.k_ELobbyTypePublic);
            LobbyData.Instance.IsPublic = true;
            SyncPrivacy(true);
            return;
        }

        SteamMatchmaking.SetLobbyType(LobbyData.Instance.LobbyID, ELobbyType.k_ELobbyTypePrivate);
        LobbyData.Instance.IsPublic = false;
        SyncPrivacy(false);
    }

    [ObserversRpc]
    void SyncPrivacy(bool isPublic)
    {
        LobbyData.Instance.IsPublic = isPublic;
        MainMenuObjects.Instance.joinInputField.text = LobbyData.Instance.IsPublic ? "Public" : "Private";
    }

}



