using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.Data;
using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.Networking;


public class PlayerFunction : MonoBehaviour
{
    public static bool Fire_Skill_Used = false;
    public static bool Water_Skill_Used = false;

    private UIButtonManager UIManager = null;
    private GameStartManager GameManager = null;
    private MapMaker MatrixManager = null;
    private MarkManager MarkManager = null;
    private SyncManager SyncManager = null;

    float divide = 0;
    float startTime;

    private void Awake()
    {
        UIManager = GameObject.Find("UIManager").GetComponent<UIButtonManager>();
        GameManager = GameObject.Find("StartManager").GetComponent<GameStartManager>();
        MatrixManager = GameObject.Find("MapMaker").GetComponent<MapMaker>();
        MarkManager = GameObject.Find("MarkManager").GetComponent<MarkManager>();
        SyncManager = GameObject.Find("SyncManager").GetComponent<SyncManager>();
    }

    private void Start()
    {
    }

    public void Event_Insert()
    {
        Manager_Network.Instance.e_Player_Move.AddListener(new UnityAction<ushort, ulong, ulong>(Jump));
    }

    private bool moving = false;
    private int x;
    private int y;
    private int player;
    private float t;
    public void Jump(ushort _player, ulong _x, ulong _y)
    {
        Manager_Sound.Instance.Play_SE(SE_INDEX.JUMP);
        Debug.LogFormat("점프 on");
        player = _player;
        moving = true;
    }

    void Move_tile()
    {
        if (GameManager.position == (ulong)player)
        {
            MarkManager.MarkErasing((ushort)player);
            Vector3 center = (GameManager.Nowposobj.transform.localPosition + MatrixManager.Maps[y][x].transform.localPosition) * 0.5f;
            if (MatrixManager.Maps[y][x].transform.localPosition.x == GameManager.Nowposobj.transform.localPosition.x)
            {
                center.x -= 1f;

                if (GameManager.Nowposobj.transform.localPosition.y < MatrixManager.Maps[y][x].transform.localPosition.y)
                {
                    if (!GameManager.mychar.transform.GetChild(1).gameObject.activeSelf)
                    {
                        GameManager.mychar.transform.GetChild(0).gameObject.SetActive(false);
                        GameManager.mychar.transform.GetChild(1).gameObject.SetActive(true);
                        GameManager.mychar.transform.GetChild(2).gameObject.SetActive(false);
                        GameManager.mychar.transform.GetChild(3).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (GameManager.Nowposobj.transform.localPosition.x > MatrixManager.Maps[y][x].transform.localPosition.x)
                {
                    if (!GameManager.mychar.transform.GetChild(2).gameObject.activeSelf)
                    {
                        GameManager.mychar.transform.GetChild(0).gameObject.SetActive(false);
                        GameManager.mychar.transform.GetChild(1).gameObject.SetActive(false);
                        GameManager.mychar.transform.GetChild(2).gameObject.SetActive(true);
                        GameManager.mychar.transform.GetChild(3).gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (!GameManager.mychar.transform.GetChild(3).gameObject.activeSelf)
                    {
                        GameManager.mychar.transform.GetChild(0).gameObject.SetActive(false);
                        GameManager.mychar.transform.GetChild(1).gameObject.SetActive(false);
                        GameManager.mychar.transform.GetChild(2).gameObject.SetActive(false);
                        GameManager.mychar.transform.GetChild(3).gameObject.SetActive(true);
                    }
                }
                center.y -= 1f;
            }

            // 클릭한 위치와 본인의 위치가 같을 경우 함수 탈출
            if (GameManager.Nowposobj.transform.localPosition ==
                MatrixManager.Maps[y][x].transform.localPosition)
            {
                return;
            }

            // Sleap 구현을 위해 출발점과 도착점의 센터점을 지정
            Vector3 RelCenter = GameManager.Nowposobj.transform.localPosition - center;
            Vector3 aimRelCenter = MatrixManager.Maps[y][x].transform.localPosition - center;

            // 처음 한번만 divide와 startTime을 지정
            if (divide == 0)
            {
                divide = Vector3.Distance(
                    GameManager.Nowposobj.transform.localPosition,
                    MatrixManager.Maps[y][x].transform.localPosition) * 0.2f;
                startTime = Time.time;
            }

            // divide가 클 수록 천천히 증가
            float fracComplete = (Time.time - startTime) / divide;
            // 최소 최대 지정
            Mathf.Clamp(fracComplete, 0, 1);

            if (fracComplete != 0)
            {
                GameManager.mychar.transform.localPosition = Vector3.Slerp(RelCenter, aimRelCenter, fracComplete);
                GameManager.mychar.transform.localPosition += center;
            }

            // 도착하면 divide 초기화
            if (GameManager.mychar.transform.localPosition ==
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

            if (GameManager.mychar.transform.localPosition == MatrixManager.Maps[y][x].transform.localPosition)
            {
                Manager_Sound.Instance.Play_SE(SE_INDEX.TILE_CHANGE);
                GameManager.mychar.transform.GetChild(0).gameObject.SetActive(true);
                GameManager.mychar.transform.GetChild(1).gameObject.SetActive(false);
                GameManager.mychar.transform.GetChild(2).gameObject.SetActive(false);
                GameManager.mychar.transform.GetChild(3).gameObject.SetActive(false);

                moving = false;
                GameManager.Nowposobj = MatrixManager.Maps[y][x];
                GameManager.Nowpos = new Vector3(GameManager.Nowposobj.transform.localPosition.x,
                    GameManager.Nowposobj.transform.localPosition.y, GameManager.Nowposobj.transform.localPosition.z);
                t = 0f;
                player = -1;
            }
        }
    }

    private void Update()
    {

        if(GameManager.clickpossible)
        {
            if (moving)
            {
                Move_tile();
            }
            if(!UIManager.Question.activeSelf/* && !clickcheck*/)
            {
                click();
            }
        }
    }
    private bool MatrixSelection(RaycastHit2D hit)
    {
        GameObject[] PossibleMatrix = new GameObject[64];
        GameObject[] PossibleAllMatrix = new GameObject[64];
        int[] PossibleNum = new int[64];

        int pm_count = 0;
        int pam_count = 0;
        int pn_count = 0;

        int BlockNum = -1;

        for(int i = 0; i < MatrixManager.arr_height; i++)
        {
            for(int j = 0; j < MatrixManager.arr_width; j++)
            {
                if(GameManager.Nowposobj == MatrixManager.Maps[i][j])
                {
                    BlockNum = MatrixManager.MapMatrix[i][j];
                    break;
                }
            }
        }

        for(int i = 0; i < MatrixManager.arr_height; i++)           // 클릭 할 수 있는 타일 거르기
        {
            for(int j = 0; j < MatrixManager.arr_width; j++)
            {
                if(MatrixManager.MapMatrix[i][j] == BlockNum)
                {
                    if(i != MatrixManager.arr_height - 1)
                    {
                        if(MatrixManager.MapMatrix[i + 1][j] / 100 < 1)
                        {
                            PossibleMatrix[pm_count++] = MatrixManager.Maps[i + 1][j];
                            PossibleNum[pn_count++] = MatrixManager.MapMatrix[i + 1][j];
                        }
                    }
                    if(i != 0)
                    {
                        if(MatrixManager.MapMatrix[i - 1][j] / 100 < 1)
                        {
                            PossibleMatrix[pm_count++] = MatrixManager.Maps[i - 1][j];
                            PossibleNum[pn_count++] = MatrixManager.MapMatrix[i - 1][j];
                        }
                    }
                    if(j != MatrixManager.arr_width - 1)
                    {
                        if(MatrixManager.MapMatrix[i][j + 1] / 100 < 1)
                        {
                            PossibleMatrix[pm_count++] = MatrixManager.Maps[i][j + 1];
                            PossibleNum[pn_count++] = MatrixManager.MapMatrix[i][j + 1];
                        }
                    }
                    if(j != 0)
                    {
                        if(MatrixManager.MapMatrix[i][j - 1] / 100 < 1)
                        {
                            PossibleMatrix[pm_count++] = MatrixManager.Maps[i][j - 1];
                            PossibleNum[pn_count++] = MatrixManager.MapMatrix[i][j - 1];
                        }
                    }
                }
            }
        }

        for(int i = 0; i < pn_count; i++)
        {
            for(int j = 0; j < MatrixManager.arr_height; j++)
            {
                for(int k = 0; k < MatrixManager.arr_width; k++)
                {
                    if(PossibleNum[i] == MatrixManager.MapMatrix[j][k])
                    {
                        PossibleAllMatrix[pam_count++] = MatrixManager.Maps[j][k];
                    }
                }
            }
        }



        for(int i = 0; i < pam_count; i++)
        {
            if(PossibleAllMatrix[i] == hit.collider.transform.parent.gameObject)
            {
                return true;
            }
        }

        return false;
    }
    private RaycastHit2D hit;
    public bool clickcheck = false;
    private void click()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.LogFormat("클릭했다");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
            if(hit.collider != null)
            {
                MatrixCompare(hit);
            }
        }
    }

    static int last_x = -1;
    private void MatrixCompare(RaycastHit2D hit)
    {
        for(int i = 0; i < MatrixManager.arr_height; i++)
        {
            for(int j = 0; j < MatrixManager.arr_width; j++)
            {
                // 내가 클릭한 것과 일치하며 장애물이 아닐 때
                if(MatrixManager.Maps[i][j] == hit.collider.transform.parent.gameObject && MatrixManager.MapMatrix[i][j] != -1)
                {
                    // 내 소유지 이거나 검출 함수의 true일 경우
                    if(MatrixManager.MapMatrix[i][j] / 100 == 1 || MatrixSelection(hit))
                    {
                        // 물 종족, 불 종족의 이동 방해 스킬에 영향을 받지 않는가?
                        if (Skill_Check(j))
                        {
                            // 현재 오브젝트가 내 캐릭터인가?
                            if (this.gameObject == GameManager.mychar)
                            {
                                last_x = j;
                                // 서버에 보낸다.
                                Sender.Send_Tile_Access((ulong)j, (ulong)i);
                                //Debug.LogFormat("서버에 보낸다 " + i + "," + j + this.transform.gameObject.name);
                                y = i;
                                x = j;
                            }
                        }
                    }
                }
            }
        }
    }

    bool Skill_Check(int _click_x)
    {
        if (Water_Skill_Used && Manager_Network.Instance.m_Client_CharacterIndex != 3)
        {
                return false;
        }
        if (Fire_Skill_Used && Manager_Network.Instance.m_Client_CharacterIndex != 2)
        {
            if (last_x <= 3 && _click_x >= 3)
                return false;
            if (last_x >= 4 && _click_x <= 4)
                return false;
        }

        return true;
    }
}
