using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public GameObject base_panel;
    public GameObject level_panel;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LevelPanel()
    {
        base_panel.SetActive(false); 
        level_panel.SetActive(true);
    }

    public void BasePanel()
    {
        base_panel.SetActive(true );
        level_panel.SetActive(false);
    }

    public void LevelSelect(int level_id)
    {
        SceneManager.LoadScene(level_id);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
