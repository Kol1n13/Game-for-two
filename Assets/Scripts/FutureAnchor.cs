using UnityEngine;

public class FutureAnchor : MonoBehaviour
{
    public GameObject PresentAnchor;
    
    public static FutureAnchor Instance;
    
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
        return transform.position - PresentAnchor.transform.position;
    }
}