using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class knopka : MonoBehaviour
{
    private Animator[] anims;
    private bool isActive = false;
    [SerializeField] private GameObject[] doors;
    [SerializeField] [CanBeNull] private knopka pairButton;
    [SerializeField] [CanBeNull] private bool isPresent;
    
    
    void Start()
    {
         anims = doors.Select(door => door.GetComponent<Animator>()).ToArray();
    }

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
        Debug.Log("voshel");
        Array.ForEach(doors, door => door.GetComponent<BoxCollider2D>().enabled = false);
        Array.ForEach(anims, anim => anim.Play("openDoor"));
    }

    private void DeActivate()
    {
        isActive = false;
        Array.ForEach(doors, door => door.GetComponent<BoxCollider2D>().enabled = true);
        Array.ForEach(anims, anim => anim.Play("closeDoor"));
    }
}
