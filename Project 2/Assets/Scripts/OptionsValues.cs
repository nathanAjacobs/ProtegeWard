using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsValues : MonoBehaviour {

    public float mouseSens = 2;
    public float controllerSens = 3;
    public float masterVolume = 0;
    public float musicVolume = 0;
    public float sfxVolume = 0;


    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
