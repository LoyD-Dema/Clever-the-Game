using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations;


[RequireComponent(typeof(Rigidbody))]
public class DieBehavior : InteractObject
{
    private ParentConstraint parentConstraint;
    private Rigidbody rb;
    [SerializeField] float force = 2.5f;



    private bool canMoveAround;
    private float speedMultiplayer = 2.0f;

    // moveAround parameters
    private float pitch;
    private float yaw;
    private float roll;

    Vector3 centerOfMove;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        parentConstraint = GetComponent<ParentConstraint>();    
    }

    public override void OnNetworkSpawn()
    {
        rb.isKinematic = true;
    }

    private void FixedUpdate()
    {
        if (!IsServer)
            return;

        if(canMoveAround)
        {
            Rotate();
        }
    }

    public void IncreseRotationSpeed()
    {
        pitch *= speedMultiplayer;
        yaw *= speedMultiplayer;
        roll *= speedMultiplayer;
    }

    public void DecreseRotationSpeed()
    {
        pitch /= speedMultiplayer;
        yaw /= speedMultiplayer;
        roll /= speedMultiplayer;
    }

    public void Lunch(Vector3 direction)
    {
        canMoveAround = false;
        rb.isKinematic = false;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    public void MoveAround(Vector3 center)
    {
        canMoveAround = true;

        pitch = Random.Range(-50.0f, 50.0f);
        yaw = Random.Range(-50.0f, 50.0f);
        roll = Random.Range(-50.0f, 50.0f);

        centerOfMove = center;
    }

    private void Rotate()
    {
        Vector3 rotation = new Vector3(pitch, yaw, roll) * Time.fixedDeltaTime;
        Quaternion delta = Quaternion.Euler(rotation);
        rb.MoveRotation(rb.rotation * delta);
    }


    public void ResetDie()
    {
        SetPoint(StartPos, 2.0f);
        transform.localRotation = Quaternion.identity;
        rb.isKinematic = true;
    }

    public void Detach()
    {
        parentConstraint.constraintActive = false;
    }
    public void Attach()
    {
        parentConstraint.constraintActive = true;
    }
}
