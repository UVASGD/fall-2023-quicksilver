using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class EndOfLevel : MonoBehaviour
{
    private HighscoreHandler hsHandler;
    private PlayerMovement pm;
    private HighscoreUI hsUI;
    private PlayerUI playerUI;
    private HighscoreElement score;
    public TMP_InputField input;
    public GameObject highscore_panel;
    float timer = 10.0f;
    bool flag = false;

    private void Start()
    {
        pm = FindAnyObjectByType<PlayerMovement>();
        hsHandler = FindObjectOfType<HighscoreHandler>();
        hsUI = FindObjectOfType<HighscoreUI>();
        playerUI = FindObjectOfType<PlayerUI>();
        score = new HighscoreElement("", 0, SceneManager.GetActiveScene().buildIndex);
        input.gameObject.SetActive(false);
        timer = 10.0f;
    }

    private void Update()
    {
        score.points += Time.deltaTime;
        playerUI.UpdateScore(score.points);
        if (input.gameObject.active)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                score.playerName = input.text;
                hsHandler.AddHighscoreIfPossible(score);
                highscore_panel.SetActive(true);
                input.gameObject.SetActive(false);
            }
        }
        if (highscore_panel.gameObject.active && Input.GetKeyDown(KeyCode.Space))
        { 
            Time.timeScale = 1.0f;
            pm.enabled = true;
            playerUI.gameObject.SetActive(true);
            if (SceneManager.GetActiveScene().buildIndex < 5)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            else
                SceneManager.LoadScene(0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "Player")
        {
            pm.enabled = false;
            playerUI.gameObject.SetActive(false);
            input.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(input.gameObject);
            Time.timeScale = 0;
            //hsHandler.AddHighscoreIfPossible()
        }
    }
}
