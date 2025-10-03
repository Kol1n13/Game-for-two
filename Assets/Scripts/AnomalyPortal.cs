using System;
using UnityEngine;

public class AnomalyPortal : MonoBehaviour
{

    [Header("Portal Configuration")]
    [SerializeField] private bool isActive = false;
    [SerializeField] private AnomalyPortal pairPortal;
    [SerializeField] private Animator portalAnimator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("YEWS");
        if (isActive && other.CompareTag("Player"))
        {
            TeleportObject(other.gameObject);
        }
    }

    private void TeleportObject(GameObject obj)
    {
        obj.transform.position = pairPortal.transform.position;
    }

    public void Activate()
    {
        portalAnimator.SetBool("isActive", true);
        isActive = true;
    }

    public void DeActivate()
    {
        portalAnimator.SetBool("isActive", false);
        isActive = false;
    }
}
