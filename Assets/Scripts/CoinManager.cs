using System;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CoinManager : NetworkBehaviour
{
    public static CoinManager Instance;

    // Событие, которое будет вызываться на сервере при изменении общего числа монет
    public static event Action<int> OnTotalCoinsChanged;

    [SyncVar(hook = nameof(OnCoinsChanged))]
    private int totalCoins = 0;

    private Text coinsText;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnStartClient()
    {
        if (coinsText == null)
        {
            var go = GameObject.Find("CoinsText");
            if (go != null) coinsText = go.GetComponent<Text>();
            UpdateUI();
        }
    }

    [Server]
    public void AddCoin()
    {
        totalCoins++;
        // Сразу оповестим подписчиков (событие выполняется на сервере)
        OnTotalCoinsChanged?.Invoke(totalCoins);
    }

    [Server]
    public void RemoveCoin()
    {
        totalCoins = Mathf.Max(0, totalCoins - 1);
        OnTotalCoinsChanged?.Invoke(totalCoins);
    }

    [Server]
    public int GetTotalCoins()
    {
        return totalCoins;
    }

    private void OnCoinsChanged(int oldValue, int newValue)
    {
        // UI обновляем как и раньше (вызов хука сработает на клиентах/сервере)
        UpdateUI();
        // Не вызываем OnTotalCoinsChanged здесь, чтобы не дублировать вызов:
        // событие уже вызывается в серверных методах AddCoin/RemoveCoin.
    }

    private void UpdateUI()
    {
        if (coinsText != null)
        {
            coinsText.text = "Coins: " + totalCoins;
        }
    }
}
