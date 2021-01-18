using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MapMaker : MonoBehaviour
{
    public static MapMaker Instance;

    public GameObject Tile_Prefab;
    public GameObject[][] Maps;
    public int[][] MapMatrix;
    
    public int arr_width;
    public int arr_height;

    GameStartManager GameManager;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;

        Maps = new GameObject[arr_height][];
        for(int i = 0; i < arr_height; i++)
            Maps[i] = new GameObject[arr_width];

        MapMatrix = new int[arr_height][];
        for(int i = 0; i < arr_height; i++)
            MapMatrix[i] = new int[arr_width];

        CreateMatrix();
        MatrixSetting();
        Map_Border_Setting();
    }

    private void Start()
    {
        GameManager = GameObject.Find("StartManager").GetComponent<GameStartManager>();
        Manager_Network.Instance.e_Player_Capture.AddListener(new UnityAction<ushort, ushort>(Tile_Renewal));

        Manager_Network.Instance.e_Player_Capture.Invoke(100, 1);
        Manager_Network.Instance.e_Player_Capture.Invoke(200, 2);
    }


    void Tile_Renewal(ushort tile_index, ushort _player)
    {
        //Debug.LogFormat("타일 바꾸기 함수 출현 플레이어 " + _player + " 타일 번호 " + tile_index);
        StartCoroutine(Tile_Anim(tile_index, _player));
    }
    
    IEnumerator Tile_Anim(ushort tile_index, ushort _player)
    {
        yield return new WaitForSeconds(0.016f * 39f);
        for (int i = 0; i < arr_height; i++)
        {
            for (int j = 0; j < arr_width; j++)
            {
                if (MapMatrix[i][j] == tile_index && GameManager.position == _player)
                {
                    switch (GameManager.my)
                    {
                        case 1:
                            Maps[i][j].transform.GetChild(0).gameObject.SetActive(true);
                            Maps[i][j].transform.GetChild(1).gameObject.SetActive(false);
                            Maps[i][j].transform.GetChild(2).gameObject.SetActive(false);
                            Maps[i][j].transform.GetChild(3).gameObject.SetActive(false);
                            break;
                        case 2:
                            Maps[i][j].transform.GetChild(0).gameObject.SetActive(false);
                            Maps[i][j].transform.GetChild(1).gameObject.SetActive(true);
                            Maps[i][j].transform.GetChild(2).gameObject.SetActive(false);
                            Maps[i][j].transform.GetChild(3).gameObject.SetActive(false);
                            break;
                        case 3:
                            Maps[i][j].transform.GetChild(0).gameObject.SetActive(false);
                            Maps[i][j].transform.GetChild(1).gameObject.SetActive(false);
                            Maps[i][j].transform.GetChild(2).gameObject.SetActive(true);
                            Maps[i][j].transform.GetChild(3).gameObject.SetActive(false);
                            break;
                    }
                    Instantiate(GameManager.myEffect, Maps[i][j].transform.localPosition, Maps[i][j].transform.localRotation);
                }
                else if (GameManager.other_position == _player && MapMatrix[i][j] == tile_index)
                {
                    switch (GameManager.other)
                    {
                        case 1:
                            Maps[i][j].transform.GetChild(0).gameObject.SetActive(true);
                            Maps[i][j].transform.GetChild(1).gameObject.SetActive(false);
                            Maps[i][j].transform.GetChild(2).gameObject.SetActive(false);
                            Maps[i][j].transform.GetChild(3).gameObject.SetActive(false);
                            break;
                        case 2:
                            Maps[i][j].transform.GetChild(0).gameObject.SetActive(false);
                            Maps[i][j].transform.GetChild(1).gameObject.SetActive(true);
                            Maps[i][j].transform.GetChild(2).gameObject.SetActive(false);
                            Maps[i][j].transform.GetChild(3).gameObject.SetActive(false);
                            break;
                        case 3:
                            Maps[i][j].transform.GetChild(0).gameObject.SetActive(false);
                            Maps[i][j].transform.GetChild(1).gameObject.SetActive(false);
                            Maps[i][j].transform.GetChild(2).gameObject.SetActive(true);
                            Maps[i][j].transform.GetChild(3).gameObject.SetActive(false);
                            break;
                    }
                    Instantiate(GameManager.otherEffect, Maps[i][j].transform.localPosition, Maps[i][j].transform.localRotation);
                }

            }
        }
        yield return null;
    }

    private void MatrixSetting()
    {
        // -1 : 장애물 200 : 상대 진영  100 : 내 진영

        MapMatrix[0][0] = 1;
        MapMatrix[0][1] = 1;
        MapMatrix[0][2] = 2;
        MapMatrix[0][3] = -1;
        MapMatrix[0][4] = -1;
        MapMatrix[0][5] = 4;
        MapMatrix[0][6] = 200;       // 상대 진영
        MapMatrix[0][7] = 200;       // 상대 진영

        MapMatrix[1][0] = 1;
        MapMatrix[1][1] = 2;
        MapMatrix[1][2] = 2;
        MapMatrix[1][3] = 2;
        MapMatrix[1][4] = 3;
        MapMatrix[1][5] = 4;
        MapMatrix[1][6] = 200;       // 상대 진영
        MapMatrix[1][7] = 200;       // 상대 진영

        MapMatrix[2][0] = 1;
        MapMatrix[2][1] = 7;
        MapMatrix[2][2] = 3;
        MapMatrix[2][3] = 3;
        MapMatrix[2][4] = 3;
        MapMatrix[2][5] = 4;
        MapMatrix[2][6] = 5;
        MapMatrix[2][7] = 5;

        MapMatrix[3][0] = 6;
        MapMatrix[3][1] = 7;
        MapMatrix[3][2] = 7;
        MapMatrix[3][3] = 8;
        MapMatrix[3][4] = 8;
        MapMatrix[3][5] = 4;
        MapMatrix[3][6] = 9;
        MapMatrix[3][7] = 5;

        MapMatrix[4][0] = 6;
        MapMatrix[4][1] = 7;
        MapMatrix[4][2] = 10;
        MapMatrix[4][3] = 8;
        MapMatrix[4][4] = 8;
        MapMatrix[4][5] = 9;
        MapMatrix[4][6] = 9;
        MapMatrix[4][7] = 5;

        MapMatrix[5][0] = 6;
        MapMatrix[5][1] = 6;
        MapMatrix[5][2] = 10;
        MapMatrix[5][3] = 11;
        MapMatrix[5][4] = 11;
        MapMatrix[5][5] = 11;
        MapMatrix[5][6] = 9;
        MapMatrix[5][7] = 13;

        MapMatrix[6][0] = 100;
        MapMatrix[6][1] = 100;
        MapMatrix[6][2] = 10;
        MapMatrix[6][3] = 11;
        MapMatrix[6][4] = 12;
        MapMatrix[6][5] = 12;
        MapMatrix[6][6] = 12;
        MapMatrix[6][7] = 13;


        MapMatrix[7][0] = 100;
        MapMatrix[7][1] = 100;
        MapMatrix[7][2] = 10;
        MapMatrix[7][3] = -1;
        MapMatrix[7][4] = -1;
        MapMatrix[7][5] = 12;
        MapMatrix[7][6] = 13;
        MapMatrix[7][7] = 13;
        
    }

    private void CreateMatrix()
    {
        for(int y = 0; y < arr_height; y++)
        {
            for(int x = 0; x < arr_width; x++)
            {
                GameObject tile = Instantiate(Tile_Prefab, transform);
                tile.transform.localPosition = new Vector3(x * 0.9f, y * -0.9f);
                Maps[y][x] = tile;
                MapMatrix[y][x] = 0;
            }
        }
        Maps[0][3].SetActive(false);
        Maps[0][4].SetActive(false);
        Maps[7][3].SetActive(false);
        Maps[7][4].SetActive(false);

    }

    void Map_Border_Setting()
    {
        for (int y = 0; y < arr_height; y++)
        {
            for (int x = 0; x < arr_width; x++)
            {
                if (Maps[y][x].activeSelf)
                {
                    Border_Data border = Maps[y][x].GetComponent<Border_Data>();
                    int left = x > 0 ? MapMatrix[y][x - 1] : -999;
                    int top = y > 0 ? MapMatrix[y - 1][x] : -999;
                    int right = x < arr_width - 1 ? MapMatrix[y][x + 1] : -999;
                    int bottom = y < arr_height - 1 ? MapMatrix[y + 1][x] : -999;
                    border.Set_Border(MapMatrix[y][x], left, top, right, bottom);
                }
            }
        }
    }
}
