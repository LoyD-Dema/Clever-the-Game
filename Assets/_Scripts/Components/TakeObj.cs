using System;
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

}

public class TakeObj : MonoBehaviour
{
    [SerializeField] Transform reachTransform;
    Vector3 startPos;
    [SerializeField] float speed = 1.0f;

    private IInteractable hooveredObj;
    private IInteractable activeObj;

    private PlayerInput playerInput;

    bool isTake;
    bool isGoBack;

    private Action<RaycastHit> onHoveredEnter;
    private Action<RaycastHit> onHoveredExit;

    private void Awake()
    {
        playerInput = GetComponentInParent<PlayerInput>();
    }

    private void OnEnable()
    {
        onHoveredEnter = (RaycastHit hit) => { hooveredObj = hit.collider.GetComponent<IInteractable>();
                                               startPos = hooveredObj.StartPos; };
        onHoveredExit = (RaycastHit hit) => hooveredObj = null;

        CastRay.OnHoveredEnter += onHoveredEnter;
        CastRay.OnHoveredExit += onHoveredExit;
        
        playerInput.actions["Interact"].started += OnInteract;
        playerInput.actions["Interact"].performed += OnInteract;
        playerInput.actions["Interact"].canceled += OnInteract;
        playerInput.actions["Releseback"].started += OnReleseBack;
    }

    private void OnDisable()
    {
        CastRay.OnHoveredEnter -= onHoveredEnter;
        CastRay.OnHoveredExit -= onHoveredExit;

        playerInput.actions["Interact"].started -= OnInteract;
        playerInput.actions["Interact"].performed -= OnInteract;
        playerInput.actions["Interact"].canceled -= OnInteract;
        playerInput.actions["Releseback"].started -= OnReleseBack;
    }

    private void Update()
    {
        if (isTake)
            Take();
        else if (isGoBack)
            GoBack();
    }


    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (activeObj != null)
            {
                activeObj.LeftMousePress();
            }
            else if (hooveredObj != null) // Here I am already sure the active object is null
            {
                activeObj = hooveredObj;
                activeObj.IsActiveObj = true;
                isTake = true;
            }
        }
        else if(context.performed)
        {
            if (activeObj != null)
            {
                activeObj.LeftMouseHold();
            }
        }
        else if(context.canceled)
        {
            if (activeObj != null)
            {
                activeObj.LeftMouseRelese();
            }
        }
    }

    private void OnReleseBack(InputAction.CallbackContext context)
    {
        if (context.started && activeObj != null)
        {
            isGoBack = true;
        }
    }

    private void Take()
    {
        activeObj.GameObject.transform.position = Vector3.Lerp(activeObj.GameObject.transform.position, reachTransform.position,
                                                                 speed * Time.deltaTime);

        if ((activeObj.GameObject.transform.position - reachTransform.position).magnitude < 0.001f)
        {
            activeObj.PositionReached();
            isTake = false;
        }
    }

    private void GoBack()
    {
        activeObj.GameObject.transform.position = Vector3.Lerp(activeObj.GameObject.transform.position, startPos,
                                                                 speed * 2.5f * Time.deltaTime);

        if ((activeObj.GameObject.transform.position - startPos).magnitude < 0.001f)
        {
            isGoBack = false;
            activeObj = null;
        }
    }
}
