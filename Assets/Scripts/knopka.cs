using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class knopka : MonoBehaviour
{
    private Animator[] anims;
    private bool isActive = false;
    [SerializeField] private AnomalyPortal[] portals;
    [SerializeField] [CanBeNull] private knopka pairButton;
    [SerializeField] [CanBeNull] private bool isPresent;
    [SerializeField] private Animator buttonAnimator;
    
    

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (pairButton)
                pairButton.Activate();
        }
        Activate();
        Debug.Log("voshel");
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (pairButton)
                pairButton.DeActivate();
        }
        Debug.Log(other.tag);
        DeActivate();
    }

        private void Activate()
    {
        Debug.Log("ad");
        buttonAnimator.SetBool("pressed", true);
        Array.ForEach(portals, portal => portal.Activate());
    }

    private void DeActivate()
    {
        buttonAnimator.SetBool("pressed", false);
        Array.ForEach(portals, portal => portal.DeActivate());
    }
}
