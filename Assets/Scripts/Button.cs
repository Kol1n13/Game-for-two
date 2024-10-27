using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using UnityEngine.Events; 

public class PressurePad: MonoBehaviour {
 
    public UnityEvent OnActivate; 
    public UnityEvent OnDeactivate;
    public GameObject door;

    public float deactivationDelay = 6f;

    int objectsInContact;
    Coroutine waitingToDeactivate;


    IEnumerator Timer() { 
        yield return new WaitForSeconds(deactivationDelay);

        // Tell our listeners we're switched OFF.
        if (OnDeactivate != null) 
            OnDeactivate.Invoke(); 

        // Return to ready-to-activate state:
        waitingToDeactivate = null;
    } 

    void OnTriggerEnter2D(Collider2D other) { 
        Debug.Log("Hello");
        var a = door.GetComponent<Animation>();
        var coll = door.GetComponent<BoxCollider2D>();
        coll.enabled = false;
        Debug.Log(a);
        a.Play();
        objectsInContact++; 
        
        // Pressure pad pressed.
        if (objectsInContact == 1) {

            if (waitingToDeactivate != null) {
                // Cancel deactivation timer. 
                StopCoroutine(waitingToDeactivate);
                waitingToDeactivate = null;

            } else if (OnActivate != null) {
                // Tell our listeners we're switched ON.
                OnActivate.Invoke(); 
            } 
        }
    }

    void OnTriggerExit2D(Collider2D other) { 
        objectsInContact--; 

        // Pressure pad released.
        if (objectsInContact == 0) {
            // Start a delay before telling anyone.
            waitingToDeactivate = StartCoroutine(Timer()); 
        } 
    }
}