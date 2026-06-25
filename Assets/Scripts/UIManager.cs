using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Steamworks;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
   public static UIManager Instance { get; private set; }
   
   public MainMenuUI mainMenuUI;
   public ServerListUI serverListUI;
   public LobbyUI lobbyUI;
   public AllPanels panels;

   public enum CurrentState
   {
      MainMenu,
      ServersList,
      Lobby
   }
   
   private Dictionary<CurrentState, GameObject> _currentPanelDic;

   private void Awake()
   {
      if (Instance != null && Instance != this)
      {
         Destroy(gameObject);
         return;
      }
      Instance = this;
      
      _currentPanelDic = new Dictionary<CurrentState, GameObject>
      {
         { CurrentState.MainMenu, panels.mainMenu },
         { CurrentState.ServersList, panels.serversList },
         { CurrentState.Lobby, panels.lobby}
      };
   }

   private void OnEnable()
   {
      mainMenuUI.hostLobbyButton.onClick.AddListener(OnHostLobbyButtonClicked);
      mainMenuUI.joinLobbyButton.onClick.AddListener(OnJoinLobbyButtonClicked);
      mainMenuUI.serverListButton.onClick.AddListener(() => SwitchState(CurrentState.ServersList));
      mainMenuUI.quitLobbyButton.onClick.AddListener(() => Application.Quit());
      
      serverListUI.backButton.onClick.AddListener(() => SwitchState(CurrentState.MainMenu));
      serverListUI.refreshButton.onClick.AddListener(OnRefreshButtonClicked);

      lobbyUI.copyLobbyCodeButton.onClick.AddListener(() => GUIUtility.systemCopyBuffer = SteamLobbyManager.Instance.TempLobbyID.ToString()); 
      lobbyUI.exitLobbyButton.onClick.AddListener(OnExitLobbyButtonClicked);
      lobbyUI.privacyButton.onClick.AddListener(OnPrivacyButtonClicked);
      lobbyUI.startGameButton.onClick.AddListener(OnStartGameButtonClicked);
      
   }

   private void OnDisable()
   {
      mainMenuUI.hostLobbyButton.onClick.RemoveAllListeners();
      mainMenuUI.joinLobbyButton.onClick.RemoveAllListeners();
      mainMenuUI.serverListButton.onClick.RemoveAllListeners();
      mainMenuUI.quitLobbyButton.onClick.RemoveAllListeners();
      
      serverListUI.backButton.onClick.RemoveAllListeners();
      serverListUI.refreshButton.onClick.RemoveAllListeners();
      
      lobbyUI.copyLobbyCodeButton.onClick.RemoveAllListeners();
      lobbyUI.exitLobbyButton.onClick.RemoveAllListeners();
      lobbyUI.privacyButton.onClick.RemoveAllListeners();
      lobbyUI.startGameButton.onClick.RemoveAllListeners();
   }

   private void Start()
   {
      SwitchState(CurrentState.MainMenu);
   }

   public void SwitchState(CurrentState whichState)
   {
      foreach (var (key,value) in _currentPanelDic)
      {
         value.SetActive(false);
      }
      
      foreach (var (key,value) in _currentPanelDic)
      {
         if (key == whichState)
         {
            value.SetActive(true);
            return;
         }
      }
   }

   void OnHostLobbyButtonClicked()
   {
      SteamLobbyManager.Instance.HostLobby();
   }

   void OnJoinLobbyButtonClicked()
   {
      string code = mainMenuUI.joinInputField.text.Trim();
      if (string.IsNullOrEmpty(code))
      {
         Debug.LogWarning("Enter a lobby code first.");
         return;
      }
      SteamLobbyManager.Instance.JoinLobby(code);
   }

   void OnExitLobbyButtonClicked()
   {
      SteamLobbyManager.Instance.LeaveLobby();
   }

   void OnRefreshButtonClicked()
   {
      
   }

   void OnPrivacyButtonClicked()
   {
      
   }

   void OnStartGameButtonClicked()
   {
      
   }
}

[Serializable]
public class AllPanels
{
   public GameObject mainMenu;
   public GameObject serversList;
   public GameObject lobby;
}

[Serializable]
public class MainMenuUI
{
   public Button serverListButton;
   public Button hostLobbyButton;
   public Button joinLobbyButton;
   public Button quitLobbyButton;
   public TMP_Text mainMenuTitle;
   public TMP_InputField joinInputField;
}

[Serializable]
public class ServerListUI
{
   public Button refreshButton;
   public Button backButton;
   public TMP_Text refreshTitle;
   public GameObject serversListPlace;
}

[Serializable]
public class LobbyUI
{
   public Button startGameButton;
   public Button privacyButton;
   public Button exitLobbyButton;
   public Button copyLobbyCodeButton;
   public TMP_Text lobbyTitle;
   public TMP_Text lobbyCode;
   public GameObject playersListPlace;
}
