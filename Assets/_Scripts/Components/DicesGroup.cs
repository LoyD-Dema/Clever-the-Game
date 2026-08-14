using System.Collections;
using UnityEngine;

public class DicesGroup : MonoBehaviour, IInteractable
{
    public Vector3 StartPos { get; private set; }

    public GameObject GameObject => gameObject;

    private DiceBehavior[] dices;

    [SerializeField] private Transform[] dicesPos;

    private void Awake()
    {
        dices = GetComponentsInChildren<DiceBehavior>();
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

    public void Relese()
    {
        foreach (DiceBehavior d in dices)
        {
            d.Lunch(transform.forward);
        }
    }
}

