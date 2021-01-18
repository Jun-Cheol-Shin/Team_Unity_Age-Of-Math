using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum WINDOW_BUTTON_TYPE : int { OK = 0, CANCEL, YESNO, NONE };

public class Window_Popup : MonoBehaviour
{
    public static Window_Popup Instance;

    [SerializeField]
    private Text m_Title = null;
    [SerializeField]
    private Text m_Desc = null;
    [SerializeField]
    private Button m_Button = null;

    public GameObject[] Buttons;

    /// <summary>
    /// 팝업 윈도우 생성
    /// </summary>
    /// <param name="_title">윈도우 이름</param>
    /// <param name="_desc">윈도우 설명</param>
    /// <returns></returns>
    public static Window_Popup CreateWindow(string _title, string _desc, WINDOW_BUTTON_TYPE _button_type, UnityAction _button_action_1,  UnityAction _button_action_2)
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        GameObject window = Instantiate(Resources.Load<GameObject>("Prefab/Window_Popup"));
        Instance = window.GetComponent<Window_Popup>();
        Instance.Initialize(_title, _desc, _button_type, _button_action_1, _button_action_2);

        return Instance;
    }

    public static void Force_CloseWindow()
    {
        Destroy(Instance.gameObject);
    }

    // 초기화
    public void Initialize(string _title, string _desc, WINDOW_BUTTON_TYPE _button_type, UnityAction _button_action_1, UnityAction _button_action_2)
    {
        m_Title.text = _title;
        m_Desc.text = _desc;
        
        // OK = 0, CANCEL = 1, YESNO = 2,3, NONE = 다 꺼버려
        Buttons[0].SetActive(_button_type == WINDOW_BUTTON_TYPE.OK);
        if(_button_type == WINDOW_BUTTON_TYPE.OK)
            m_Button = Buttons[0].GetComponent<Button>();
        Buttons[1].SetActive(_button_type == WINDOW_BUTTON_TYPE.CANCEL);
        if (_button_type == WINDOW_BUTTON_TYPE.CANCEL)
            m_Button = Buttons[1].GetComponent<Button>();
        Buttons[2].SetActive(_button_type == WINDOW_BUTTON_TYPE.YESNO);
        if (_button_type == WINDOW_BUTTON_TYPE.YESNO)
            m_Button = Buttons[2].GetComponent<Button>();
        Buttons[3].SetActive(_button_type == WINDOW_BUTTON_TYPE.YESNO);
        
        if(_button_action_1 != null)
            m_Button.onClick.AddListener(_button_action_1);
        if (_button_action_2 != null)
            Buttons[3].GetComponent<Button>().onClick.AddListener(_button_action_2);
    }

    public void Set_Text(string _title, string _desc)
    {
        if(_title != "")
            m_Title.text = _title;
        if (_desc != "")
            m_Desc.text = _desc;
    }

    public void Set_Closable(bool _closable)
    {
        Buttons[0].SetActive(_closable);
    }
    
    public void CloseWindow()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
