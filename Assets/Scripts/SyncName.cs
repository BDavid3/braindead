using System;
using FishNet.Object;
using Steamworks;
using TMPro;
using UnityEngine;

public class SyncName : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if (IsOwner)
        {
            // Only the owner runs this
            string myName = SteamFriends.GetPersonaName();
            SendNameToServer(myName);
        }
    }

    [ServerRpc] 
    void SendNameToServer(string playerName)
    {
        SyncNameToAll(playerName);
    }

    [ObserversRpc] 
    void SyncNameToAll(string playerName)
    {
        playerNameText.text = playerName;
    }
}
