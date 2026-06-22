using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MainMenuObjects
{
    public static MainMenuObjects Instance { get; set; }

    [Header("Buttons")]
    public Button serverListButton;
    public Button hostLobbyButton;
    public Button joinLobbyButton;
    public Button quitButton;
    
    public Button privacyButton;
    public Button exitLobbyButton;
    public Button backButton;
    public Button readyUpButton;
    public Button startGameButton;
    
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject lobbyPanel;
    public GameObject serverListPanel;
    
    [Header("InputFields")]
    public TMP_InputField joinInputField;

    [Header("Texts")]
    public TMP_Text privacyButtonText;

    [Header("Lobby Players Place")] 
    public GameObject lobbyPlayerPlace;
}
