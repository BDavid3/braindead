using System.Collections.Generic;
using Steamworks;
using UnityEngine;

// Local LobbyData
public class LobbyData
{
    public CSteamID LobbyID;
    public bool IsPublic;
    public Dictionary<CSteamID, LobbyPlayerData> LobbyIDAndPlayerData = new Dictionary<CSteamID,LobbyPlayerData>();
}
