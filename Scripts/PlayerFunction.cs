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

    //IEnumerator enumerator;
    private void Start()
    {
        //enumerator = MoveCoroutine();
    }

    public void Event_Insert()
    {
        Manager_Network.Instance.e_Player_Move.AddListener(new UnityAction<ushort, ulong, ulong>(Jump));
    }

    //private bool moving = false;
    //private int x;
    //private int y;
    //private int player;
    private float t;

    public void Jump(ushort _player, ulong _x, ulong _y)
    {
        if (_player == GameManager.position)
        {
            Manager_Sound.Instance.Play_SE(SE_INDEX.JUMP);
            StartCoroutine(MoveCoroutine(_x, _y, _player));
        }
    }

    IEnumerator MoveCoroutine(ulong x, ulong y, ushort player)
    {
        // 내 캐릭터 이동 함수
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        moving = true;
        // 이동 함수 이동 완료 or 번호가 틀리면 코루틴 종료
        while(!Move_tile(x, y, player))
        {
            yield return waitForEndOfFrame;
        }

        moving = false;
    }

    bool Move_tile(ulong x, ulong y, ushort player)
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
                return true;
            }

            // Sleap 구현을 위해 출발점과 도착점의 센터점을 지정
            // 윗줄에서 center 선언
            // Vector3 center = (GameManager.Nowposobj.transform.localPosition + MatrixManager.Maps[y][x].transform.localPosition) * 0.5f;

            Vector3 RelCenter = GameManager.Nowposobj.transform.localPosition - center;
            Vector3 aimRelCenter = MatrixManager.Maps[y][x].transform.localPosition - center;

            // 처음 한번만 divide와 startTime을 지정해 Time.time이 늘어나면서 fracComplete 변수가 1이 될 때까지 실행(도착 까지)
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

            if (GameManager.mychar.transform.localPosition == MatrixManager.Maps[y][x].transform.localPosition)
            {
                Manager_Sound.Instance.Play_SE(SE_INDEX.TILE_CHANGE);
                GameManager.mychar.transform.GetChild(0).gameObject.SetActive(true);
                GameManager.mychar.transform.GetChild(1).gameObject.SetActive(false);
                GameManager.mychar.transform.GetChild(2).gameObject.SetActive(false);
                GameManager.mychar.transform.GetChild(3).gameObject.SetActive(false);
                GameManager.Nowposobj = MatrixManager.Maps[y][x];
                GameManager.Nowpos = new Vector3(GameManager.Nowposobj.transform.localPosition.x,
                    GameManager.Nowposobj.transform.localPosition.y, GameManager.Nowposobj.transform.localPosition.z);
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

    private void Update()
    {
        if(GameManager.clickpossible)
        {
            //if (moving)
            //{
            //    Move_tile();
            //}
            if (!UIManager.Question.activeSelf/* && !clickcheck*/)
            {
                click();
            }
        }
    }
    private bool MatrixSelection(RaycastHit2D hit)
    {
        HashSet<int> PossibleNum = new HashSet<int>();

        int BlockNum = -1;

        // 현재 캐릭터가 서 있는 블록의 넘버를 가져온다.
        for(int i = 0; i < MatrixManager.arr_height; i++)
        {
            bool flag = false;
            for(int j = 0; j < MatrixManager.arr_width; j++)
            {
                if(GameManager.Nowposobj == MatrixManager.Maps[i][j])
                {
                    BlockNum = MatrixManager.MapMatrix[i][j];
                    flag = true;
                }
                if (flag) break;
            }
            if (flag) break;
        }

        // 현재 캐릭터의 영역 주위 상하좌우에 있는 영역의 넘버를 해쉬 맵에 삽입.
        for(int i = 0; i < MatrixManager.arr_height; i++)
        {
            for(int j = 0; j < MatrixManager.arr_width; j++)
            {
                if(MatrixManager.MapMatrix[i][j] == BlockNum)
                {
                    if(i != MatrixManager.arr_height - 1)
                    {
                        PossibleNum.Add(MatrixManager.MapMatrix[i + 1][j]);
                    }
                    if(i != 0)
                    {
                        PossibleNum.Add(MatrixManager.MapMatrix[i - 1][j]);
                    }
                    if(j != MatrixManager.arr_width - 1)
                    {
                        PossibleNum.Add(MatrixManager.MapMatrix[i][j + 1]);
                    }
                    if(j != 0)
                    {
                        PossibleNum.Add(MatrixManager.MapMatrix[i][j - 1]);
                    }
                }
            }
        }

        // 클릭한 영역이 내가 갈 수 있는 영역인지 체크
        for (int i = 0; i < MatrixManager.arr_height; i++)
        {
            for (int j = 0; j < MatrixManager.arr_width; j++)
            {
                if(hit.collider.transform.parent.gameObject == MatrixManager.Maps[i][j])
                {
                    foreach(int number in PossibleNum)
                    {
                        if (MatrixManager.MapMatrix[i][j] == number) return true;
                    }
                    return false;
                }
            }
        }

        //// 밟을 수 있는 땅의 넘버인 땅 오브젝트를 모두 배열에 넣는다.
        //foreach (int number in PossibleNum)
        //{
        //    for(int j = 0; j < MatrixManager.arr_height; j++)
        //    {
        //        for (int k = 0; k < MatrixManager.arr_width; k++)
        //        {
        //            if (number == MatrixManager.MapMatrix[j][k])
        //            {
        //                PossibleAllMatrix.Add(MatrixManager.Maps[j][k]);
        //            }
        //        }
        //    }
        //}

        // 내가 이동하려 하는 땅이 이동 가능한 땅인지 확인
        //foreach (GameObject Block in PossibleAllMatrix)
        //{
        //    if (Block == hit.collider.transform.parent.gameObject)
        //    {
        //        return true;
        //    }
        //}

        return false;
    }
    private RaycastHit2D hit;
    public bool clickcheck = false;
    bool moving = false;
    private void click()
    {
        if(Input.GetMouseButtonDown(0) && !moving)
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
                    if(MatrixManager.MapMatrix[i][j] / 100 >= 1 || MatrixSelection(hit))
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
                                //y = i;
                                //x = j;
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
