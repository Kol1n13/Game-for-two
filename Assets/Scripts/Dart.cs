using UnityEngine;

public class Dart : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;
    [HideInInspector] public Vector3 direction = Vector3.forward;

    private float _timer;

    void Update()
    {
        transform.position += direction.normalized * speed * Time.deltaTime;
        _timer += Time.deltaTime;
        if (_timer >= lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Trap"))
        {
            Destroy(gameObject);
        }
    }
}
