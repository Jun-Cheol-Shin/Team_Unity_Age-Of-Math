using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CountDownManager : MonoBehaviour
{
    public static CountDownManager Instance;

    public Text timeText;
    public ulong time;

    private int Sec = 0;
    private int Min = 0;

    private void Awake()
    {
        Instance = this;
        time = 180000;
        Min = (int)time / 60000;
        Sec = (int)time % 60000 / 1000;

        Manager_Network.Instance.e_Timer.AddListener((ulong _timer) =>
       {
           time = _timer;

           Min = (int)time / 60000;
           Sec = (int)time % 60000 / 1000;
           timeText.text = Min.ToString() + " : " + Sec.ToString("D2");

           if (Min == 0 && Sec == 0)
               timeText.text = "";
       });
    }
}
