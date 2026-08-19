using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class DieBehavior : NetworkBehaviour
{
    private Rigidbody rb;
    [SerializeField] float force = 2.5f;


    private bool isMoveAround;
    private float speedMultiplayer = 2.0f;

    // moveAround parameters
    private float pitch;
    private float yaw;
    private float roll;

    Vector3 centerOfMove;



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.isKinematic = true;
    }

    private void FixedUpdate()
    {
        if(isMoveAround)
        {
            Rotate();
        }
    }

    public void Hold()
    {
        pitch *= speedMultiplayer;
        yaw *= speedMultiplayer;
        roll *= speedMultiplayer;
    }

    public void UnHold()
    {
        pitch /= speedMultiplayer;
        yaw /= speedMultiplayer;
        roll /= speedMultiplayer;
    }

    public void Lunch(Vector3 direction)
    {
        CancelInvoke(nameof(Rotate));
        rb.isKinematic = false;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    public void MoveAround(Vector3 center)
    {
        isMoveAround = true;

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

    private void Move()
    {

    }
}
