using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BGM_INDEX : int
{
    TITLE = 0,
    INGAME,
    ENDGAME_LOSE,
    ENDGAME_WIN
}
public enum SE_INDEX : int
{
    BUTTON_SOUND = 0,
    CHARACTER_SELECT,
    COMBO_COUNT,
    EFFECT_SKILL,
    GOODBYE_1,
    GOODBYE_2,
    JUMP,
    SIGN_COUNTDOWN,
    SIGN_ENDGAME,
    SIGN_GAMESTART,
    SIGN_HURRYUP,
    TILE_CHANGE
}

public class Manager_Sound : MonoBehaviour
{
    public static Manager_Sound Instance;

    public AudioSource m_BGM;
    public List<AudioSource> m_SE;

    public AudioClip[] m_BGM_Clips;
    public AudioClip[] m_SE_Clips;

    public int sound_channels;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 1; i < sound_channels; i++)
        {
            GameObject obj = Instantiate(m_SE[0].gameObject, transform);
            m_SE.Add(obj.GetComponent<AudioSource>());
        }
    }

    public void Play_BGM(BGM_INDEX _index)
    {
        m_BGM.clip = m_BGM_Clips[(int)_index];
        m_BGM.volume = Option.m_BGM_Volume;
        m_BGM.Play();
    }

    public void FadeOut_BGM(float _sec)
    {
        StartCoroutine(FadeOut(_sec));
    }

    IEnumerator FadeOut(float _sec)
    {
        float original_volume = m_BGM.volume;
        for (float time = 0f; time <= _sec; time += Time.deltaTime)
        {
            m_BGM.volume = original_volume * (1f - time / _sec);

            yield return new WaitForEndOfFrame();
        }
        m_BGM.volume = 0f;
        m_BGM.clip = null;

        yield return null;
    }

    public void Play_SE(SE_INDEX _index)
    {
        for(int i = 0; i < sound_channels; i++)
        {
            if (m_SE[i].isPlaying)
                continue;
            m_SE[i].clip = m_SE_Clips[(int)_index];
            m_SE[i].volume = Option.m_SE_Volume;
            m_SE[i].Play();
            break;
        }
    }
}
