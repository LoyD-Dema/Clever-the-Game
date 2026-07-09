using UnityEngine;

public class Webcam : MonoBehaviour
{
    [SerializeField] private Material webCamtextureMat;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        foreach (var item in devices)
        {
            Debug.Log(item.name);
        }

        if (devices.Length > 0)
        {
            WebCamTexture webCamTexture = new WebCamTexture(devices[0].name, 1920, 1080, 30);

            webCamtextureMat.mainTexture = webCamTexture;

            webCamTexture.Play();
        }

    }
}
