using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerCoins : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnCoinsChanged))]
    private int coins = 0;

    public Text coinsText; // UI-ссылка (счётчик в левом верхнем углу)

    private void Start()
    {
        if (isLocalPlayer)
        {
            // Находим UI-счётчик монеток в сцене
            coinsText = GameObject.Find("CoinsText").GetComponent<Text>();
            UpdateCoinsUI();
        }
    }

    // Добавление монетки
    [Server]
    public void AddCoin()
    {
        coins++;
    }

    // Хук Mirror, срабатывает у всех клиентов, когда coins меняется
    private void OnCoinsChanged(int oldValue, int newValue)
    {
        UpdateCoinsUI();
    }

    private void UpdateCoinsUI()
    {
        if (isLocalPlayer && coinsText != null)
        {
            coinsText.text = "Coins: " + coins;
        }
    }
}
