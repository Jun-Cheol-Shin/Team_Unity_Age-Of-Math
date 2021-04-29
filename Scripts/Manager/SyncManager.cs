using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SyncManager : MonoBehaviour
{

    GameStartManager GameManager;
    MarkManager MarkManager;
    MapMaker MatrixManager;

    private void Awake()
    {
        GameManager = GameObject.Find("StartManager").GetComponent<GameStartManager>();
        MarkManager = GameObject.Find("MarkManager").GetComponent<MarkManager>();
        MatrixManager = GameObject.Find("MapMaker").GetComponent<MapMaker>();
    }

    //private int player;
    //private ulong x;
    //private ulong y;
    //private bool moving;
    private float t;

    float startTime;
    float divide = 0;
    //IEnumerator enumerator;

    private void Start()
    {
        //enumerator = MoveCoroutine();
        Manager_Network.Instance.e_Player_Move.AddListener(new UnityAction<ushort, ulong, ulong>(Jump));
    }

    IEnumerator MoveCoroutine(ulong x, ulong y, ushort player)
    {
        // 상대 캐릭터 이동 함수
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        while (!other_Move_tile(x, y, player))
        {
            yield return waitForEndOfFrame;
        }

    }

    public void Jump(ushort _player, ulong _x, ulong _y)
    {
        if (_player == GameManager.other_position)
        {
            Manager_Sound.Instance.Play_SE(SE_INDEX.JUMP);
            StartCoroutine(MoveCoroutine(_x, _y, _player));
        }
    }


    bool other_Move_tile(ulong x, ulong y, ushort player)
    {
        if (GameManager.other_position == (ulong)player)
        {
            MarkManager.MarkErasing((ushort)player);
            Vector3 center = (GameManager.Nowotherobj.transform.localPosition + MatrixManager.Maps[y][x].transform.localPosition) * 0.5f;
            if (MatrixManager.Maps[y][x].transform.localPosition.x == GameManager.Nowotherobj.transform.localPosition.x)
            {
                center.x -= 1f;

                if (GameManager.Nowotherobj.transform.localPosition.y < MatrixManager.Maps[y][x].transform.localPosition.y)
                {
                    if (!GameManager.otherchar.transform.GetChild(1).gameObject.activeSelf)
                    {
                        GameManager.otherchar.transform.GetChild(0).gameObject.SetActive(false);
                        GameManager.otherchar.transform.GetChild(1).gameObject.SetActive(true);
                        GameManager.otherchar.transform.GetChild(2).gameObject.SetActive(false);
                        GameManager.otherchar.transform.GetChild(3).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (GameManager.Nowotherobj.transform.localPosition.x > MatrixManager.Maps[y][x].transform.localPosition.x)
                {
                    if (!GameManager.otherchar.transform.GetChild(2).gameObject.activeSelf)
                    {
                        GameManager.otherchar.transform.GetChild(0).gameObject.SetActive(false);
                        GameManager.otherchar.transform.GetChild(1).gameObject.SetActive(false);
                        GameManager.otherchar.transform.GetChild(2).gameObject.SetActive(true);
                        GameManager.otherchar.transform.GetChild(3).gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (!GameManager.otherchar.transform.GetChild(3).gameObject.activeSelf)
                    {
                        GameManager.otherchar.transform.GetChild(0).gameObject.SetActive(false);
                        GameManager.otherchar.transform.GetChild(1).gameObject.SetActive(false);
                        GameManager.otherchar.transform.GetChild(2).gameObject.SetActive(false);
                        GameManager.otherchar.transform.GetChild(3).gameObject.SetActive(true);
                    }
                }
                center.y -= 1f;
            }


            if(GameManager.Nowotherobj.transform.localPosition ==
                MatrixManager.Maps[y][x].transform.localPosition)
            {
                return true;
            }

            Vector3 RelCenter = GameManager.Nowotherobj.transform.localPosition - center;
            Vector3 aimRelCenter = MatrixManager.Maps[y][x].transform.localPosition - center;

            if (divide == 0)
            {
                divide = Vector3.Distance(
                    GameManager.Nowotherobj.transform.localPosition,
                    MatrixManager.Maps[y][x].transform.localPosition) * 0.2f;
                startTime = Time.time;
            }

            float fracComplete = (Time.time - startTime) / divide;
            Mathf.Clamp(fracComplete, 0, 1);

            if (fracComplete != 0)
            {
                GameManager.otherchar.transform.localPosition = Vector3.Slerp(RelCenter, aimRelCenter, fracComplete);
                GameManager.otherchar.transform.localPosition += center;
            }

            if(GameManager.otherchar.transform.localPosition ==
                MatrixManager.Maps[y][x].transform.localPosition)
            {
                divide = 0;
            }

            //if (t < 1f)
            //{
            //    t += 0.04f;
            //}
            //else
            //{
            //    t = 0f;
            //}

            if (GameManager.otherchar.transform.localPosition == MatrixManager.Maps[y][x].transform.localPosition)
            {
                Manager_Sound.Instance.Play_SE(SE_INDEX.TILE_CHANGE);
                GameManager.otherchar.transform.GetChild(0).gameObject.SetActive(true);
                GameManager.otherchar.transform.GetChild(1).gameObject.SetActive(false);
                GameManager.otherchar.transform.GetChild(2).gameObject.SetActive(false);
                GameManager.otherchar.transform.GetChild(3).gameObject.SetActive(false);
                GameManager.Nowotherobj = MatrixManager.Maps[y][x];
                GameManager.Nowotherpos = new Vector3(GameManager.Nowotherobj.transform.localPosition.x,
                    GameManager.Nowotherobj.transform.localPosition.y, GameManager.Nowotherobj.transform.localPosition.z);
                t = 0f;
                //player = -1;

                return true;
            }
        }

        else
        {
            return true;
        }

        return false;
    }
    //private void Update()
    //{
    //    //if (moving)
    //    //{
    //    //    other_Move_tile();
    //    //}
    //}
}
