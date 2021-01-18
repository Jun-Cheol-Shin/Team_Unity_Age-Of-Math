using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Network.Data;



public class GameStartManager : MonoBehaviour
{
    private float StartCountDown = 4f;                  // 3초

    public GameObject CountDownText;
    private MapMaker mapMaker;

    public GameObject Player;

    public GameObject Fire;
    public GameObject Wind;
    public GameObject Water;

    public GameObject FireMark;
    public GameObject WindMark;
    public GameObject WaterMark;

    public GameObject FireEffect;
    public GameObject WindEffect;
    public GameObject WaterEffect;

    public GameObject mychar;
    public GameObject mymark;
    public GameObject myEffect;
    public GameObject othermark;
    public GameObject otherchar;
    public GameObject otherEffect;

    public GameObject Nowposobj;
    public Vector3 Nowpos;
    public GameObject Nowotherobj;
    public Vector3 Nowotherpos;

    public bool clickpossible = false;

    private void Awake()
    {
        mapMaker = GameObject.Find("MapMaker").GetComponent<MapMaker>();
        Manager_Network.Instance.e_GameStart.AddListener(new UnityAction(GameStart));

        my = Manager_Network.Instance.m_Client_CharacterIndex;
        other = Manager_Network.Instance.m_OtherPlayer_CharacterIndex;
        position = Manager_Network.Instance.m_Client_Position;
        switch (position)
        {
            case 1:
                other_position = 2;
                break;
            case 2:
                other_position = 1;
                break;
        }
        MarkSelect();
    }

    public ulong position;
    public ulong other_position;
    public ulong my;
    public ulong other;
    IEnumerator Count()
    {
        mychar = CharacterCreate(my);
        mychar.GetComponent<PlayerFunction>().Event_Insert();
        otherchar = CharacterCreate(other);
        Characterposition(position, mychar, otherchar);

        Fader.End_Fade();
        yield return new WaitForSecondsRealtime(1f);

        CountDownText.SetActive(true);
        Manager_Sound.Instance.Play_SE(SE_INDEX.SIGN_COUNTDOWN);

        yield return new WaitForSecondsRealtime(3f);

        Debug.Log("게임 시작");

        yield return new WaitForSecondsRealtime(1f);

        CountDownText.SetActive(false);


        Manager_Sound.Instance.Play_BGM(BGM_INDEX.INGAME);
        clickpossible = true;

        yield return null;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        Sender.Send_Ready(); // Network
    }

    public void GameStart()
    {
        StartCoroutine(Count());
    }

    private GameObject CharacterCreate(ulong my)
    {
        GameObject cha = null;
        switch (my)
        {
            case 1:
                cha = Instantiate(Wind, transform.localPosition, transform.localRotation);
                break;
            case 2:
                cha = Instantiate(Fire, transform.localPosition, transform.localRotation);
                break;
            case 3:
                cha = Instantiate(Water, transform.localPosition, transform.localRotation);
                break;
        }
        return cha;
    }
    private void MarkSelect()
    {
        switch (Manager_Network.Instance.m_Client_CharacterIndex)
        {
            case 1:
                mymark = WindMark;
            myEffect = WindEffect;
                break;
            case 2:
                mymark = FireMark;
            myEffect = FireEffect;
                break;
            case 3:
                mymark = WaterMark;
            myEffect = WaterEffect;
                break;
        }

        switch (Manager_Network.Instance.m_OtherPlayer_CharacterIndex)
        {
            case 1:
                othermark = WindMark;
            otherEffect = WindEffect;
                break;
            case 2:
                othermark = FireMark;
            otherEffect = FireEffect;
                break;
            case 3:
                othermark = WaterMark;
            otherEffect = WaterEffect;
                break;
        }
    }
    private void Characterposition(ulong position, GameObject mychar, GameObject otherchar)
    {
        if (position == 1)
        {
            mychar.transform.localPosition = new Vector3(mapMaker.Maps[7][0].transform.localPosition.x,
                mapMaker.Maps[7][0].transform.localPosition.y, mapMaker.Maps[7][0].transform.localPosition.z);
            otherchar.transform.localPosition = new Vector3(mapMaker.Maps[0][7].transform.localPosition.x,
                mapMaker.Maps[0][7].transform.localPosition.y, mapMaker.Maps[0][7].transform.localPosition.z);
            Nowposobj = mapMaker.Maps[7][0];
            Nowotherobj = mapMaker.Maps[0][7];

        }
        else
        {
            mychar.transform.localPosition = new Vector3(mapMaker.Maps[0][7].transform.localPosition.x,
                mapMaker.Maps[0][7].transform.localPosition.y, mapMaker.Maps[0][7].transform.localPosition.y);
            otherchar.transform.localPosition = new Vector3(mapMaker.Maps[7][0].transform.localPosition.x,
                mapMaker.Maps[7][0].transform.localPosition.y, mapMaker.Maps[7][0].transform.localPosition.y);
            Nowposobj = mapMaker.Maps[0][7];
            Nowotherobj = mapMaker.Maps[7][0];

        }

        Nowpos = Nowposobj.transform.localPosition;
        Nowotherpos = Nowotherobj.transform.localPosition;
    }
    
}
