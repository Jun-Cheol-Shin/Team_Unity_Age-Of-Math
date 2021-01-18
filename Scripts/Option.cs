using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option : MonoBehaviour
{
    public static float m_BGM_Volume = 1f;
    public static float m_SE_Volume = 1f;
    
    public static bool m_FirstBoot = false;

    public static void Load_Option()
    {
        m_BGM_Volume = PlayerPrefs.GetFloat("Option_BGM", 1f);
        m_SE_Volume = PlayerPrefs.GetFloat("Option_SE", 1f);
    }

    public static void Save_Option()
    {
        PlayerPrefs.SetFloat("Option_BGM", m_BGM_Volume);
        PlayerPrefs.SetFloat("Option_SE", m_SE_Volume);
        PlayerPrefs.Save();
    }
}
