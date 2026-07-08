using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateHead : MonoBehaviour
{
    [SerializeField] Transform neckTransform;
    

    [Header("Params")]
    [SerializeField] float rotationSpeed;
    [SerializeField] float minHorizontalRotation;
    [SerializeField] float maxHorizontalRotation;
    [SerializeField] float minVerticalRotation;
    [SerializeField] float maxVerticalRotation;

    private Vector2 inputDirectiont;
    float yaw; // Horizontal
    float picth; // Vertical

    private void OnMove(InputValue value)
    {
        // the value is already normalized
        inputDirectiont = value.Get<Vector2>();
       
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
}
