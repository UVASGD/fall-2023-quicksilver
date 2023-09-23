using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticClassAccess : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
    }

    public void LoadMainMenu()
    {
        SceneSwitcher.LoadMainMenu();
    }
}
