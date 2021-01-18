#       #### Age_of_Math
![캡처](https://user-images.githubusercontent.com/58795584/100971537-7cbd6000-357a-11eb-8dcd-f5f8b9bdca41.PNG)
+ 멀티 플레이가 가능한 PVP 퍼즐 게임입니다.
+ 크로스 플랫폼 개발
+ **빌드 파일과 서버로 게임 플레이가 가능합니다!!**
+ **장르** : 퍼즐 게임
+ **개발 기간** : 2주일
+ **담당** : 클라이언트
-------
# 1. 게임 소개
## ⓐ 게임 규칙
+ **1분**의 시간이 주어지며, 맵은 **총 13개**의 테트로미노 조각들로 구성되어 있고 조각당 산수 문제 하나씩 존재한다.
+ 문제를 먼저 푼 플레이어가 해당 땅의 주인이 되며 땅의 색깔이 바뀝니다.
+ 이미 소유권이 넘어간 문제를 다시 풀 수 있으며 땅의 소유권은 문제를 푼 플레이어의 소유로 넘어가게 됩니다.
+ 소유권이 넘어갈 때마다 해당 땅의 문제의 난이도가 올라가며 최대 난이도는 3단계까지 존재합니다.
+ 캐릭터의 이동은 캐릭터의 소유 땅과 근접해 있는 땅으로만 이동할 수 있습니다.
+ 게임 시작 초반에 주어지는 첫 소유지는 상대방이 접근 불가능하고 뺏을 수도 없습니다.

### * 맵 구조
![캡처2](https://user-images.githubusercontent.com/58795584/100980553-bd23da80-3588-11eb-8f40-41b220425566.PNG)

## ⓑ 캐릭터 소개
### * 캐릭터 및 스킬 소개
+ 캐릭터는 총 3명으로, 중복해서 선택이 불가합니다. 만약 중복 선택 시 서로 매칭이 되지 않습니다.
+ 캐릭터들은 각각의 고유 스킬을 가지고 있습니다.
+ 콤보 시스템이 존재합니다. 문제를 연속으로 맞출 경우 스택이 쌓이는데 3,6,9문제를 맞출 때 스킬이 발동합니다.
+ 3, 6문제 연속 정답 시에는 점수의 배율이 증가하며 9문제를 연속으로 맞추면 캐릭터의 고유 스킬이 발동합니다.

#### 인게임 캐릭터입니다.
불 종족 | 물 종족 | 풀 종족
--------|--------|--------
<img src="https://user-images.githubusercontent.com/58795584/100981777-98306700-358a-11eb-9d30-c71e7e34b6d0.png"  width="50"> | <img src="https://user-images.githubusercontent.com/58795584/100981853-b39b7200-358a-11eb-80db-a3f33eb451ed.png"  width="50"> | <img src="https://user-images.githubusercontent.com/58795584/100981897-c01fca80-358a-11eb-98c4-b1023ba2a4c7.png"  width="50">

#### 각 종족들의 소유지입니다.
일반 땅 | 불 소유지 | 물 소유지 | 풀 소유지
-------|-------|-------|-------
<img src="https://user-images.githubusercontent.com/58795584/100983788-27d71500-358d-11eb-8c6e-387ede8b2559.png"  width="50"> | <img src="https://user-images.githubusercontent.com/58795584/100983821-3291aa00-358d-11eb-8b1f-f9e2351b3f26.png"  width="50"> | <img src="https://user-images.githubusercontent.com/58795584/100983863-3e7d6c00-358d-11eb-9f29-22851d2fe79c.png"  width="50"> | <img src="https://user-images.githubusercontent.com/58795584/100983919-51903c00-358d-11eb-8836-edd9d7c871ff.png"  width="50">

#### 각 종족들의 고유 스킬입니다.
<img src="https://user-images.githubusercontent.com/58795584/100992460-8c976d00-3597-11eb-9b70-d16606c523d0.PNG">

#### 불 종족 스킬 : 맵의 중간부분에 불을 질러 상대방이 넘어오지 못하도록 합니다.

<img src="https://user-images.githubusercontent.com/58795584/101012902-d63b8400-35a6-11eb-91d3-dcb29870c24f.PNG">

#### 물 종족 스킬: 5초간 상대가 이동 할 수 없도록 합니다.

<img src="https://user-images.githubusercontent.com/58795584/101013723-118a8280-35a8-11eb-8cd3-d8cccc015144.gif">

#### 풀 종족 : 다음 이동하는 땅을 3번까지 문제 풀이없이 소유할 수 있습니다.
-------
# 2. 구현 설명
## ⓐ. 맵 구현
+ 2차원 배열을 만들어 각 테트루미노 조각에 맞게 값을 설정합니다. ex) 문제1 지역 = 1, 문제2 지역 = 2....
```C#
        // -1 : 장애물 200 : 상대 진영  100 : 내 진영
        MapMatrix[0][0] = 1;
        MapMatrix[0][1] = 1;
        MapMatrix[0][2] = 2;
        MapMatrix[0][3] = -1;
        MapMatrix[0][4] = -1;
        MapMatrix[0][5] = 4;
        MapMatrix[0][6] = 200;       // 상대 진영
        MapMatrix[0][7] = 200;       // 상대 진영
                .
                .
                .
```
+ 소유지가 바뀌는 것은 여러번 자꾸 바뀌기 때문에 매터리얼 변경보다는 SetActive를 이용하여 변경하였습니다.
```
// case (종족)에 따라 SetActive를 다르게 한다.
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
```
## ⓑ. 이동 구현
+ 캐릭터 이동(점프)은 본인이 위치한 테트로미노 땅에 인접한 땅만 접근이 가능하도록 구현했습니다.
+ 캐릭터의 이동(점프)은 원의 호를 그리며 점프하도록 표현하기 위해 Slerp 함수로 이동경로가 원호를 그리도록 구현했습니다.
+ 이동의 자세한 사진은 윗 페이지 풀 종족 스킬 사진 참고해주세요.
```
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
```
## ⓒ. 이동 검출 후 서버에 보내는 작업
+ 이동할 수 있는 땅을 검출하여 이동이 가능하다면 서버에 위치값을 보내는 작업입니다.
```
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
```
## ⓓ. 서버에서 받아와 실제 이동하는 작업
+ 서버에서는 클라이언트에게 위치를 받아 다시 클라이언트에게 본인의 이동할 위치와 상대방의 위치를 보냅니다.
+ 클라이언트는 ulong형 숫자를 받아와 본인의 캐릭터와 상대의 캐릭터를 동시에 움직이도록 합니다.
+ 한 클라이언트에 2명의 위치값을 보내 총 4개의 캐릭터를 움직이도록 합니다.
```
  private void Start()
    {
        Manager_Network.Instance.e_Player_Move.AddListener(new UnityAction<ushort, ulong, ulong>(Jump));
    }
```
#### Start 함수에 서버에게 AddListner를 이용하여 함수를 보낸 뒤 서버에서 호출 할 수 있도록 합니다.
```
   GameManager.Nowposobj = MatrixManager.Maps[y][x];
                GameManager.Nowpos = new Vector3(GameManager.Nowposobj.transform.localPosition.x,
                    GameManager.Nowposobj.transform.localPosition.y, GameManager.Nowposobj.transform.localPosition.z);
```
#### 게임를 관리하는 매니저가 현 플레이어의 위치를 알게끔 합니다.
```
  GameManager.Nowotherobj = MatrixManager.Maps[y][x];
                GameManager.Nowotherpos = new Vector3(GameManager.Nowotherobj.transform.localPosition.x,
                    GameManager.Nowotherobj.transform.localPosition.y, GameManager.Nowotherobj.transform.localPosition.z);
```
#### 상대 캐릭터 역시 포지션값을 받아와 입력합니다.
-------
# 3. 구현하면서...
+ 유니티를 배우면서 처음으로 팀 프로젝트로 작업한 게임입니다.
+ 서버와 클라이언트의 동기화가 힘들었습니다..
+ 이동 검출의 자세한 코드를 보고싶다면 PlayerFunction.cs 파일의 Move_tile()의 함수를 참고해주세요.
