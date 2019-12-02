using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentScore : MonoBehaviour {

    public int points = 0;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
