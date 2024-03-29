#       #### Age_of_Math
![캡처](https://user-images.githubusercontent.com/58795584/100971537-7cbd6000-357a-11eb-8dcd-f5f8b9bdca41.PNG)
+ 멀티 플레이가 가능한 1:1 땅따먹기 퍼즐 게임입니다.
+ **개발 기간** : 2주일
+ **담당** : 클라이언트

## [플레이 영상](https://youtu.be/NgJH7dRPv_Q)

# 목차
* [게임 소개](#게임-소개)
* [구현 내용](#구현-내용)
* [어려웠던 점](#어려웠던-점)

-------
# 게임 소개
* [게임 규칙](#게임-규칙)
* [맵 구조](#맵-구조)
* [캐릭터 소개](#캐릭터-소개)

## 게임 규칙
+ **1분**의 시간이 주어지며, 맵은 **총 13개**의 테트로미노 조각들로 구성되어 있고 조각당 산수 문제 하나씩 존재한다.
+ 문제를 먼저 푼 플레이어가 해당 땅의 주인이 되며 땅의 색깔이 바뀝니다.
+ 이미 소유권이 넘어간 문제를 다시 풀 수 있으며 땅의 소유권은 문제를 푼 플레이어의 소유로 넘어가게 됩니다.
+ 소유권이 넘어갈 때마다 해당 땅의 문제의 난이도가 올라가며 최대 난이도는 3단계까지 존재합니다.
+ 캐릭터의 이동은 캐릭터의 소유 땅과 근접해 있는 땅으로만 이동할 수 있습니다.
+ 게임 시작 초반에 주어지는 첫 소유지는 상대방이 접근 불가능하고 뺏을 수도 없습니다.

## 맵 구조
![캡처2](https://user-images.githubusercontent.com/58795584/100980553-bd23da80-3588-11eb-8f40-41b220425566.PNG)

## 캐릭터 소개
### 캐릭터 소개
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

### 캐릭터 고유 스킬 소개
#### 불 종족 스킬 : 맵의 중간부분에 불을 질러 상대방이 넘어오지 못하도록 합니다.
<img src="https://user-images.githubusercontent.com/58795584/100992460-8c976d00-3597-11eb-9b70-d16606c523d0.PNG">

#### 물 종족 스킬: 5초간 상대가 이동 할 수 없도록 합니다.
<img src="https://user-images.githubusercontent.com/58795584/101012902-d63b8400-35a6-11eb-91d3-dcb29870c24f.PNG">

#### 풀 종족 스킬: 다음 이동하는 땅을 3번까지 문제 풀이없이 소유할 수 있습니다.
<img src="https://user-images.githubusercontent.com/58795584/101013723-118a8280-35a8-11eb-8cd3-d8cccc015144.gif">

-------
# 구현 내용
* [맵 구현](#맵-구현)
* [이동 구현](#이동-구현)
* [이동 조건 검출 로직](#이동-조건-검출-로직)
* [서버 클라이언트 연동](#서버-클라이언트-연동)

## 맵 구현
![image](https://user-images.githubusercontent.com/77636255/116548043-a4003980-a92e-11eb-9a63-096a65cc6d8c.png)
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
-------------

## 이동 구현
![ezgif com-gif-maker](https://user-images.githubusercontent.com/77636255/116549471-7caa6c00-a930-11eb-9b46-4fcd7d2270d7.gif)
+ Slerp 함수를 이용해 포물선을 그리며 이동하도록 구현
```
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
```
-------------

## 이동 조건 검출 로직
![image](https://user-images.githubusercontent.com/77636255/116559125-10813580-a93b-11eb-86a9-f769624268ff.png)

1. 현재 캐릭터가 밟고 있는 영역의 숫자를 가져온다 (ex) BlockNum = (1 ~ 100 or 200)
```c#
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
```
2. 탐색을 통해 밟을 수 있는 영역의 번호를 HashSet에 삽입해 보관한다. (O 표시한 블록의 번호를 가져온다.)
```
// 현재 캐릭터의 영역 주위 상하좌우에 있는 영역의 넘버를 배열로 받아온다.
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
```
3. HashSet을 이용해 클릭한 영역과 비교
```
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
```
-------------

## 서버 클라이언트 연동
### 서버는 양 클라이언트에게 같은 패킷을 보낸다. (한 클라이언트가 이동 할 경우 매번 패킷을 보냄)
### 1번 클라이언트가 이동할 경우 => 1번 클라이언트 : 내 캐릭터 이동, 2번 클라이언트 : 상대 캐릭터가 이동
![image](https://user-images.githubusercontent.com/77636255/116575918-0e72a300-a94a-11eb-929a-eb5de6ba8160.png)

1. 게임 시작 시 서버에서 자신의 번호를 받아 상대 번호를 세팅
```
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
```
2. AddListener를 이용해 함수를 등록하고 패킷을 받으면 등록된 함수를 실행(Jump 함수)
```
Manager_Network.Instance.e_Player_Move.AddListener(new UnityAction<ushort, ulong, ulong>(Jump));
```
3. Position과 패킷의 매개변수 player를 비교해 이동을 결정 (내 캐릭터 라면)
```
public void Jump(ushort _player, ulong _x, ulong _y)
{
        if (_player == GameManager.position)
        {
            Manager_Sound.Instance.Play_SE(SE_INDEX.JUMP);
            StartCoroutine(MoveCoroutine(_x, _y, _player));
        }
}
```
4. 코루틴을 이용해 이동을 실행
#### 이동 중 패킷이 오더라도 이동이 중단되지 않고 새로운 코루틴을 실행하기 때문에 멀티 스레드의 효과를 볼 수 있다.
```
IEnumerator MoveCoroutine(ulong x, ulong y, ushort player)
{
        // 내 캐릭터 이동 함수
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        // 이동 함수 이동 완료 or 번호가 틀리면 코루틴 종료
        while(!Move_tile(x, y, player))
        {
            yield return waitForEndOfFrame;
        }
}
```
-------------

# 어려웠던 점
* 이동 도중 도착 하기 전 새로운 패킷을 받으면 이동이 끊기는 문제를 코루틴을 통해 해결
-------------
