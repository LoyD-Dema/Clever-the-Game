using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public GameObject GameObject { get; }
    public Vector3 StartPos { get; }
    public bool IsActiveObj { get; set; }

    public void PositionReached();
    public void LeftMousePress();
    public void LeftMouseHold();
    public void LeftMouseRelese();
    public void ReleseBackPressed();

}

public class TakeObject : NetworkBehaviour
{
    [SerializeField] Transform pointToReach;
    [SerializeField] float takeSpeed = 1.0f;
    private float releseSpeed = 1.0f;

    private GameObject hooveredGameObject;
    private InteractObject activeObj;

    private PlayerInput playerInput;

    bool isTaken;
    bool isGoBack;
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
        playerInput.actions["Interact"].performed += OnInteract;
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
        playerInput.actions["Interact"].performed -= OnInteract;
        playerInput.actions["Interact"].canceled -= OnInteract;
        playerInput.actions["ReleseBack"].started -= OnReleseBack;
    }

    private void Update()
    {
        //if (isHolding)
        //{
        //    if (activeObj != null)
        //    {
        //        activeObj.LeftMouseHold();
        //    }
        //}
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (hooveredGameObject != null && activeObj == null) // Here I am already sure the active object is null
            {
                if(hooveredGameObject.TryGetComponent<InteractObject>(out InteractObject interactObject))
                { 
                    activeObj = interactObject;
                    activeObj.Focus();
                    activeObj.SetPoint(pointToReach.position, takeSpeed);
                }
            }
        }
        else if (context.canceled)
        {
            isHolding = false;

            if (activeObj != null)
            {
                //activeObj.LeftMouseRelese();
            }
        }
    }

    private void OnReleseBack(InputAction.CallbackContext context)
    {
        Debug.Log("E Pressed");

        if (context.started && activeObj != null && hooveredGameObject == activeObj.gameObject)
        {
            activeObj.UnFocus();
            activeObj.SetPoint(activeObj.StartPos, releseSpeed);
            activeObj = null;
        }
    }
}
