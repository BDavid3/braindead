using System.Collections.Generic;
using Steamworks;

namespace UseScripts
{
    public sealed class LobbyData
    {
        public static LobbyData Instance { get; private set; } =  new LobbyData();
    
        public CSteamID CurrentLobbyID { get; set; }
        public CSteamID HostID { get; set; }
        public bool CurrentLobbyIsPublic { get; set; }
        public int MaxPlayerCount { get; set; }

        private LobbyData() { }

        public void InitializeLobbyData(CSteamID lobbyID,  CSteamID hostID, bool isPublic, int maxPlayerCount)
        {
            CurrentLobbyID = lobbyID;
            HostID = hostID;
            CurrentLobbyIsPublic = isPublic;
            MaxPlayerCount = maxPlayerCount;
        }
    
        public void Clear()
        {
            CurrentLobbyID = CSteamID.Nil;
            HostID = CSteamID.Nil;
            CurrentLobbyIsPublic = false;
            MaxPlayerCount = 0;
        }
    
    }

    public class LobbyPlayerData
    {
        public static LobbyPlayerData Instance { get; private set; } = new LobbyPlayerData();
        public CSteamID PlayerSteamID { get; set; }
        public string PlayerNickName { get; set; }
        public bool IsHost { get; set; }

        private LobbyPlayerData()
        {
        }

        public void InitializeLobbyPlayerData(CSteamID playerSteamID,  string playerNick, bool isHost)
        {
            PlayerSteamID = playerSteamID;
            PlayerNickName = playerNick;
            IsHost = isHost;
        }

        public void Clear()
        {
            PlayerSteamID = CSteamID.Nil;
            PlayerNickName = null;
            IsHost = false;
        }
    }

    public class TempData
    {
        public static TempData Instance { get; private set; } = new TempData();
        public bool TempIsHost { get; set; }
    }
}