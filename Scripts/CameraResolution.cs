using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //Screen.SetResolution(Screen.width, Screen.width * 16/ 9, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
