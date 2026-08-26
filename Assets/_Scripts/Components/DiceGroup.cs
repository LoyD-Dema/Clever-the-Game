using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DiceGroup : InteractObject
{
    private List<DieBehavior> activeDice;
    [SerializeField] private List<DieBehavior> dice;

    private bool isHolding;

    private Vector3 newPointToReach => clientPlayerCam.transform.position + clientPlayerCam.transform.forward * 1.2f;
    private Camera clientPlayerCam;
    private bool canOverride;

    [SerializeField] private GravityPoint gravityPoint;

    private void Awake()
    {
        activeDice = dice;
    }


    protected override void Update()
    {
        if (!IsServer)
            return;

        if (canOverride)
        {
            Debug.Log(clientPlayerCam.transform.rotation);
            OverridePositionToReach(newPointToReach);
        }

        base.Update();
    }

    public override void Select()
    {
        base.Select();

        if(NetworkManager.LocalClient.PlayerObject.TryGetComponent<RotateHead>(out RotateHead rotateHead))
        {
            rotateHead.PartialLock();
        }

        SetOverideServerRpc(true, NetworkManager.LocalClient.PlayerObject);
        HoldDiceServerRpc();
    }

    public override void Back()
    {
        base.Back();

        SetOverideServerRpc(false, NetworkManager.LocalClient.PlayerObject);
        ResetGroupServerRpc();
    }

    public override void UnSelect()
    {
        base.UnSelect();

        if (NetworkManager.LocalClient.PlayerObject.TryGetComponent<RotateHead>(out RotateHead rotateHead))
        {
            rotateHead.Unlock();
        }
        UnselectServerRpc();
    }

    public void RemoveDice(DieBehavior die)
    {
        die.Detach();
        activeDice.Remove(die);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void UnselectServerRpc()
    {
        if ((transform.position - newPointToReach).magnitude < 0.5f)
        {
            gravityPoint.Active();
            LunchDice(clientPlayerCam.transform.forward);
        }
        else
        {
            SetOverideServerRpc(false, NetworkManager.LocalClient.PlayerObject);
            SetPoint(StartPos, speed);
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SetOverideServerRpc(bool enable, NetworkObjectReference clientPlayer)
    {
        canOverride = enable;

        if (clientPlayer.TryGet(out NetworkObject obj))
        {
            clientPlayerCam = obj.GetComponentInChildren<Camera>(true);
        }
    }

   


    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void HoldDiceServerRpc()
    {
        foreach (DieBehavior die in activeDice)
        {
            die.MoveAround(transform.position);
            die.IncreseRotationSpeed();
        }
    }

    private void LunchDice(Vector3 direction)
    {
        foreach (DieBehavior die in activeDice)
        {
            die.Detach();
            die.DecreseRotationSpeed();
            die.Lunch(direction);

            // TODO - Comunicare al takeObject che nessun oggetto attivo e' impostato
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SelectDiceServerRpc()
    {
        foreach (DieBehavior die in activeDice)
        {
            die.MoveAround(transform.position);
        }
    }


    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void ResetDiceServerRpc()
    {
        foreach (DieBehavior die in activeDice)
        {
            die.ResetDie();
        }
    }

    private void AttachDice()
    {
        foreach (DieBehavior die in activeDice)
        {
            die.Attach();
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)] //  TODO - Change in only server
    public void ResetGroupServerRpc()
    {
        activeDice = dice.ToList();
        ResetDiceServerRpc();
        AttachDice();
    }
}

