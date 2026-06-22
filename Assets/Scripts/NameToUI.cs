using TMPro;
using UnityEngine;

public class NameToUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerUIText;

    public void Setup(string playerName)
    {
        playerUIText.text = playerName;
    }
}
