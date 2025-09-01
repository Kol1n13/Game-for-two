using UnityEngine;

public class PresentAnchor : MonoBehaviour
{
    public GameObject FutureAnchor;
    
    public static PresentAnchor Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Vector3 GetSceneVector()
    {
        return transform.position - FutureAnchor.transform.position;
    }
}
