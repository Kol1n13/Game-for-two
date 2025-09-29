using UnityEngine;
using Mirror;

public class EnsureSingleNetworkManager : MonoBehaviour
{
    void Awake()
    {
        // если уже есть синглтон и это не он → убиваем
        if (NetworkManager.singleton != null && NetworkManager.singleton != GetComponent<NetworkManager>())
        {
            Debug.Log("[EnsureSingleNetworkManager] Duplicate NetworkManager detected -> destroying this one.");
            Destroy(gameObject);
            return;
        }

        // этот становится основным
        DontDestroyOnLoad(gameObject);
    }
}
