using System;
using System.Collections.Generic;
using UnityEngine;

namespace UseScripts
{
    public class MainMenuManager : MonoBehaviour
    {
        public static MainMenuManager Instance { get; private set; }

        public enum MenuState
        {
            MainMenu,
            ServerList,
            Lobby
        }

        public MenuState CurrentState { get; set; }

        public static Action OnHostLobbyRequested;
        public static Action<string> OnJoinLobbyRequested;
        public static Action OnExitLobbyRequested;
        public static Action OnStartGameRequested;
        public static Action OnPrivacyButtonRequested;
        public static Action OnReadyUpButtonRequested;

        [SerializeField] private Panels panels;
        [SerializeField] private MainMenuUIObjects mainMenuUIObjects;
        [SerializeField] private ServerListUIObjects serverListUIObjects;
        [SerializeField] private LobbyUIObjects lobbyUIObjects;
        
        public LobbyUIObjects LobbyUIObjects => lobbyUIObjects;

        private Dictionary<MenuState, GameObject> _panelsAndStates;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }

            Instance = this;

            _panelsAndStates = new Dictionary<MenuState, GameObject>
            {
                { MenuState.MainMenu, panels.mainMenuPanel },
                { MenuState.ServerList, panels.serverListPanel },
                { MenuState.Lobby, panels.lobbyPanel }
            };
        }

        void Start()
        {
            CurrentState = MenuState.MainMenu;
        }
        
        void OnEnable()
        {
            mainMenuUIObjects.serverListButton.onClick.AddListener(() => SwitchState(MenuState.ServerList));
            mainMenuUIObjects.hostLobbyButton.onClick.AddListener(OnHostLobbyButtonClick);
            mainMenuUIObjects.joinLobbyButton.onClick.AddListener(OnJoinLobbyButtonClick);
            mainMenuUIObjects.quitButton.onClick.AddListener(() => Quit());
            
            serverListUIObjects.backButton.onClick.AddListener(() => SwitchState(MenuState.MainMenu));
            // serverListUIObjects.refreshButton.onClick.AddListener();
            
            lobbyUIObjects.exitLobbyButton.onClick.AddListener(OnExitLobbyButtonClick);
            lobbyUIObjects.readyUpButton.onClick.AddListener(OnReadyUpButtonClick);
            lobbyUIObjects.setPrivacyButton.onClick.AddListener(OnPrivacyButtonClick);
            lobbyUIObjects.startGameButton.onClick.AddListener(OnStartGameButtonClick);
        }

        void OnDisable()
        {
            mainMenuUIObjects.serverListButton.onClick.RemoveAllListeners();
            mainMenuUIObjects.hostLobbyButton.onClick.RemoveAllListeners();
            mainMenuUIObjects.joinLobbyButton.onClick.RemoveAllListeners();
            mainMenuUIObjects.quitButton.onClick.RemoveAllListeners();
            
            serverListUIObjects.backButton.onClick.RemoveAllListeners();
            // serverListUIObjects.refreshButton.onCLick.RemoveAllListeners();
            
            lobbyUIObjects.exitLobbyButton.onClick.RemoveAllListeners();
            lobbyUIObjects.readyUpButton.onClick.RemoveAllListeners();
            lobbyUIObjects.setPrivacyButton.onClick.RemoveAllListeners();
            lobbyUIObjects.startGameButton.onClick.RemoveAllListeners();
        }

        public void SwitchState(MenuState whichState)
        {
            foreach (var (menuState,panelGameObject) in _panelsAndStates)
            {
                panelGameObject.SetActive(menuState == whichState);
            }
            
            CurrentState = whichState;
        }

        void OnHostLobbyButtonClick()
        {
            OnHostLobbyRequested?.Invoke();
        }

        void OnJoinLobbyButtonClick()
        {
            if (mainMenuUIObjects.joinInputField.text == null || string.IsNullOrEmpty( mainMenuUIObjects.joinInputField.text))
            {
                Debug.LogError("Input field empty!");
                return;
            }
            
            OnJoinLobbyRequested?.Invoke(mainMenuUIObjects.joinInputField.text);   
        }
        
        void OnExitLobbyButtonClick()
        {
            OnExitLobbyRequested?.Invoke();
        }
        
        void OnPrivacyButtonClick()
        {
            OnPrivacyButtonRequested?.Invoke();
        }

        void OnReadyUpButtonClick()
        {
            OnReadyUpButtonRequested?.Invoke();
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
}
