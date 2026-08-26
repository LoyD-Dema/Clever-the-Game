using System;
using System.Collections;
using UnityEngine;
public class TriggerRelay : MonoBehaviour
{
    public SphereCollider sphereCollider {  get; private set; }

    public event Action<Collider> OnTriggerEnterEvent;
    public event Action<Collider> OnTriggerStayEvent;
    public event Action<Collider> OnTriggerExitEvent;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerStayEvent?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }
}
