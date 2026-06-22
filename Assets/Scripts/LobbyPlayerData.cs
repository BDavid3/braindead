using Steamworks;

// Local PlayerData
public class LobbyPlayerData
{
    public static LobbyPlayerData Instance { get; set; }

    public CSteamID PlayerSteamID;
    public CSteamID HostSteamID;
    public string PlayerNickName;
    public bool IsHost;
}
