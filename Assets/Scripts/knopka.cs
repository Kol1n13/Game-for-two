using UnityEngine;

public class knopka : MonoBehaviour
{
    private Animator anim;
    private bool isActive = false;
    [SerializeField] private GameObject door;
    
    
    void Start()
    {
         anim = door.GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("voshel");
        anim.Play("openDoor");
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        isActive = false;
        anim.Play("closeDoor");
    }
}
