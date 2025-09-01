using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class Restart : NetworkBehaviour
{
    public void ToTemple()
    {
        if (isServer)
        {
            NetworkManager.singleton.ServerChangeScene("Temple");

        }
        else
        {
            Debug.LogWarning("Scene change can only be initiated by the server.");
        }
    }
}