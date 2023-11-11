using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TestUI : MonoBehaviour
{
    public TextMeshProUGUI speed;
    public TextMeshProUGUI state;

    public PlayerMovement pm;
    public Swinging grapple;
    public GameObject grapple_sign;

    private void Update()
    {
        float fullSpeed = Mathf.Round(pm.rb.velocity.magnitude * 100f) * 0.01f;
        speed.text = fullSpeed.ToString();
        state.text = pm.state.ToString();

        grapple_sign.SetActive(grapple.inRange);
    }
}
