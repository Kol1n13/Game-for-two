using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkGoalTile2D : NetworkBehaviour
{
    [Tooltip("Сколько игроков должно стоять на плитке")]
    public int requiredPlayers = 2;

    [Tooltip("Сколько секунд игроки должны простоять на блоке")]
    public float requiredTime = 2f;

    [Tooltip("Минимальное количество монет для завершения уровня")]
    public int requiredCoins = 3;

    // список игроков на плитке
    private HashSet<uint> playersHere = new HashSet<uint>();

    // таймер
    private float timer = 0f;
    private bool levelFinished = false;

    [ServerCallback]
    private void Update()
    {
        if (levelFinished) return;

        if (playersHere.Count >= requiredPlayers)
        {
            timer += Time.deltaTime;

            if (timer >= requiredTime)
            {
                // Проверяем, достаточно ли монет собрано
                if (CoinManager.Instance != null && CoinManager.Instance.GetTotalCoins() >= requiredCoins)
                {
                    levelFinished = true;
                    RpcShowEnd();
                }
                else
                {
                    Debug.Log($"[Server] Требуется минимум {requiredCoins} монет. Сейчас собрано: " +
                              (CoinManager.Instance != null ? CoinManager.Instance.GetTotalCoins() : 0));
                }
            }
        }
        else
        {
            timer = 0f; // сброс если игроков меньше чем нужно
        }
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D other)
    {
        var pm = other.GetComponentInParent<PlayerMovement>();
        if (pm == null) return;

        NetworkIdentity ni = pm.GetComponent<NetworkIdentity>();
        if (ni == null) return;

        if (playersHere.Add(ni.netId))
        {
            Debug.Log($"[Server] Player {ni.netId} entered goal. Count = {playersHere.Count}");
        }
    }

    [ServerCallback]
    private void OnTriggerExit2D(Collider2D other)
    {
        var pm = other.GetComponentInParent<PlayerMovement>();
        if (pm == null) return;

        NetworkIdentity ni = pm.GetComponent<NetworkIdentity>();
        if (ni == null) return;

        if (playersHere.Remove(ni.netId))
        {
            Debug.Log($"[Server] Player {ni.netId} left goal. Count = {playersHere.Count}");
        }
    }

    // вызывается на всех клиентах
    [ClientRpc]
    private void RpcShowEnd()
    {
        if (EndPanelManager.Instance != null)
        {
            EndPanelManager.Instance.ShowEndPanel();
        }

        // отключаем движение игрока у всех
        foreach (var pm in FindObjectsOfType<PlayerMovement>())
        {
            pm.enabled = false;
        }
    }
}
