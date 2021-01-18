using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MarkManager : MonoBehaviour
{

    GameStartManager GameManager;
    MapMaker MatrixManager;

    private void Awake()
    {
        GameManager = GameObject.Find("StartManager").GetComponent<GameStartManager>();
        MatrixManager = GameObject.Find("MapMaker").GetComponent<MapMaker>();
    }

    private void Start()
    {
        Manager_Network.Instance.e_Player_Mark.AddListener(new UnityAction<ushort, ulong, ulong>(Marking));
        Manager_Network.Instance.e_Player_Mark_Erase.AddListener(new UnityAction<ushort>(MarkErasing));
    }

    public void Marking(ushort _player, ulong _x, ulong _y)
    {
        Debug.LogFormat("마킹하세요");
        if(GameManager.position == _player)
        {
            GameManager.mymark.transform.localPosition = MatrixManager.Maps[_y][_x].transform.localPosition;
            if(!GameManager.mymark.activeSelf)
            {
                GameManager.mymark.SetActive(true);
            }
        }

        else if(GameManager.other_position == _player)
        {
            GameManager.othermark.transform.localPosition = MatrixManager.Maps[_y][_x].transform.localPosition;
            if(!GameManager.othermark.activeSelf)
            {
                GameManager.othermark.SetActive(true);
            }
        }
    }

    public void MarkErasing(ushort _player)
    {
        Debug.LogFormat("마크를 지우세요");
        if(_player == GameManager.position)
        {
            GameManager.mymark.SetActive(false);
        }

        else if(_player == GameManager.other_position)
        {
            GameManager.othermark.SetActive(false);
        }
    }
}
