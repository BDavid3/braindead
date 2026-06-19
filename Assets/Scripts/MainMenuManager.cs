using System;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private bool _isInProcess;
    [SerializeField] private Button hostLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button quitButton;
    public static MainMenuManager Instance { get; private set; }

    public bool IsInProcess
    {
        get => _isInProcess;
        set
        {
            _isInProcess = value;
            
            hostLobbyButton.interactable = !_isInProcess;
            joinLobbyButton.interactable = !_isInProcess;
            quitButton.interactable = !_isInProcess;
        }
    }

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
        hostLobbyButton.onClick.AddListener(OnHostLobbyButtonClick);
        joinLobbyButton.onClick.AddListener(OnJoinLobbyButtonClick);
        quitButton.onClick.AddListener(OnQuitLobbyButtonClick);
    }

    void OnHostLobbyButtonClick()
    {
        SteamLobbyManager.Instance.HostLobby();
    }

    void OnJoinLobbyButtonClick()
    {
        SteamLobbyManager.Instance.JoinLobby();
        // show List
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
        // Make sure to destroy when changing Scene

        hostLobbyButton.onClick.RemoveAllListeners();
        joinLobbyButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }
    
}
