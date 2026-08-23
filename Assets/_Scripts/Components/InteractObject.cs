using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class InteractObject : NetworkBehaviour
{
    private Vector3 positionToReach;
    private float speed;
    private bool canMove;

    public bool IsInFocus { get; protected set; }
    public Vector3 StartPos { get; protected set; }

    public override void OnNetworkSpawn()
    {
        StartPos = transform.position;
    }

    protected virtual void Update()
    {
        if(IsOwner && canMove)
        {
            GoTo();
        }
        
    }
    
    public void SetPoint(Vector3 positionToReach, float speed)
    {
        if(!IsOwner)
        {
            SetPointServerRpc(positionToReach, speed);
            return;
        }

        this.positionToReach = positionToReach;
        this.speed = speed;
        canMove = true;
    }


    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void SetPointServerRpc(Vector3 positionToReach, float speed)
    {
        this.positionToReach = positionToReach;
        this.speed = speed;
        canMove = true;
    }

    protected virtual void GoTo()
    {
        transform.position = Vector3.Lerp(transform.position, positionToReach, speed * Time.deltaTime);

        if((transform.position - positionToReach).sqrMagnitude < 0.001f)
        {
            transform.position = positionToReach;
           //canMove = false;
        }
    }

    public void Focus()
    {
        IsInFocus = true;
    }
    public void UnFocus()
    {
        IsInFocus = false;
    }
}
