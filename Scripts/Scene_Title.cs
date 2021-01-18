using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scene_Title : MonoBehaviour
{
    public enum TITLE_STATE { MAIN = 0, SETTING, MANUAL, CONNECT }

    public TITLE_STATE m_State;

    public RectTransform m_BackImage;
    public GameObject m_IntroObject;

    public GameObject w_Title;
    public GameObject w_Setting;
    public GameObject w_Manual;
    public GameObject w_Connect;

    public Slider slider_BGM;
    public Slider slider_SE;
    public InputField in_IP;
    public InputField in_Port;

    public GameObject[] m_SCGs;

    public float m_Cam_shake_time;
    public float m_Cam_shake_power;

    private void Awake()
    {
        slider_BGM.value = Option.m_BGM_Volume;
        slider_SE.value = Option.m_SE_Volume;
    }
    
    IEnumerator Start()
    {
        // IP와 포트를 전에 입력했던 것으로 설정
        in_IP.text = PlayerPrefs.GetString("Connection_Server_IP", "101.235.109.24");
        in_Port.text = PlayerPrefs.GetString("Connection_Server_Port", "10000");

        Option.Load_Option();

        if (!Option.m_FirstBoot)
        {
            Option.m_FirstBoot = true;
            m_IntroObject.SetActive(true);

            float time = 0f;
            while(time < 31f)
            {
                time += Time.deltaTime;

                if (Input.touchCount > 0 || Input.GetMouseButton(0))
                    break;

                yield return new WaitForEndOfFrame();
            }
        }

        m_IntroObject.SetActive(false);
        Fader.End_Fade();
        Manager_Sound.Instance.Play_BGM(BGM_INDEX.TITLE);
        GetComponent<Animation>().Play();
    }

    void Update()
    {
        if(m_Cam_shake_time > 0f)
        {
            m_Cam_shake_time -= Time.deltaTime;
            m_BackImage.anchoredPosition = new Vector2(Random.Range(-m_Cam_shake_power, m_Cam_shake_power), Random.Range(-m_Cam_shake_power, m_Cam_shake_power));
        }
        else
        {
            m_BackImage.anchoredPosition = new Vector2();
        }
    }

    #region STATE

    public void Change_State(TITLE_STATE _state)
    {
        m_State = _state;

        w_Title.SetActive(m_State == TITLE_STATE.MAIN);
        w_Setting.SetActive(m_State == TITLE_STATE.SETTING);
        w_Manual.SetActive(m_State == TITLE_STATE.MANUAL);
        w_Connect.SetActive(m_State == TITLE_STATE.CONNECT);

        switch (m_State)
        {
            case TITLE_STATE.MAIN:
                break;
            case TITLE_STATE.SETTING:
                break;
            case TITLE_STATE.MANUAL:
                break;
            case TITLE_STATE.CONNECT:
                break;
        }

        Option.Save_Option();
        Manager_Sound.Instance.Play_SE(SE_INDEX.BUTTON_SOUND);
    }

    public void Go_Main() { Change_State(TITLE_STATE.MAIN); }

    public void Go_Setting() { Change_State(TITLE_STATE.SETTING); }

    public void Go_Manual() { Change_State(TITLE_STATE.MANUAL); }

    public void Go_Connect() { Change_State(TITLE_STATE.CONNECT); }

    #endregion

    public void Shake_Camera()
    {
        m_Cam_shake_time = 0.1f;
        m_Cam_shake_power = 10f;
    }

    // SCG 애니메이션
    public void Fade_In_Chara(int _index)
    {
        Manager_Sound.Instance.Play_SE(SE_INDEX.CHARACTER_SELECT);
        for (int i = 0; i < 3; i++)
            m_SCGs[i].GetComponent<Animation>().Play(i == _index - 1 ? "SCG_FadeIn" : "SCG_FadeOut");
        Manager_Network.Instance.m_Client_CharacterIndex = (ulong)_index;
    }

    // IP와 포트 저장
    public void Save_IP_Port()
    {
        PlayerPrefs.SetString("Connection_Server_IP", in_IP.text);
        PlayerPrefs.SetString("Connection_Server_Port", in_Port.text);
        PlayerPrefs.Save();
    }
    
    // 서버에 연결
    public void Connect_To_Server()
    {
        Manager_Sound.Instance.Play_SE(SE_INDEX.BUTTON_SOUND);
        if (Manager_Network.Instance.m_Client_CharacterIndex == 0)
        {
            Window_Popup.CreateWindow("연결 실패", "캐릭터를 선택해야 해요.", WINDOW_BUTTON_TYPE.OK, null, null);
            return;
        }
        Save_IP_Port();
        Manager_Network.Instance.Connect_To_Server(in_IP.text, in_Port.text);
    }

    bool quit = false;
    public void QuitApplication()
    {
        if (quit)
            return;
        quit = true;
        StartCoroutine(Quit_Process());
    }

    IEnumerator Quit_Process()
    {
        Manager_Sound.Instance.Play_SE(SE_INDEX.GOODBYE_1 + Random.Range(0, 1));

        yield return new WaitForSeconds(2f);

        Application.Quit();
    }

    public void Set_BGM()
    {
        Option.m_BGM_Volume = slider_BGM.value;
        Manager_Sound.Instance.m_BGM.volume = Option.m_BGM_Volume;
    }

    public void Set_SE()
    {
        Option.m_SE_Volume = slider_SE.value;
    }
}
