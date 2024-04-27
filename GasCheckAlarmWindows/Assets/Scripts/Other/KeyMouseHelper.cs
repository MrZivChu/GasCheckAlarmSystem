using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMouseHelper : MonoBehaviour
{
    public GameObject HandleLogPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            HandleLogPanel.SetActive(!HandleLogPanel.activeSelf);
        }
    }
}
