#       #### Age_of_Math
![캡처](https://user-images.githubusercontent.com/58795584/100971537-7cbd6000-357a-11eb-8dcd-f5f8b9bdca41.PNG)
+ 멀티 플레이가 가능한 PVP 퍼즐 게임입니다.
+ 크로스 플랫폼 개발
+ **빌드 파일과 서버로 게임 플레이가 가능합니다!!**
+ **장르** : 퍼즐 게임
+ **개발 기간** : 2주일
+ **담당** : 클라이언트

# 목차
* [게임 소개](#게임-소개)
* [구현 내](#구현-내용)
* [어려웠던 점](#어려웠던-점)

-------
# 게임 소개
## 게임 규칙
+ **1분**의 시간이 주어지며, 맵은 **총 13개**의 테트로미노 조각들로 구성되어 있고 조각당 산수 문제 하나씩 존재한다.
+ 문제를 먼저 푼 플레이어가 해당 땅의 주인이 되며 땅의 색깔이 바뀝니다.
+ 이미 소유권이 넘어간 문제를 다시 풀 수 있으며 땅의 소유권은 문제를 푼 플레이어의 소유로 넘어가게 됩니다.
+ 소유권이 넘어갈 때마다 해당 땅의 문제의 난이도가 올라가며 최대 난이도는 3단계까지 존재합니다.
+ 캐릭터의 이동은 캐릭터의 소유 땅과 근접해 있는 땅으로만 이동할 수 있습니다.
+ 게임 시작 초반에 주어지는 첫 소유지는 상대방이 접근 불가능하고 뺏을 수도 없습니다.

### 맵 구조
![캡처2](https://user-images.githubusercontent.com/58795584/100980553-bd23da80-3588-11eb-8f40-41b220425566.PNG)

## 캐릭터 소개
### 캐릭터 및 스킬 소개
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
#### 불 종족 스킬 : 맵의 중간부분에 불을 질러 상대방이 넘어오지 못하도록 합니다.
<img src="https://user-images.githubusercontent.com/58795584/100992460-8c976d00-3597-11eb-9b70-d16606c523d0.PNG">

#### 물 종족 스킬: 5초간 상대가 이동 할 수 없도록 합니다.
<img src="https://user-images.githubusercontent.com/58795584/101012902-d63b8400-35a6-11eb-91d3-dcb29870c24f.PNG">

#### 풀 종족 스킬: 다음 이동하는 땅을 3번까지 문제 풀이없이 소유할 수 있습니다.
<img src="https://user-images.githubusercontent.com/58795584/101013723-118a8280-35a8-11eb-8cd3-d8cccc015144.gif">

-------
# 구현 내용
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

-------------

## 서버 클라이언트 연동

-------------

# 어려웠던 점

-------------
