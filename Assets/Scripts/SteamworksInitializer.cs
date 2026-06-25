using System;
using UnityEngine;
using Steamworks;

public class SteamworksInitializer : MonoBehaviour
{
    public static bool SteamReady { get; private set; }

    private void Awake()
    {
        if (SteamReady) return;

        if (!Packsize.Test())
        {
            Debug.LogError("[Steamworks] Pack size test failed. Wrong Steamworks.NET build.");
            return;
        }

        if (!DllCheck.Test())
        {
            Debug.LogError("[Steamworks] DLL check failed. Ensure steam_api64.dll is present.");
            return;
        }

        try
        {
            if (!SteamAPI.Init())
            {
                Debug.LogError("[Steamworks] SteamAPI.Init() failed. " +
                               "Is Steam running? Is steam_appid.txt correct?");
                return;
            }
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError($"[Steamworks] steam_api64.dll not found: {e.Message}");
            return;
        }
        
        SteamReady = true;
        Debug.Log($"[Steamworks] Initialized. Logged in as: {SteamFriends.GetPersonaName()}");
    }

    private void Update()
    {
        if (SteamReady)
        {
            SteamAPI.RunCallbacks();
        }
    }

    private void OnDestroy()
    {
        SteamAPI.Shutdown();
        SteamReady = false;
    }
}
