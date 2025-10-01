using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerCoins : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnCoinsChanged))]
    private int coins = 0;

    public Text coinsText; // UI-������ (������� � ����� ������� ����)

    private void Start()
    {
        if (isLocalPlayer)
        {
            // ������� UI-������� ������� � �����
            coinsText = GameObject.Find("CoinsText").GetComponent<Text>();
            UpdateCoinsUI();
        }
    }

    // ���������� �������
    [Server]
    public void AddCoin()
    {
        coins++;
    }

    // ��� Mirror, ����������� � ���� ��������, ����� coins ��������
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
