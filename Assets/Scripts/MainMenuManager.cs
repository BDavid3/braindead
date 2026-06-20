using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button serverListButton;
    [SerializeField] private Button hostLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button quitButton;
    
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject serverListPanel;
    [SerializeField] private GameObject optionsPanel;
    
    [SerializeField] private TMP_InputField joinInputField;
    
    public TMP_InputField JoinInputField => joinInputField;
    public GameObject LobbyPanel  => lobbyPanel;
    
    public static MainMenuManager Instance { get; private set; }
    private List<GameObject> Panels => new List<GameObject> {mainMenuPanel, lobbyPanel, serverListPanel, optionsPanel};
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
    
    private void Start()
    {
        StartListening();
    }

    void OnServerListButtonClick()
    {
        StopListening();
        OnlyCurrentPanel(serverListPanel);
    }
    

    void OnHostLobbyButtonClick()
    {
        StopListening();
        SteamLobbyManager.Instance.HostLobby();
    }

    void OnJoinLobbyButtonClick()
    {
        SteamLobbyManager.Instance.JoinLobby();
    }

    void OnQuitLobbyButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        StopListening();
    }
    
    void StopListening()
    {
        serverListButton.onClick.RemoveAllListeners();
        hostLobbyButton.onClick.RemoveAllListeners();
        joinLobbyButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }

     public void StartListening()
    {
        serverListButton.onClick.AddListener(OnServerListButtonClick);
        hostLobbyButton.onClick.AddListener(OnHostLobbyButtonClick);
        joinLobbyButton.onClick.AddListener(OnJoinLobbyButtonClick);
        quitButton.onClick.AddListener(OnQuitLobbyButtonClick);
    }

    public void OnlyCurrentPanel(GameObject whichNotPanel)
    {
        foreach (var item in Panels)
        {
            item.SetActive(true);
        }
        foreach (var item in Panels)
        {
            if  (item != whichNotPanel)
            {
                item.SetActive(false);
            }
        }
    }

}
