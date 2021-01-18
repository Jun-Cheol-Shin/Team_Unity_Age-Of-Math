using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Network.Data;

public class UIButtonManager : MonoBehaviour
{
    public GameObject Question;
    public void GiveUpButton()
    {
        Debug.LogFormat("항복버튼 클릭");
    }

    public void QuestionExitButton()
    {
        Manager_Sound.Instance.Play_SE(SE_INDEX.BUTTON_SOUND);
        int a = -1;
        Sender.Send_Answer((ushort)a);
        if(Question.activeSelf == true)
        {
            Question.SetActive(false);
        }
    }

    public int Answer = -999999;                 // 정답에 해당안되는 숫자로 초기값을 넣는다.
    public void AnswerButton()
    {
        Manager_Sound.Instance.Play_SE(SE_INDEX.BUTTON_SOUND);
        GameObject tempBtn = EventSystem.current.currentSelectedGameObject;
        //Text text = tempBtn.transform.GetChild(0).GetComponent<Text>();
        //Answer = int.Parse(text.text);

        int a = int.Parse(tempBtn.name);
        Debug.LogFormat(a.ToString());

        Sender.Send_Answer((ushort)a);
        // 코루틴 넣어서 정답 유무 확인표시해주고 끈다.
        if(Question.activeSelf == true)
        {
            Question.SetActive(false);
        }

    }
}
