using System.Collections;
using UnityEngine;

public class DiceGroup : MonoBehaviour, IInteractable
{
    public Vector3 StartPos { get; private set; }

    public GameObject GameObject => gameObject;

    private DieBehavior[] dices;

    [SerializeField] private Transform[] dicesPos;

    private void Awake()
    {
        dices = GetComponentsInChildren<DieBehavior>();
    }

    private void Start()
    {
        StartPos = transform.position;
    }

    public void ResetPos()
    {
        InvokeRepeating(nameof(GoBack), 0, Time.deltaTime);
    }

    private void GoBack()
    {
        transform.position = Vector3.Lerp(transform.position, StartPos, 5 * 2.5f);

        if ((transform.position - StartPos).magnitude < 0.001f)
        {
            CancelInvoke(nameof(GoBack));
        }
    }

    public void OnPositionReached()
    {
        foreach (DieBehavior d in dices)
        {
            d.MoveAround(transform.position);
        }
    }
}

