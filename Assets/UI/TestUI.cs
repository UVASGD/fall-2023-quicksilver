using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI speed;
    public TextMeshProUGUI state;

    private PlayerMovement pm;
    private Swinging grapple;
    public GameObject grapple_sign;
    public TextMeshProUGUI time;

    private void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        grapple = FindObjectOfType<Swinging>();
    }

    private void Update()
    {
        float fullSpeed = (Mathf.Round(pm.rb.velocity.magnitude * 10f)) * 0.1f;
        speed.text = fullSpeed.ToString();
        state.text = pm.state.ToString();

        grapple_sign.SetActive(grapple.inRange);
    }

    public void UpdateScore(float score)
    {
        score = Mathf.Round((score * 10)) * 0.1f;
        time.SetText(score.ToString());
    }
}
