using System;
using UnityEngine;

public class AddPlayerCard : MonoBehaviour
{
    public static AddPlayerCard Instance { get; private set; }

    [SerializeField] private GameObject lobbyCardPrefab;
    [SerializeField] private GameObject playerPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddPlayerCardMethod(string playerName)
    {
        var card = Instantiate(playerPrefab, lobbyCardPrefab.transform);
        card.GetComponent<NameToUI>().Setup(playerName);
    }

}
