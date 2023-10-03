using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TestUI : MonoBehaviour
{
    public TextMeshProUGUI speed;
    public TextMeshProUGUI state;

    public PlayerMovementStateMachine pm;

    private void Update()
    {
        speed.text = "Speed: " + (Mathf.Round(pm.move.rb.velocity.magnitude * 100f) *0.01f).ToString();
        state.text = pm.state.ToString();
    }
}
