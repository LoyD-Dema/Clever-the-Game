using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public GameObject GameObject { get; }
    public Vector3 StartPos { get; }
    public void OnPositionReached();

    public void ResetPos();
}

public class TakeObj : MonoBehaviour
{
    [SerializeField] Transform reachTransform;
    Vector3 startPos;
    [SerializeField] float speed = 1.0f;

    private bool isPressed;
    private IInteractable hooveredObj;
    private IInteractable activeObj;

    bool isTake;
    bool isGoBack;

    private void Awake()
    {
        CastRay.OnHoveredEnter += (RaycastHit hit) =>
        {
            hooveredObj = hit.collider.GetComponent<IInteractable>(); 
            startPos = hooveredObj.StartPos;
        };

        CastRay.OnHoveredExit += (RaycastHit hit) => hooveredObj = null;
    }

    private void Update()
    {
        if (isTake)
            Take();
        else if (isGoBack)
            GoBack();
    }


    private void OnInteract(InputValue value)
    {
        if (value.isPressed && !isPressed && hooveredObj != null && activeObj == null)
        {
            isPressed = true;
            activeObj = hooveredObj;
            isTake = true;
        }
    }

    private void OnReleseBack(InputValue value)
    {
        if (value.isPressed && activeObj != null)
        {
            isGoBack = true;
        }
    }

    private void Take()
    {
        // No Time.deltaTime required because this method is already called every DeltaTime
        activeObj.GameObject.transform.position = Vector3.Lerp(activeObj.GameObject.transform.position, reachTransform.position, 
                                                                 speed * Time.deltaTime);

        if ((activeObj.GameObject.transform.position - reachTransform.position).magnitude < 0.001f)
        {
            isPressed = false;
            activeObj.OnPositionReached();
            isTake = false;
        }
    }

    private void GoBack()
    {
        // No Time.DeltaTime required because this method is already called every DeltaTime
        activeObj.GameObject.transform.position = Vector3.Lerp(activeObj.GameObject.transform.position, startPos, 
                                                                 speed * 2.5f * Time.deltaTime);

        if ((activeObj.GameObject.transform.position - startPos).magnitude < 0.001f)
        {
            isGoBack = false;
            activeObj = null;
        }
    }
}
