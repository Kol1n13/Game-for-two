using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class IpWrite : MonoBehaviour
{
    public NetworkManager networkManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void setNetworkAddress(string ip)
    {

        Debug.Log(ip);
        networkManager.networkAddress = ip;
    }
}
