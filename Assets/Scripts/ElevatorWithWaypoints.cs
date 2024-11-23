using UnityEngine;

public class ElevatorWithWaypoints : MonoBehaviour
{
    public Transform waypointA; 
    public Transform waypointB; 
    public float speed = 2f; 
    public EdgeCollider2D collider1;
    public EdgeCollider2D collider2;
    public EdgeCollider2D collider3;
    public EdgeCollider2D collider4;
    public float activationDistance = 0.1f;

    private Transform targetWaypoint; 

    private void Start()
    {
        targetWaypoint = waypointA;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            targetWaypoint = targetWaypoint == waypointA ? waypointB : waypointA;
        }
        
        
        if (Vector3.Distance(transform.position, waypointA.position) < activationDistance)
        {
            collider1.enabled = true;
            collider4.enabled = true;
        }
        else
        {
            collider1.enabled = false;
            collider4.enabled = false;
                
        }
        
        
        if (Vector3.Distance(transform.position, waypointB.position) < activationDistance)
        {
            collider2.enabled = true;
            collider3.enabled = true;
        }
        else
        {
            collider2.enabled = false;
            collider3.enabled = false;
        }
        
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }
}