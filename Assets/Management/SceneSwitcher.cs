using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher
{
    //a class wtih static methods for scene switching

    public static void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void LoadNextSceneInBuildOrder()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public static void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public static void Quit()
    {
        Application.Quit();
    }

    /*
     * other methods to add
     * NextLevel(), PreviousLevel(), LoadLevel(int levelNum), etc
     * LoadMainMenu(), LoadLevelSelect(), LoadOptionsMenu(), etc
     * LoadScene(String name), LoadScene(int buildIndex), etc
     *
     */

}
