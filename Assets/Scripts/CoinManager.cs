using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CoinManager : NetworkBehaviour
{
    public static CoinManager Instance; // �������� ��� �������� �������

    [SyncVar(hook = nameof(OnCoinsChanged))]
    private int totalCoins = 0;

    private Text coinsText;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnStartClient()
    {
        // ������� UI ����� ������ �� �������
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

    [Server]
    public int GetTotalCoins()
    {
        return totalCoins;
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
