using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject StartButton;
    [SerializeField]
    private GameObject BackButton;
    [SerializeField]
    private GameObject[] LevelButtons;
    [SerializeField]
    private GameObject QuitButton;

    private void Start()
    {
        Back();
    }

    public void StartGame()
    {
        StartButton.SetActive(false);
        BackButton.SetActive(true);
        for (int i = 0; i < LevelButtons.Length; i++)
        {
            LevelButtons[i].SetActive(true);
        }
    }

    public void LevelSelect() // TODO: maybe in the future make this take an int level or maybe enum level as an arg to specifically load a certain level?
    {
        SceneManager.LoadScene("Level1");
    }

    public void Quit()
    {
        Application.Quit();
    }

    // Hide level select and back, show Start and Quit
    public void Back()
    {
        for (int i = 0; i < LevelButtons.Length; i++)
        {
            LevelButtons[i].SetActive(false);
        }
        BackButton.SetActive(false);
        StartButton.SetActive(true);
        QuitButton.SetActive(true);
    }
}
