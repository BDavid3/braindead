using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class LobbyDataBase : NetworkBehaviour
{
    public static LobbyDataBase Instance { get; private set; }
    public readonly SyncList<PlayerDataBase> Players = new();

    public override void OnStartServer()
    {
        base.OnStartServer();
        Instance = this;
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        Instance = this;
    }
    
    [Server]
    public void RegisterPlayer(PlayerDataBase db)
    {
        Players.Add(db);
    }
    
    [Server]
    public void UnRegisterPlayer(PlayerDataBase db)
    {
        Players.Remove(db);
    }
}
