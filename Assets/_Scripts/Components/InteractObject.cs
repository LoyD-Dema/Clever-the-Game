using Unity.Netcode;
using UnityEngine;

public class InteractObject : NetworkBehaviour
{
    private Vector3 positionToReach;
    protected float speed {  get; private set; }    
    protected bool canMove { get; private set; }

    public Vector3 StartPos { get; protected set; }

    public override void OnNetworkSpawn()
    {
        StartPos = transform.localPosition;
    }

    protected virtual void Update()
    {
        if (IsOwner && canMove)
        {
            GoTo();
        }

    }


    public void SetPoint(Vector3 positionToReach, float speed)
    {
        if (!IsOwner)
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

        if ((transform.position - positionToReach).sqrMagnitude < 0.0001f)
        {
            transform.position = positionToReach;
            canMove = false;
        }
    }

    protected virtual void OverridePositionToReach(Vector3 newPositionToReach)
    {
        positionToReach = newPositionToReach;
    }

    public virtual void Select()
    {

    }
    public virtual void UnSelect()
    {

    }
    public virtual void Back()
    {
       
    }

}
