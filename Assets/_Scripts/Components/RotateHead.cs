using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateHead : MonoBehaviour
{
    [SerializeField] Transform neckTransform;
    

    [Header("Params")]
    [SerializeField] private float defaultRotationSpeed;
    [SerializeField] private float defaultMinHorizontalRotation;
    [SerializeField] private float defaultMaxHorizontalRotation;
    [SerializeField] private float defaultMinVerticalRotation;
    [SerializeField] private float defaultMaxVerticalRotation;

    private float rotationSpeed;
    private float minHorizontalRotation;
    private float maxHorizontalRotation;
    private float minVerticalRotation;
    private float maxVerticalRotation;

    private Vector2 inputDirectiont;
    float yaw; // Horizontal
    float picth; // Vertical

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        Cursor.lockState = CursorLockMode.Locked;
        Unlock();
    }

    private void OnEnable()
    {
        playerInput.actions["Move"].started += OnMove;
    }
    private void OnDisable()
    {
        playerInput.actions["Move"].started -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // the value is already normalized
        inputDirectiont = context.ReadValue<Vector2>();
       
    }

    private void Update()
    {
        // Update yaw and pitch values
        yaw += rotationSpeed * inputDirectiont.x * Time.deltaTime;
        picth += rotationSpeed * inputDirectiont.y * Time.deltaTime;

        // Clap the values to avoid weird rotations
        yaw = Mathf.Clamp(yaw, minHorizontalRotation, maxHorizontalRotation);
        picth = Mathf.Clamp(picth, minVerticalRotation, maxVerticalRotation);
        
        // Set the final rotation
        neckTransform.localRotation = Quaternion.Euler(picth, yaw, 0);
    }

    public void PartialLock()
    {
        rotationSpeed = defaultRotationSpeed * 0.33f;
        minHorizontalRotation = defaultMinHorizontalRotation * 0.33f;
        maxHorizontalRotation = defaultMaxHorizontalRotation * 0.33f;
        minVerticalRotation = defaultMinVerticalRotation * 0.33f;
        maxVerticalRotation = defaultMaxVerticalRotation * 0.33f;
    }

    public void Unlock()
    {
        rotationSpeed = defaultRotationSpeed;
        minHorizontalRotation = defaultMinHorizontalRotation;
        maxHorizontalRotation = defaultMaxHorizontalRotation;
        minVerticalRotation = defaultMinVerticalRotation;
        maxVerticalRotation = defaultMaxVerticalRotation;
    }
}
