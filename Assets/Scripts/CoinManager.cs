using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CoinManager : NetworkBehaviour
{
    public static CoinManager Instance; // синглтон для удобного доступа

    [SyncVar(hook = nameof(OnCoinsChanged))]
    private int totalCoins = 0;

    private Text coinsText;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnStartClient()
    {
        // Находим UI текст только на клиенте
        if (coinsText == null)
        {
            coinsText = GameObject.Find("CoinsText").GetComponent<Text>();
            UpdateUI();
        }
    }

    [Server]
    public void AddCoin()
    {
        totalCoins++;
    }

    private void OnCoinsChanged(int oldValue, int newValue)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinsText != null)
        {
            coinsText.text = "Coins: " + totalCoins;
        }
    }
}
