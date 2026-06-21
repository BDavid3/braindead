using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class ReadyUpSync : NetworkBehaviour
{
    public bool IsReady { get; private set; }
    public static readonly List<ReadyUpSync> Players = new();
    public override void OnStartServer()
    {
        // After Initialize, called by server
        base.OnStartServer();
        Players.Add(this);
    }
    
    public override void OnStopServer()
    {
        // Before DeInitialize, called by server
        base.OnStopServer();
        Players.Remove(this);
    }

    [ServerRpc]
    // Send message to server
    // Get if ready or Not
    public void SetReady(bool readyState)
    {
        IsReady = readyState;
        CheckReady();
    }
    
    void CheckReady()
    {
        if (Players.Count == 0) return;
        
        foreach (var player in Players)
        {
            if  (!player.IsReady)
                return;
        }
        
        Debug.Log("Everybody is Ready!");
        // Enable Start Game Button
        // Start Game
    }
}
