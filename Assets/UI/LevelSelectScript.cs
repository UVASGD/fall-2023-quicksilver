using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectScript : MonoBehaviour
{
    // TODO: add level thumbnails to each one
    public void Level1()
    {
        SceneManager.LoadScene("Level1"); // TODO: perhaps eventually have views automatically update, perhaps a Portal 2-like "panel" view of levels that updates automatically
    }
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
