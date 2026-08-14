using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))] 
public class DiceBehavior : NetworkBehaviour
{
    private Rigidbody rb;
    [SerializeField] float force = 2.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    private void Start()
    {
        rb.isKinematic = true;
    }

    public void Lunch(Vector3 direction)
    {
        // In futuor isKinematic si trova gia' a false
        rb.isKinematic = false;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
}
