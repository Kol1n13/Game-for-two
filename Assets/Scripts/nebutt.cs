using UnityEngine;

public class neButton : UnityEngine.UIElements.Button
{
    
    [SerializeField] private GameObject door;
    public bool IsActive;
    private Animator anim;


    void Update()
    {
        if (IsActive)
        {
            anim.Play("Door");
        }
    }
    
    private void Awake()
    {
        anim = door.GetComponent<Animator>();
    }
}
