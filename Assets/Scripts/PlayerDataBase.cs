using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;

public class PlayerDataBase : NetworkBehaviour
{
    public readonly SyncVar<string> PlayerName   = new(new SyncTypeSettings(WritePermission.ServerOnly, ReadPermission.Observers));
    public readonly SyncVar<ulong>  PlayerSteamID = new(new SyncTypeSettings(WritePermission.ServerOnly, ReadPermission.Observers));
    public readonly SyncVar<bool>   IsHost        = new(new SyncTypeSettings(WritePermission.ServerOnly, ReadPermission.Observers));
    
    [Server]
    public void Initialize(string name, ulong steamId, bool host)
    {
        PlayerName.Value = name;
        PlayerSteamID.Value = steamId;
        IsHost.Value = host;
    }

}
