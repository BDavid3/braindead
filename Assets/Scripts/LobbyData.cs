using System.Collections.Generic;
using Steamworks;

// Local LobbyData
public class LobbyData
{
    public static LobbyData Instance { get; set; } =  new LobbyData();
    
    public CSteamID LobbyID;
    public bool IsPublic;
}
