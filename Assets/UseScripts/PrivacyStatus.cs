using FishNet.Object;
using Steamworks;
using UnityEngine;

namespace UseScripts
{
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

        [ServerRpc(RequireOwnership =  false)]
        void TogglePrivacy()
        {
            if (!LobbyData.Instance.CurrentLobbyIsPublic)
            {
                SteamMatchmaking.SetLobbyType(LobbyData.Instance.CurrentLobbyID, ELobbyType.k_ELobbyTypePublic);
                LobbyData.Instance.CurrentLobbyIsPublic = true;
                
                Debug.Log(LobbyData.Instance.CurrentLobbyIsPublic);
                SyncPrivacy(true);
                return;
            }

            SteamMatchmaking.SetLobbyType(LobbyData.Instance.CurrentLobbyID, ELobbyType.k_ELobbyTypePrivate);
            LobbyData.Instance.CurrentLobbyIsPublic = false;
            Debug.Log(LobbyData.Instance.CurrentLobbyIsPublic);
            SyncPrivacy(false);
        }

        [ObserversRpc]
        void SyncPrivacy(bool isPublic)
        {
            LobbyData.Instance.CurrentLobbyIsPublic = isPublic;
            MainMenuManager.Instance.LobbyUIObjects.privacyText.text = LobbyData.Instance.CurrentLobbyIsPublic ? "Public" : "Private";
        }

    }
}



