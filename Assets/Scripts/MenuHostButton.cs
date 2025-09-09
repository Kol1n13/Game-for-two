using UnityEngine;
using Mirror;

public class MenuHostButton : MonoBehaviour
{
    public void OnClickHost()
    {
        if (NetworkManager.singleton == null)
        {
            Debug.LogError("[MenuHostButton] NetworkManager not found in scene!");
            return;
        }

        // если что-то запущено — сначала останавливаем
        if (NetworkServer.active || NetworkClient.isConnected)
        {
            Debug.Log("[MenuHostButton] Already running, stopping before restart...");
            NetworkManager.singleton.StopHost();
        }

        Debug.Log("[MenuHostButton] Starting Host...");
        NetworkManager.singleton.StartHost();
    }
}
