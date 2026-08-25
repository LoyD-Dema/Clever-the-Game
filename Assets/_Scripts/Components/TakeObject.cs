using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class TakeObject : NetworkBehaviour
{
    [SerializeField] Transform pointToReach;
    [SerializeField] float takeSpeed = 1.0f;
    private float releseSpeed = 1.0f;

    private GameObject hooveredGameObject;
    private InteractObject activeObj;

    private PlayerInput playerInput;

    bool isHolding;

    private Action<RaycastHit> onHoveredStay;
    private Action<RaycastHit> onHoveredExit;

    private void Awake()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        onHoveredStay = (RaycastHit hit) => { hooveredGameObject = hit.collider.gameObject; };
        onHoveredExit = (RaycastHit hit) => { hooveredGameObject = null; };
    }

    private void Start()
    {
        releseSpeed = takeSpeed * 2.5f;
    }


    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        CastRay.OnHoveredEnter += onHoveredStay;
        CastRay.OnHoveredExit += onHoveredExit;

        // Inputs
        playerInput.actions["Interact"].started += OnInteract;
        playerInput.actions["Interact"].canceled += OnInteract;
        playerInput.actions["ReleseBack"].started += OnReleseBack;
    }


    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        CastRay.OnHoveredEnter -= onHoveredStay;
        CastRay.OnHoveredExit -= onHoveredExit;

        // Inputs
        playerInput.actions["Interact"].started -= OnInteract;
        playerInput.actions["Interact"].canceled -= OnInteract;
        playerInput.actions["ReleseBack"].started -= OnReleseBack;
    }


    private void OnInteract(InputAction.CallbackContext context)
    {
        if (hooveredGameObject == null)
            return;

        if (context.started)
        {
            if (activeObj != null)
                return;

            // Take an object from the table
            if (hooveredGameObject.TryGetComponent<InteractObject>(out InteractObject interactObject))
            {
                activeObj = interactObject;
                activeObj.SetPoint(pointToReach.position, takeSpeed);
                activeObj.Select();
            }
            else
            {
                Debug.LogError("The gameobject has the tag Interact, but not the InteractObject script", hooveredGameObject);
            }
        }
        else if (context.canceled && activeObj != null && activeObj.gameObject == hooveredGameObject)
        {
            activeObj.UnSelect();
        }
    }

    private void OnReleseBack(InputAction.CallbackContext context)
    {
        if (activeObj == null || hooveredGameObject == null)
            return;

        if (context.started && hooveredGameObject == activeObj.gameObject)
        {
            activeObj.Back();
            activeObj.SetPoint(activeObj.StartPos, releseSpeed);
            activeObj = null;
        }
    }
}
