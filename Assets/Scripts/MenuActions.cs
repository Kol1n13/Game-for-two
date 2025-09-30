using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections;

public static class MenuActions
{
    // Вынесенная логика выхода в меню
    public static void GoToMenu(bool stopHostWhenHostPressesMenu, MonoBehaviour caller)
    {
        if (NetworkClient.active && NetworkServer.active)
        {
            // Хост
            Debug.Log("[MenuActions] Local player is server -> changing scene for everyone to 'Menu'.");
            if (NetworkManager.singleton != null)
            {
                NetworkManager.singleton.ServerChangeScene("Menu");

                if (stopHostWhenHostPressesMenu && caller != null)
                {
                    caller.StartCoroutine(KillNetworkManagerDelayed());
                }
            }
        }
        else if (NetworkClient.isConnected)
        {
            // Клиент
            Debug.Log("[MenuActions] Local player is client -> disconnecting client and loading local Menu scene.");
            if (NetworkManager.singleton != null)
            {
                NetworkManager.singleton.StopClient();
                Object.Destroy(NetworkManager.singleton.gameObject);
            }
            SceneManager.LoadScene("Menu");
        }
        else
        {
            // Соло запуск без сети
            Debug.Log("[MenuActions] No network -> just loading Menu.");
            SceneManager.LoadScene("Menu");
        }
    }

    private static IEnumerator KillNetworkManagerDelayed()
    {
        yield return null; // ждём кадр
        if (NetworkManager.singleton != null)
        {
            Debug.Log("[MenuActions] Stopping and destroying NetworkManager.");
            NetworkManager.singleton.StopHost();
            Object.Destroy(NetworkManager.singleton.gameObject);
        }
    }
}
