using UnityEngine;

public class IsoDartTrap : MonoBehaviour
{
    public Transform firePoint;
    public GameObject dartPrefab;
    public float fireRate = 1f;
    public float dartSpeed = 10f;
    public Vector3 direction = Vector3.forward;

    private float _cooldown;

    void Start()
    {
        if (firePoint == null)
        {
            GameObject fp = new GameObject("FirePoint");
            fp.transform.SetParent(transform);
            fp.transform.localPosition = new Vector3(0, 0, 0.5f); // чуть спереди
            firePoint = fp.transform;
        }
    }

    void Update()
    {
        _cooldown -= Time.deltaTime;
        if (_cooldown <= 0f)
        {
            Fire();
            _cooldown = 1f / fireRate;
        }
    }

    void Fire()
    {
        GameObject go = Instantiate(dartPrefab, firePoint.position, Quaternion.identity);
        Dart dart = go.GetComponent<Dart>();
        dart.direction = direction;
        dart.speed = dartSpeed;
    }
}
