using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    public static Action OnHostLobbyRequested;
    public static Action<string> OnJoinLobbyRequested;
    public static Action OnExitLobbyRequested;
    public static Action OnStartGameRequested;
    public static Action OnPrivacyButtonRequested;
    
    public enum MenuState
    {
        MainMenu,
        ServerList,
        Lobby
    }

    private Dictionary<MenuState, GameObject> _panelsAndState;
    private (Button button, UnityAction action)[] _buttonsWithActions;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;

        _panelsAndState = new Dictionary<MenuState, GameObject>
        {
            { MenuState.MainMenu, MainMenuObjects.Instance.mainMenuPanel },
            { MenuState.ServerList,  MainMenuObjects.Instance.serverListPanel },
            { MenuState.Lobby,  MainMenuObjects.Instance.lobbyPanel }
        };

        _buttonsWithActions = new (Button, UnityAction)[]
        {
            ( MainMenuObjects.Instance.serverListButton, OnServerListButtonClick),
            ( MainMenuObjects.Instance.hostLobbyButton, OnHostLobbyButtonClick),
            ( MainMenuObjects.Instance.joinLobbyButton, OnJoinLobbyButtonClick),
            ( MainMenuObjects.Instance.quitButton, OnQuitLobbyButtonClick),

            ( MainMenuObjects.Instance.exitLobbyButton, OnExitLobbyButtonClick),
            ( MainMenuObjects.Instance.privacyButton, OnPrivacyButtonClick),
            ( MainMenuObjects.Instance.backButton, OnBackButtonClick),
            ( MainMenuObjects.Instance.startGameButton, OnStartGameButtonClick),
        };
    }

    public void ShowCurrentPanel(MenuState state, bool isHost)
    {
        foreach (var kvp in _panelsAndState)
        {
            if (!isHost && state == MenuState.Lobby)
            {
                MainMenuObjects.Instance.startGameButton.interactable = false;
            }
            
            kvp.Value.SetActive(kvp.Key == state);
        }
    }

    void OnEnable()
    {
        foreach (var (button, action) in _buttonsWithActions)
        {
            button.onClick.AddListener(action);
        }
    }

    void OnDisable()
    {
        foreach (var (button, action) in _buttonsWithActions)
        {
            button.onClick.RemoveAllListeners();
        }
    }
    
    void OnServerListButtonClick()
    {
        ShowCurrentPanel(MenuState.ServerList, false);
    }

    void OnHostLobbyButtonClick()
    {
        ShowCurrentPanel(MenuState.Lobby, true);
        OnHostLobbyRequested?.Invoke();
    }

    void OnJoinLobbyButtonClick()
    {
        ShowCurrentPanel(MenuState.Lobby, false);
        if ( MainMenuObjects.Instance.joinInputField == null || string.IsNullOrEmpty( MainMenuObjects.Instance.joinInputField.text))
        {
            Debug.LogError("Input field empty!");
            return;
        }
        ShowCurrentPanel(MenuState.Lobby, true);
        OnJoinLobbyRequested?.Invoke( MainMenuObjects.Instance.joinInputField.text);   
    }

    void OnQuitLobbyButtonClick()
    {
        Quit();
    }

    void OnExitLobbyButtonClick()
    {
        ShowCurrentPanel(MenuState.MainMenu, false);
        OnExitLobbyRequested?.Invoke();
    }
    
    void OnBackButtonClick()
    {
        ShowCurrentPanel(MenuState.MainMenu, false);
    }

    void OnPrivacyButtonClick()
    {
        OnPrivacyButtonRequested?.Invoke();
    }

    void OnStartGameButtonClick()
    {
        OnStartGameRequested?.Invoke();
    }

    void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

}
