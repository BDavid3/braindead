using System.Collections.Generic;
using Steamworks;

// Local LobbyData
public class LobbyData
{
    public static LobbyData Instance { get; set; }
    
    public CSteamID LobbyID;
    public bool IsPublic;
}
