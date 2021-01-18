using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance = null;
    public static T GetI
    {
        get
        {
            if(m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType<T>();
                if(m_Instance == null)
                {
                    Debug.LogErrorFormat("매니저가 없음 확인 하세요 ");
                }
            }

            return m_Instance;
        }
        //set;
    }
}

