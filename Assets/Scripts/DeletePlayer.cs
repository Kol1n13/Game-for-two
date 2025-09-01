using UnityEngine;

public class DeletePlayer : MonoBehaviour
{
    bool playerFound = false;

    void Update()
    {
        if (!playerFound)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.SetActive(false);
                playerFound = true;
            }
        }
    }
}