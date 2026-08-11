using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnBehavior : NetworkBehaviour
{
    PlayerInput input;
    RotateHead rotateHead;
    [SerializeField] GameObject cameraObj;


    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        rotateHead = GetComponent<RotateHead>();   

        input.enabled = false;
        rotateHead.enabled = false;
        cameraObj.SetActive(false);
    }

    private void Start()
    {
        ActiveInputs();
    }


    private void ActiveInputs()
    {
        if (!IsOwner)
            return;

        input.enabled = true;
        rotateHead.enabled = true;
        cameraObj.SetActive(true);
    }
}
