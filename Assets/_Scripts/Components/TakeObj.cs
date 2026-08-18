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
    
    private void Awake()
    {
        CastRay.OnHoveredEnter += (RaycastHit hit) => { hooveredObj = hit.collider.GetComponent<IInteractable>(); startPos = hooveredObj.StartPos; };
    }


    private void OnIteract(InputValue value)
    {
        if (value.isPressed && !isPressed && hooveredObj != null)
        {
            isPressed = true;
            activeObj = hooveredObj;
            InvokeRepeating(nameof(Take), 0, Time.deltaTime);
        }
    }

    private void OnExit(InputValue value)
    {
        if (value.isPressed && hooveredObj == null)
        {
            InvokeRepeating(nameof(GoBack), 0, Time.deltaTime);
        }
    }

    private void Take()
    {
        // No Time.deltaTime required because this method is already called every DeltaTime
        hooveredObj.GameObject.transform.position = Vector3.Lerp(activeObj.GameObject.transform.position, reachTransform.position, speed);
        
        if ((hooveredObj.GameObject.transform.position - reachTransform.position).magnitude < 0.001f)
        {
            isPressed = false;
            hooveredObj.OnPositionReached();
            CancelInvoke(nameof(Take));
        }
    }

    private void GoBack()
    {
        // No Time.DeltaTime required because this method is already called every DeltaTime
        hooveredObj.GameObject.transform.position = Vector3.Lerp(activeObj.GameObject.transform.position, startPos, speed * 2.5f);

        if ((hooveredObj.GameObject.transform.position - startPos).magnitude < 0.001f)
        {
            CancelInvoke(nameof(GoBack));
        }
    }
}
