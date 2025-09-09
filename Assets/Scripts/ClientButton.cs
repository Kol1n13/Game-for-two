using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ClientButton : MonoBehaviour
{
    public InputField ipInput; // поле ввода IP (добавь в инспекторе)

    public void OnClientPressed()
    {
        if (NetworkManager.singleton == null)
        {
            Debug.LogError("[ClientButton] NetworkManager.singleton == null!");
            return;
        }

        // Если уже есть подключение — сначала отключаем
        if (NetworkClient.isConnected || NetworkClient.active)
        {
            Debug.Log("[ClientButton] Already connected -> StopClient()");
            NetworkManager.singleton.StopClient();
        }

        // Подставляем IP, если указано
        if (ipInput != null && !string.IsNullOrWhiteSpace(ipInput.text))
        {
            NetworkManager.singleton.networkAddress = ipInput.text;
        }
        else
        {
            NetworkManager.singleton.networkAddress = "localhost"; // по умолчанию
        }

        Debug.Log($"[ClientButton] Starting client to {NetworkManager.singleton.networkAddress}");
        NetworkManager.singleton.StartClient();
    }
}
