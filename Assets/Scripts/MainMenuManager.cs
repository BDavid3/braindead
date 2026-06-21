using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using System.Linq;
using UnityEngine.Events;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    private enum MenuState
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

    void ShowCurrentPanel(MenuState state)
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

    void CheckIfButtonActive()
    {
        foreach (var (button,action) in _buttonsWithActions)
        {
            if (!button.enabled)
            {
                button.onClick.RemoveListener(action);
            }
        }
    }

    void OnServerListButtonClick()
    {
        ShowCurrentPanel(MenuState.ServerList);
        CheckIfButtonActive();
    }

    void OnHostLobbyButtonClick()
    {
        ShowCurrentPanel(MenuState.Lobby);
        CheckIfButtonActive();
    }

    void OnJoinLobbyButtonClick()
    {
        ShowCurrentPanel(MenuState.Lobby);
        CheckIfButtonActive();
    }

    void OnQuitLobbyButtonClick()
    {
        Quit();
    }

    void OnExitLobbyButtonClick()
    {
        ShowCurrentPanel(MenuState.MainMenu);
        CheckIfButtonActive();
    }
    
    void OnBackButtonClick()
    {
        ShowCurrentPanel(MenuState.MainMenu);
        CheckIfButtonActive();
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
