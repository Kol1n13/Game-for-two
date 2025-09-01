using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class ToMenu : NetworkBehaviour
{
    public void ChangeToMenu()
    {
        if (isServer)
        {
            NetworkManager.singleton.ServerChangeScene("Menu");
            NetworkManager.singleton.StopHost();

        }
        else
        {
            Debug.LogWarning("Scene change can only be initiated by the server.");
        }
    }
}