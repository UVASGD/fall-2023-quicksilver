using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadLevel1()
    {
        SceneSwitcher.LoadNextSceneInBuildOrder();
    }

    public void Quit()
    {
        SceneSwitcher.Quit();
    }
}
