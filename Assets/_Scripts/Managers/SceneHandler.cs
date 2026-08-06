using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum SceneType
{
    MainMenu,
    PlayScene,
    GameOver,
}

public class SceneHandler : MonoBehaviour
{
    //public static SceneHandler I { get; private set; }
    private static Dictionary<SceneType, string> Scenes;


    // The order of the elements must be the same of the SceneType enum
    [SerializeField] string[] scenesNames;

    private void Awake()
    {
        //if (I == null)
        //    I = this;
        //else
        //    Destroy(I);

        Scenes = new Dictionary<SceneType, string>();

        for (int i = 0; i < scenesNames.Length; i++)
        {
            Scenes.Add((SceneType)i, scenesNames[i]);
        }
    }


    public static void LoadScene(SceneType type)
    {
        if (Scenes.Count <= 0)
        {
            Debug.LogError("No Scenes in the dictionary");
            return;
        }

        SceneManager.LoadScene(Scenes[type]);
    }
}
