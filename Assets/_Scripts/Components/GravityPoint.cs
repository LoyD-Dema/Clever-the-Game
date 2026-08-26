using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GravityPoint : NetworkBehaviour
{
    private Dictionary<Collider, Rigidbody> bodies;

    [SerializeField] private TriggerRelay detectRelay;
    [SerializeField] private TriggerRelay deactiveRelay;
    [SerializeField] float attractionForce;

    private bool isActive;

    private void Awake()
    {
        bodies = new Dictionary<Collider, Rigidbody>();
    }

    public void Active()
    {
        CheckInitialOverlap();
        isActive = true;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        deactiveRelay.OnTriggerEnterEvent += DeactiveAttraction;
    }

    private void Start()
    {
        if (!IsServer)
            return;


    }

    private void FixedUpdate()
    {
        if (!isActive || !IsServer)
            return;

        foreach (var b in bodies)
        {
            Vector3 dir = transform.position - b.Value.position;
            b.Value.AddForce(dir * attractionForce, ForceMode.Acceleration);
        }
    }

    private void CheckInitialOverlap()
    {
        Collider[] colliders = Physics.OverlapSphere(
            detectRelay.transform.position,
            detectRelay.sphereCollider.radius
        );

        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent<Rigidbody>(out Rigidbody rb) && !bodies.ContainsKey(col))
            {
                bodies.Add(col, rb);
                rb.useGravity = false;
            }
        }
    }

    private void DeactiveAttraction(Collider collider)
    {
        if (!bodies.ContainsKey(collider))
            return;

        bodies[collider].useGravity = true;
        bodies.Remove(collider);
    }
}
