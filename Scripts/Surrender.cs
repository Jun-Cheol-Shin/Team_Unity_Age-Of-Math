using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Network.Data;

public class Surrender : MonoBehaviour
{

    public void Lets_Surrender()
    {
        Window_Popup.CreateWindow("항복", "항복할까요?", WINDOW_BUTTON_TYPE.YESNO, new UnityAction(() =>
            {
                Sender.Send_Surrender();
                Manager_Network.Instance.DIsconnect();
                SceneManager.LoadScene("Title");
            }
        ), null);
    }
}
