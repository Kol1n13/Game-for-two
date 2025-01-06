using System;
using UnityEngine;

public class AnomalyPortal : MonoBehaviour
{
  
    [Header("Portal Configuration")]
    public GameObject exit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("YEWS");
        if (other.CompareTag("box"))
        {
            // Перемещаем объект к выходу
            TeleportObject(other.gameObject);
        }
    }

    private void TeleportObject(GameObject obj)
    {
        obj.transform.position = exit.transform.position;
    }
}
