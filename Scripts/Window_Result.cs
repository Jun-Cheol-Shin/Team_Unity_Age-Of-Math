using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Network.Data;

public class Window_Result : MonoBehaviour
{
    public static bool m_Surrender_Win = false;

    public GameObject m_StopSign;
    public Animation m_Anim;
    
    public Text[] m_Lefts;
    public Text[] m_Rights;

    public GameObject m_Win_Window;
    public GameObject m_Draw_Window;
    public GameObject m_Lose_Window;

    ScoreData m_LeftData;
    ScoreData m_RightData;

    void Start()
    {
        m_Surrender_Win = false;
        Manager_Network.Instance.e_Player_UpdateScore.AddListener(new UnityAction<ushort, ScoreData>(Update_Score));
        Manager_Network.Instance.e_GameEnd.AddListener(new UnityAction( () =>
        {
            StartCoroutine(End_Process());
        }));
    }

    void Update_Score(ushort _index, ScoreData _score)
    {
        if (_index == 1)
            m_LeftData = _score;
        else
            m_RightData = _score;

        m_Lefts[0].text = "" + m_LeftData.quiz_score;
        m_Lefts[1].text = "" + m_LeftData.current_tile;
        m_Lefts[2].text = m_LeftData.multiplier.ToString("F1");
        m_Lefts[3].text = "" + m_LeftData.total_score;

        m_Rights[0].text = "" + m_RightData.quiz_score;
        m_Rights[1].text = "" + m_RightData.current_tile;
        m_Rights[2].text = m_RightData.multiplier.ToString("F1");
        m_Rights[3].text = "" + m_RightData.total_score;

        m_Lefts[0].fontSize = m_LeftData.quiz_score > m_RightData.quiz_score ? 64 : 48;
        m_Rights[0].fontSize = m_RightData.quiz_score > m_LeftData.quiz_score ? 64 : 48;

        m_Lefts[1].fontSize = m_LeftData.current_tile > m_RightData.current_tile ? 64 : 48;
        m_Rights[1].fontSize = m_RightData.current_tile > m_LeftData.current_tile ? 64 : 48;

        m_Lefts[2].fontSize = m_LeftData.multiplier > m_RightData.multiplier ? 64 : 48;
        m_Rights[2].fontSize = m_RightData.multiplier > m_LeftData.multiplier ? 64 : 48;

        m_Lefts[3].fontSize = m_LeftData.total_score > m_RightData.total_score ? 64 : 48;
        m_Rights[3].fontSize = m_RightData.total_score > m_LeftData.total_score ? 64 : 48;
        
        m_Draw_Window.SetActive(m_LeftData.total_score == m_RightData.total_score);
        if(Manager_Network.Instance.m_Client_Position == 1)
        {
            m_Win_Window.SetActive(m_LeftData.total_score > m_RightData.total_score);
            m_Lose_Window.SetActive(m_LeftData.total_score < m_RightData.total_score);
        }
        else
        {
            m_Win_Window.SetActive(m_LeftData.total_score < m_RightData.total_score);
            m_Lose_Window.SetActive(m_LeftData.total_score > m_RightData.total_score);
        }
    }

    IEnumerator End_Process()
    {
        if(m_Surrender_Win)
            Update_Score((ushort)(Manager_Network.Instance.m_Client_Position == 1 ? 2 : 1), new ScoreData());
        Manager_Sound.Instance.Play_SE(SE_INDEX.SIGN_ENDGAME);
        Manager_Sound.Instance.FadeOut_BGM(2f);
        m_StopSign.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        m_StopSign.SetActive(false);
        m_Anim.Play();
        yield return new WaitForSecondsRealtime(7f);
        if (m_Win_Window.activeSelf)
            Manager_Sound.Instance.Play_BGM(BGM_INDEX.ENDGAME_WIN);
        if (m_Draw_Window.activeSelf)
            Manager_Sound.Instance.Play_BGM(BGM_INDEX.ENDGAME_WIN);
        if (m_Lose_Window.activeSelf)
            Manager_Sound.Instance.Play_BGM(BGM_INDEX.ENDGAME_LOSE);
    }

    public void To_Title()
    {
        Manager_Sound.Instance.FadeOut_BGM(1f);
        Fader fader = Fader.Start_Fade("집으로 돌아오는 중...");
        fader.m_FCE.AddListener(new UnityAction(() =>
        {
            SceneManager.LoadSceneAsync("Title");
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
