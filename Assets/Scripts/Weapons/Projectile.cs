using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float force = 1000.0f; //¼Óµµ

    private new Rigidbody rigidbody;
    private new Collider collider;

    private Vector3 direction;
    public Vector3 Direction { set => direction = value; }

    public event Action<Collider, Collider, Vector3, Vector3> OnProjectileHit;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    private void Start()
    {
        Destroy(gameObject, 10.0f);

        rigidbody.AddForce(direction * force);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnProjectileHit?.Invoke(collider, other, transform.position, other.transform.forward);
        
        Destroy(gameObject);
    }
}