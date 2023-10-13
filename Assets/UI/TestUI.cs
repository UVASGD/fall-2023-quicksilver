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
        float fullSpeed = Mathf.Round(pm.rb.velocity.magnitude * 100f) * 0.01f;
        float horSpeed = Mathf.Round(Vector3.ProjectOnPlane(pm.rb.velocity, Vector3.up).magnitude * 100f) * 0.01f;
        speed.text = "Speed: " + fullSpeed.ToString() + "HorSpeed: " + horSpeed.ToString();
        state.text = pm.state.ToString();
    }
}
