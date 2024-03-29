﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTwoWinController : MonoBehaviour {

    private UIController ui;

    void Start()
    {
        ui = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UIController>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ui.BeatLevelTwo();
        }
    }
}
