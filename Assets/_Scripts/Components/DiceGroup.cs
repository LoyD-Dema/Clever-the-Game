using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceGroup : MonoBehaviour, IInteractable
{
    public Vector3 StartPos { get; private set; }
    public GameObject GameObject => gameObject;
    public bool IsActiveObj { get; set; }

    private DieBehavior[] dices;

    [SerializeField] private Transform[] dicesPos;
    private InputAction selectAction;

    private bool isWatched;
    private bool isHolded;


    private void Awake()
    {
        dices = GetComponentsInChildren<DieBehavior>();
        CastRay.OnHoveredEnter += (RaycastHit hit) =>
        {
            if (hit.collider.gameObject == gameObject && IsActiveObj)
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
        foreach (DieBehavior d in dices)
        {
            d.MoveAround(transform.position);
        }
    }

    public void LeftMousePress()
    {
        if (isWatched)
        {
            isHolded = true;
            TakeDice();
            isWatched = false;
        }
    }

    public void LeftMouseHold()
    {
        transform.position = Vector3.Lerp(transform.position, Camera.main.transform.forward * 5.0f, 5.0f * Time.deltaTime);
    }

    public void LeftMouseRelese()
    {
        isHolded = false;
        LunchDice();
    }

    private void TakeDice()
    {
        NetworkObject player = NetworkManager.Singleton.LocalClient.PlayerObject;
        player.GetComponent<RotateHead>().PartialLock();

        HoldDice();
    }

    private void LunchDice()
    {
        foreach(DieBehavior die in  dices)
        {
            die.Lunch(Camera.main.transform.forward);
            die.UnHold();
        }
    }

    private void HoldDice()
    {
        foreach (DieBehavior die in dices)
        {
            die.Hold();
        }
    }
}

