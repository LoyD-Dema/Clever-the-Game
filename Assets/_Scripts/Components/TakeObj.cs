using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public GameObject GameObject { get; }
    public Vector3 StartPos { get; }
    public void Relese();

    public void ResetPos();
}

public class TakeObj : MonoBehaviour
{
    [SerializeField] Transform reachTransform;
    Vector3 startPos;
    [SerializeField] float speed = 1.0f;
    private float elapsedTimeToHold;

    private bool isHolding;
    private bool canGoBack;
    private bool canRelese;
    private IInteractable obj;

    private void Awake()
    {
        CastRay.OnHoveredEnter += (RaycastHit hit) => { obj = hit.collider.GetComponent<IInteractable>(); startPos = obj.StartPos; };

        //CastRay.OnHoveredExit += (RaycastHit hit) => obj = null;
    }

    private void Start()
    {
        elapsedTimeToHold = speed;
    }

    private void Update()
    {
        if (isHolding && obj != null)
        {
            Take();
        }
        else if (canGoBack && obj != null)
        {
            obj.ResetPos();
            canGoBack = false;
        }
    }

    private void OnSelect(InputValue value)
    {
        if (value.isPressed)
        {
            isHolding = true;
            //CastRay.ChangeMask(0);
        }
        else
        {
            isHolding = false;
            Relese();
        }
    }

    private void Take()
    {
        obj.GameObject.transform.position = Vector3.Lerp(obj.GameObject.transform.position, reachTransform.position, speed * Time.deltaTime);
        
        if ((obj.GameObject.transform.position - reachTransform.position).magnitude < 0.001f)
        {
            canRelese = true;
        }
    }

    

    private void Relese()
    {
        //CastRay.ResetMask();

        if (canRelese)
        {
            obj.Relese();
            return;
        }

        canGoBack = true;

    }
}
