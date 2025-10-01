using UnityEngine;
using Mirror;

public class CoinPickup : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer) return;

        if (other.GetComponent<PlayerHealth>() != null) // ������ ����� ����� �����
        {
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoin();
            }

            NetworkServer.Destroy(gameObject); // ������� �������� � ����
        }
    }
}
