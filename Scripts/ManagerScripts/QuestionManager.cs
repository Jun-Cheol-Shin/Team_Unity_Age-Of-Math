using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network.Data;
using UnityEngine.Events;


public enum LEVEL
{
    EASY = 1,
    MIDDLE,
    HARD
};

public class QuestionManager : MonoBehaviour
{
    public GameObject Panel;
    public Text quiztext;
    public Text[] Answer = new Text[4];

    public void Start()
    {
        Manager_Network.Instance.e_QueationBox.AddListener(new UnityAction<Quiz>(Question));
    }

    public void Question(Quiz q)
    {
        Debug.LogFormat("퀴즈 함수 출현");
        Manager_Sound.Instance.Play_SE(SE_INDEX.BUTTON_SOUND);

        quiztext.text = q.m_Question;
        Answer[0].text = q.m_Answer_1;
        Answer[1].text = q.m_Answer_2;
        Answer[2].text = q.m_Answer_3;
        Answer[3].text = q.m_Answer_4;

        if(!Panel.activeSelf)
        {
            Panel.SetActive(true);
        }
    }

}
