using System;
using UnityEngine;

public class CastRay : MonoBehaviour
{
    [Range(1.0f, 1000.0f)]
    [SerializeField] private float distance = 50.0f;
    [SerializeField] private LayerMask mask;
    private static LayerMask Mask;

    //public static LayerMask Mask { get { return mask; } }

    private RaycastHit hooveredRayCastHit;
    private RaycastHit prevHooveredRayCastHit;

    public static event Action<RaycastHit> OnHoveredEnter;
    public static event Action<RaycastHit> OnHoveredStay;
    public static event Action<RaycastHit> OnHoveredExit;

    // Debug
    [Header("Debug")]
    [SerializeField] private float _debugSphereDimension = 0.05f;
    private Vector3 pos;
    private bool isHitting;

    private void OnValidate()
    {
        Mask = mask;
    }

    private void Awake()
    {
        Mask = mask;
    }

    void Update()
    {
        pos = transform.position + transform.forward * distance;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distance, Mask.value))
        {
            isHitting = true;

            pos = hit.point;

            hooveredRayCastHit = hit;

            if (prevHooveredRayCastHit.collider == hooveredRayCastHit.collider)
            {
                OnHoveredStay?.Invoke(hit);
            }
            else
            {
                if (prevHooveredRayCastHit.collider != null)
                {
                    OnHoveredExit?.Invoke(prevHooveredRayCastHit);
                }

                OnHoveredEnter?.Invoke(hooveredRayCastHit);
            }

            prevHooveredRayCastHit = hooveredRayCastHit;

        }
        else
        {
            if (prevHooveredRayCastHit.collider != null)
            {
                OnHoveredExit?.Invoke(prevHooveredRayCastHit);
            }

            isHitting = false;

            // Put both RayCastHit at something equals null
            prevHooveredRayCastHit = default;
            hooveredRayCastHit = default;
        }
    }

    public static void ChangeMask(LayerMask layerMask)
    {
        Mask = layerMask; 
    }

    public static void ResetMask()
    {
        Mask = LayerMask.GetMask("Interact");
    }

    private static int GetIndex()
    {
        return (int)Mathf.Log(Mask.value, 2); ;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, pos);

        if (isHitting)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pos, _debugSphereDimension);
        }
    }
}
