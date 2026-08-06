using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class ServicesManager : MonoBehaviour
{
    public static ServicesManager I { get; private set; }

    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(I);
    }

    public async void Authenticate(string username)
    {
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(username);

        await UnityServices.InitializeAsync(initializationOptions);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}
