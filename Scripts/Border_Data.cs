using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border_Data : MonoBehaviour
{
    public GameObject m_Left;
    public GameObject m_Top;
    public GameObject m_Right;
    public GameObject m_Bottom;

    public void Set_Border(int _original, int _left, int _top, int _right, int _bottom)
    {
        m_Left.SetActive(_left != _original);
        m_Top.SetActive(_top != _original);
        m_Right.SetActive(_right != _original);
        m_Bottom.SetActive(_bottom != _original);
    }
}
