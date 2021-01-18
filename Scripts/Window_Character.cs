using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Network.Data;

public class Window_Character : MonoBehaviour
{
    public int m_Position;
    public Text m_ScoreText;
    public Text m_TileText;
    public Text m_MultText;

    public Animation m_SkillAnim;
    public Text m_SkillText;

    public Animation m_Anim;
    public GameObject m_YouSign;
    public Image m_SCG_BackImage;
    public Image[] m_Battery;
    public GameObject[] m_SCGs;
    public GameObject[] m_Marks;

    bool m_is_Client = false;
    UInt64 m_CharaIndex = 0;
    UnityAction<ushort, ScoreData> m_Score;
    UnityAction<ushort, ushort> m_Skill;

    // Start is called before the first frame update
    void Awake()
    {
        if(Manager_Network.Instance == null)
        {
            Debug.Log("Window_Character - Cannot Find Network Manager Object");
            return;
        }
        m_Score = new UnityAction<ushort, ScoreData>(Update_Score);
        m_Skill = new UnityAction<ushort, ushort>(Use_Skill);
        Manager_Network.Instance.e_Player_UpdateScore.AddListener(m_Score);
        Manager_Network.Instance.e_Player_UseSkill.AddListener(m_Skill);
        m_is_Client = Manager_Network.Instance.m_Client_Position == m_Position;
        
        if(m_is_Client)
            m_CharaIndex = Manager_Network.Instance.m_Client_CharacterIndex;
        else
            m_CharaIndex = Manager_Network.Instance.m_OtherPlayer_CharacterIndex;

        m_YouSign.SetActive(m_is_Client);
        m_SCGs[m_CharaIndex - 1].SetActive(true);
        m_Marks[m_CharaIndex - 1].SetActive(true);

        m_TileText.text = "0";

        StartCoroutine(BackImage_Coloring());
        Initialize_Battery();
        m_Anim.Play();
    }

    IEnumerator BackImage_Coloring()
    {
        Color color = new Color(1f, 1f, 1f, 1f);
        switch(m_CharaIndex)
        {
            case 1: // 풀
                color = new Color(0.4f, 0.8f, 0.4f, 1f);
                break;
            case 2: // 불
                color = new Color(0.8f, 0.4f, 0.4f, 1f);
                break;
            case 3: // 물
                color = new Color(0.4f, 0.4f, 0.8f, 1f);
                break;
        }

        for(float i = 0f; i < 2f; i += Time.deltaTime)
        {
            m_SCG_BackImage.color = Color.Lerp(m_SCG_BackImage.color, color, 0.1f);
            yield return new WaitForEndOfFrame();
        }
        m_SCG_BackImage.color = color;

        yield return null;
    }

    void Initialize_Battery()
    {
        switch (m_CharaIndex)
        {
            case 1:
                m_Battery[0].color = new Color(34f / 255f, 204f / 255f, 29f / 255f);
                m_Battery[1].color = new Color(76f / 255f, 237f / 255f, 70f / 255f);
                m_Battery[2].color = new Color(68f / 255f, 255f / 255f, 61f / 255f);
                m_Battery[3].color = new Color(73f / 255f, 255f / 255f, 67f / 255f);
                m_Battery[4].color = new Color(109f / 255f, 255f / 255f, 103f / 255f);
                m_Battery[5].color = new Color(144f / 255f, 255f / 255f, 139f / 255f);
                m_Battery[6].color = new Color(173f / 255f, 255f / 255f, 169f / 255f);
                m_Battery[7].color = new Color(202f / 255f, 255f / 255f, 199f / 255f);
                m_Battery[8].color = new Color(244f / 255f, 255f / 255f, 243f / 255f);
                break;
            case 2:
                m_Battery[0].color = new Color(255f / 255f, 27f / 255f, 1f / 255f);
                m_Battery[1].color = new Color(255f / 255f, 67f / 255f, 43f / 255f);
                m_Battery[2].color = new Color(255f / 255f, 101f / 255f, 82f / 255f);
                m_Battery[3].color = new Color(255f / 255f, 136f / 255f, 121f / 255f);
                m_Battery[4].color = new Color(255f / 255f, 168f / 255f, 157f / 255f);
                m_Battery[5].color = new Color(255f / 255f, 189f / 255f, 181f / 255f);
                m_Battery[6].color = new Color(255f / 255f, 213f / 255f, 208f / 255f);
                m_Battery[7].color = new Color(255f / 255f, 234f / 255f, 232f / 255f);
                m_Battery[8].color = new Color(255f / 255f, 252f / 255f, 252f / 255f);
                break;
            case 3:
                m_Battery[0].color = new Color(28f / 255f, 216f / 255f, 207f / 255f);
                m_Battery[1].color = new Color(33f / 255f, 231f / 255f, 222f / 255f);
                m_Battery[2].color = new Color(49f / 255f, 255f / 255f, 246f / 255f);
                m_Battery[3].color = new Color(82f / 255f, 255f / 255f, 247f / 255f);
                m_Battery[4].color = new Color(106f / 255f, 255f / 255f, 248f / 255f);
                m_Battery[5].color = new Color(145f / 255f, 255f / 255f, 250f / 255f);
                m_Battery[6].color = new Color(175f / 255f, 255f / 255f, 251f / 255f);
                m_Battery[7].color = new Color(205f / 255f, 255f / 255f, 252f / 255f);
                m_Battery[8].color = new Color(232f / 255f, 255f / 255f, 254f / 255f);
                break;
        }
    }

    void Update_Score(ushort _index, ScoreData _score)
    {
        if(m_Position == _index)
        {
            m_ScoreText.text = "" + _score.total_score;
            m_MultText.text = "x" + _score.multiplier.ToString("F1");
            m_TileText.text = "" + _score.current_tile;

            for(int i = 0; i < 9; i++)
                m_Battery[i].gameObject.SetActive(i < _score.combo);

            if(_score.combo > 0)
                Manager_Sound.Instance.Play_SE(SE_INDEX.COMBO_COUNT);
        }
    }

    void Use_Skill(ushort _index, ushort _skill_index)
    {
        if(m_Position == _index)
        {
            Manager_Sound.Instance.Play_SE(SE_INDEX.EFFECT_SKILL);
            switch(m_CharaIndex)
            {
                case 1:
                    switch(_skill_index)
                    {
                        case 1:
                            m_SkillText.text = "새싹";
                            break;
                        case 2:
                            m_SkillText.text = "나무";
                            break;
                        case 3:
                            m_SkillText.text = "숲숲";
                            break;
                    }
                    break;
                case 2:
                    switch (_skill_index)
                    {
                        case 1:
                            m_SkillText.text = "약불";
                            break;
                        case 2:
                            m_SkillText.text = "중불";
                            break;
                        case 3:
                            m_SkillText.text = "강불";
                            break;
                    }
                    break;
                case 3:
                    switch (_skill_index)
                    {
                        case 1:
                            m_SkillText.text = "이슬";
                            break;
                        case 2:
                            m_SkillText.text = "강물";
                            break;
                        case 3:
                            m_SkillText.text = "홍수";
                            break;
                    }
                    break;
            }
            m_SkillAnim.Play();

            if (m_SkillText.text.Equals("홍수") && m_Position != Manager_Network.Instance.m_Client_Position)
            {
                // 스크린 얼리기
                StartCoroutine(Water_Skill());
            }
            if (m_SkillText.text.Equals("강불"))
            {
                // 맵 가운데에 불 내기
                StartCoroutine(Fire_Skill());
            }
        }
    }
    IEnumerator Water_Skill()
    {
        PlayerFunction.Water_Skill_Used = true;

        GameObject water = Instantiate(Resources.Load<GameObject>("Prefab/Skill/Water_Skill"));
        Destroy(water, 5f);

        yield return new WaitForSecondsRealtime(3f);
        PlayerFunction.Water_Skill_Used = false;

        yield return null;
    }

    IEnumerator Fire_Skill()
    {
        PlayerFunction.Fire_Skill_Used = true;

        GameObject fire = Resources.Load<GameObject>("Prefab/Skill/Fire_Skill");
        for (int i = 1; i < 7; i++)
        {
            GameObject fire_1 = Instantiate(fire, MapMaker.Instance.Maps[i][3].transform.localPosition, MapMaker.Instance.Maps[i][3].transform.localRotation);
            GameObject fire_2 = Instantiate(fire, MapMaker.Instance.Maps[i][4].transform.localPosition, MapMaker.Instance.Maps[i][4].transform.localRotation);
            Destroy(fire_1, 10f);
            Destroy(fire_2, 10f);
        }

        yield return new WaitForSecondsRealtime(10f);
        PlayerFunction.Fire_Skill_Used = false;

        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
