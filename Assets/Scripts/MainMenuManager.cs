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
    
    public enum MenuState
    {
        MainMenu,
        ServerList,
        Lobby
    }

    private Dictionary<MenuState, GameObject> _panelsAndState;
    private (Button button, UnityAction action)[] _buttonsWithActions;

    [SerializeField] private MainMenuObjects menu;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;

        _panelsAndState = new Dictionary<MenuState, GameObject>
        {
            { MenuState.MainMenu, menu.mainMenuPanel },
            { MenuState.ServerList, menu.serverListPanel },
            { MenuState.Lobby, menu.lobbyPanel }
        };

        _buttonsWithActions = new (Button, UnityAction)[]
        {
            (menu.serverListButton, OnServerListButtonClick),
            (menu.hostLobbyButton, OnHostLobbyButtonClick),
            (menu.joinLobbyButton, OnJoinLobbyButtonClick),
            (menu.quitButton, OnQuitLobbyButtonClick),

            (menu.exitLobbyButton, OnExitLobbyButtonClick),
            (menu.privacyButton, OnPrivacyButtonClick),
            (menu.backButton, OnBackButtonClick),
            (menu.startGameButton, OnStartGameButtonClick),
            (menu.readyUpButton, OnReadyUpButtonClick)
        };
    }

    public void ShowCurrentPanel(MenuState state)
    {
        foreach (var kvp in _panelsAndState)
        {
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
        ShowCurrentPanel(MenuState.ServerList);
    }

    void OnHostLobbyButtonClick()
    {
        ShowCurrentPanel(MenuState.Lobby);
        OnHostLobbyRequested?.Invoke();
    }

    void OnJoinLobbyButtonClick()
    {
        ShowCurrentPanel(MenuState.Lobby);
        if (menu.joinInputField != null && !string.IsNullOrEmpty(menu.joinInputField.text))
        {
            OnJoinLobbyRequested?.Invoke(menu.joinInputField.text);    
        }
        Debug.LogError("Input field empty!");
    }

    void OnQuitLobbyButtonClick()
    {
        Quit();
    }

    void OnExitLobbyButtonClick()
    {
        ShowCurrentPanel(MenuState.MainMenu);
        OnExitLobbyRequested?.Invoke();
    }
    
    void OnBackButtonClick()
    {
        ShowCurrentPanel(MenuState.MainMenu);
    }

    void OnPrivacyButtonClick() { }
    
    void OnStartGameButtonClick() { }

    void OnReadyUpButtonClick() { }

    void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

}
