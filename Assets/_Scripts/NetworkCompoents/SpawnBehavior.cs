using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnBehavior : NetworkBehaviour
{
    PlayerInput input;
    RotateHead rotateHead;
    new Camera camera;
    AudioListener audioListener;
    CastRay castRay;


    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        rotateHead = GetComponent<RotateHead>();   
        camera = GetComponentInChildren<Camera>();
        audioListener = GetComponentInChildren<AudioListener>();
        castRay = GetComponentInChildren<CastRay>();



        input.enabled = false;
        rotateHead.enabled = false;
        camera.enabled = false;
        audioListener.enabled = false;
        castRay.enabled = false;
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
        camera.enabled = true;
        audioListener.enabled = true;
        castRay.enabled = true;
    }
}
