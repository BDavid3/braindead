using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UseScripts
{
    [System.Serializable]
    public sealed class Panels
    {
        public GameObject mainMenuPanel;
        public GameObject serverListPanel;
        public GameObject lobbyPanel;
    }

    [System.Serializable]
    public sealed class MainMenuUIObjects
    {
        public Button serverListButton;
        public Button hostLobbyButton;
        public Button joinLobbyButton;
        public Button quitButton;
        public TMP_InputField joinInputField;
    }

    [System.Serializable]
    public sealed class ServerListUIObjects
    {
        public Button backButton;
        public Button refreshButton;
        public GameObject listPlace;
    }

    [System.Serializable]
    public sealed class LobbyUIObjects
    {
        public Button exitLobbyButton;
        public Button setPrivacyButton;
        public Button readyUpButton;
        public Button startGameButton;
        public Button copyCodeButton;
        public GameObject playerPlace;
        public TMP_Text privacyText;
        public TMP_Text readyUpText;
        public TMP_Text lobbyCode;
    }
}
