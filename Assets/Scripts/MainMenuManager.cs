using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using System.Linq;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button serverListButton;
    [SerializeField] private Button hostLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button quitButton;
    
    [SerializeField] private Button privacyButton;
    [SerializeField] private Button exitLobbyButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button readyUpButton;
    [SerializeField] private Button startGameButton;
    
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject serverListPanel;
    
    [SerializeField] private TMP_InputField joinInputField;
    
    public TMP_InputField JoinInputField => joinInputField;
    private TMP_Text ReadyButtonText => readyUpButton.GetComponentInChildren<TMP_Text>();
    
    public GameObject LobbyPanel  => lobbyPanel;
    
    public Button ServerListButton => serverListButton;
    public Button HostLobbyButton => hostLobbyButton;
    public Button JoinLobbyButton => joinLobbyButton;
    public Button QuitLobbyButton => quitButton;
    public Button PrivacyButton => privacyButton;
    public Button ExitLobbyButton => exitLobbyButton;
    public Button BackButton => backButton;
    public  Button ReadyUpButton => readyUpButton;
    public  Button StartGameButton => startGameButton;

    public bool isReady;
    
    public static MainMenuManager Instance { get; private set; }
    private List<GameObject> Panels => new List<GameObject> {mainMenuPanel, lobbyPanel, serverListPanel};
    private List<Button> Buttons => new List<Button> { serverListButton, hostLobbyButton, joinLobbyButton, quitButton , privacyButton , exitLobbyButton , backButton, startGameButton, readyUpButton };
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
    
    public void OnEnable()
    {
        serverListButton.onClick.AddListener(OnServerListButtonClick);
        hostLobbyButton.onClick.AddListener(OnHostLobbyButtonClick);
        joinLobbyButton.onClick.AddListener(OnJoinLobbyButtonClick);
        quitButton.onClick.AddListener(OnQuitLobbyButtonClick);
        
        exitLobbyButton.onClick.AddListener(OnExitLobbyButtonClick);
        privacyButton.onClick.AddListener(OnPrivacyButtonClick);
        backButton.onClick.AddListener(DefaultMainMenuState);
        startGameButton.onClick.AddListener(OnStartGameButtonClick);
        readyUpButton.onClick.AddListener(OnReadyUpButtonClick);
        
    }
    private void OnDestroy()
    {
        StopListeningGoTrough();
    }
    
    
    void OnServerListButtonClick()
    {
        StopListeningGoTrough();
        OnlyCurrentPanel(serverListPanel);
    }

    void OnHostLobbyButtonClick()
    {
        StopListeningGoTrough();
        SteamLobbyManager.Instance.HostLobby();
    }

    void OnJoinLobbyButtonClick()
    {
        StopListeningGoTrough();
        SteamLobbyManager.Instance.JoinLobby();
    }

    void OnQuitLobbyButtonClick()
    {
        StopListeningGoTrough();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnExitLobbyButtonClick()
    {
        SteamLobbyManager.Instance.OnLobbyLeave();
    }

    void OnPrivacyButtonClick()
    {
        if (!SteamLobbyManager.Instance.isHost)
        {
            Debug.Log("You are not the host!");
            return;
        }

        if (SteamLobbyManager.Instance.isPublic)
        {
            SteamMatchmaking.SetLobbyType(SteamLobbyManager.Instance.currentLobbyID, ELobbyType.k_ELobbyTypePrivate);
            return;
        }
        
        SteamMatchmaking.SetLobbyType(SteamLobbyManager.Instance.currentLobbyID, ELobbyType.k_ELobbyTypePublic);
        SteamLobbyManager.Instance.isPublic = true;

    }

    void OnStartGameButtonClick()
    {
        // check if host && if everyone is ready
    }   

    void OnReadyUpButtonClick()
    {
        if (isReady)
        {
            isReady = false;
            ReadyButtonText.text = "READY UP";
        }
        isReady = true;
        ReadyButtonText.text = "READY";
    }


    public void DefaultMainMenuState()
    {
        OnEnable();
        OnlyCurrentPanel(mainMenuPanel);
    }
    
    void StopListeningGoTrough()
    {
        foreach (var item in Buttons)
        {
            item.onClick.RemoveAllListeners();
        }
    }
    
    public void OnlyCurrentPanel(GameObject panelToShow)
    {
        foreach (var item in Panels)
        {
            item.SetActive(true);
        }
        
        foreach (var item in Panels)
        {
            if  (item != panelToShow)
            {
                item.SetActive(false);
            }
        }
    }

    public void OnlyChosenButtonListen(params Button[] buttons)
    {
        foreach (var item in Buttons)
        {
            if (!buttons.Contains(item))
            {
                item.onClick.RemoveAllListeners();
                // item.gameObject.SetActive(false); -> risky
            }
        }
    }
    
}
