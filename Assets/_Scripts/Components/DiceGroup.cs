using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceGroup : NetworkBehaviour, IInteractable
{
    public Vector3 StartPos { get; private set; }
    public GameObject GameObject => gameObject;
    public bool IsActiveObj { get; set; }

    private DieBehavior[] dice;

    [SerializeField] private Transform[] dicePositions;
    private InputAction selectAction;

    private bool isWatched;
    private bool isHolded;
    private bool hasReacheActivePos;


    private void Awake()
    {
        dice = GetComponentsInChildren<DieBehavior>();
        CastRay.OnHoveredStay += (RaycastHit hit) =>
        {
            if (hit.collider.gameObject == gameObject && hasReacheActivePos && !isHolded)
            {
                isWatched = true;
            }
        };

        CastRay.OnHoveredExit += (RaycastHit hit) =>
        {
            if (isHolded)
                return;

            isWatched = false;
        };
    }

    private void Start()
    {
        StartPos = transform.position;
    }


    public void PositionReached()
    {
        hasReacheActivePos = true;

        foreach (DieBehavior d in dice)
        {
            d.MoveAroundServerRpc(transform.position);
        }
    }

    public void LeftMousePress()
    {
        //if (isWatched)
        //{
        //    isHolded = true;
        //    TakeDice();
        //    isWatched = false;
        //}
    }

    public void LeftMouseHold()
    {
        if (isWatched)
        {
            isHolded = true;
            TakeDice();
            isWatched = false;
        }
        else if (isHolded)
        {
            transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position + Camera.main.transform.forward * 0.5f, 5.0f * Time.deltaTime);
        }
    }

    public void LeftMouseRelese()
    {
        if (isHolded)
        {
            isHolded = false;
            hasReacheActivePos = false;
            IsActiveObj = false;
            LunchDice();
        }
    }

    public void ReleseBackPressed()
    {
        ResetDice();
    }

    private void TakeDice()
    {
        NetworkObject player = NetworkManager.Singleton.LocalClient.PlayerObject;
        player.GetComponent<RotateHead>().PartialLock();

        HoldDice();
    }

    private void LunchDice()
    {
        foreach (DieBehavior die in dice)
        {
            die.LunchServerRpc(Camera.main.transform.forward);
            die.UnHoldServerRpc();
        }
    }

    private void HoldDice()
    {
        foreach (DieBehavior die in dice)
        {
            die.HoldServerRpc();
        }
    }

    [ContextMenu("ResetDice")]
    private void ResetDice()
    {
        IsActiveObj = false;

        for (int i = 0; i < dice.Length; i++)
        {
            dice[i].ResetDie(dicePositions[i]);
        }
    }
}

