using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Network.Data;

/// <summary>
/// 네트워크 관리자
/// </summary>
public class Manager_Network : MonoBehaviour
{
    public static Manager_Network Instance;
    
    public TcpClient m_Client = null; // TCP소켓
    public Protocol_Recv_Event e_ProtocolRecv = new Protocol_Recv_Event(); // 프로토콜 겟또다제 이벤트

    bool m_Connected = false; // 서버 연결 상태
    public UInt16 m_Client_Position; // 자신이 1번인가 2번인가
    public UInt64 m_Client_CharacterIndex; // 자신 캐릭터 인덱스(1~3)
    public UInt64 m_OtherPlayer_CharacterIndex; // 상대 플레이어 캐릭터 인덱스(1~3)

    #region 이벤트들

    /// <summary> 게임 시작 </summary>
    public Event_GameStart e_GameStart = new Event_GameStart();
    /// <summary> 게임 끝 </summary>
    public Event_GameEnd e_GameEnd = new Event_GameEnd();
    /// <summary> 게임 타이머 업데이트 </summary>
    public Event_Timer e_Timer = new Event_Timer();
    /// <summary> 문제 상자 팝업. 안에 문제 들어있음 </summary>
    public Event_QuestionBox e_QueationBox = new Event_QuestionBox();
    /// <summary> 플레이어의 마킹. 플레이어 인덱스, 타일 X, 타일 Y </summary>
    public Event_Player_Mark e_Player_Mark = new Event_Player_Mark();
    /// <summary> 플레이어의 마킹 지우기. 플레이어 인덱스 </summary>
    public Event_Player_Mark_Erase e_Player_Mark_Erase = new Event_Player_Mark_Erase();
    /// <summary> 플레이어의 이동. 플레이어 인덱스, 타일 X, 타일 Y </summary>
    public Event_Player_Move e_Player_Move = new Event_Player_Move();
    /// <summary> 플레이어의 타일 습득. 타일 키, 플레이어 인덱스 </summary>
    public Event_Player_Capture e_Player_Capture = new Event_Player_Capture();
    /// <summary> 플레이어의 스코어 업데이트. 포지션, 현재 콤보 수, 현재 토탈 점수, 현재 타일 배수(소수) </summary>
    public Event_Player_UpdateScore e_Player_UpdateScore = new Event_Player_UpdateScore();
    /// <summary> 플레이어의 스킬 사용. 플레이어 인덱스, 몇 차 스킬(1~3) </summary>
    public Event_Player_UseSkill e_Player_UseSkill = new Event_Player_UseSkill();
    /// <summary> 플레이어의 항복 </summary>
    public Event_Player_Surrender e_Player_Surrender = new Event_Player_Surrender();

    #endregion


    Manager_Packet m_Packet;

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        m_Packet = new Manager_Packet(this);
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) // 디버그 - 연결
        {
            Debug.Log("Connect to Server...");
            Connect_To_Server();
        }
        else if (Input.GetKeyDown(KeyCode.D)) // 디버그 - 연결해제
        {
            Debug.Log("Disconnect to Server...");
            DIsconnect();
        }
        else if (Input.GetKeyDown(KeyCode.T)) // 디버그 - 연결 후 패킷 전송 테스트
        {
            Debug.Log("Sended Msg...");
            Task task = new Task();
            UInt64 protocol = 0;
            m_Packet.PackProtocol(ref protocol, (UInt64)PROTOCOL.DEBUG);
            // task.buffer = Packer.PackPacket(protocol, "asdf", ref task.datasize);

            m_Packet.SendEnqueue(task);
        }

        if (m_Connected) // 연결이 된 경우
            m_Packet?.Update();
    }

    void Initialize_Events()
    {
        e_GameStart = new Event_GameStart();
        e_GameEnd = new Event_GameEnd();
        e_Timer = new Event_Timer();
        e_QueationBox = new Event_QuestionBox();
        e_Player_Mark = new Event_Player_Mark();
        e_Player_Mark_Erase = new Event_Player_Mark_Erase();
        e_Player_Move = new Event_Player_Move();
        e_Player_Capture = new Event_Player_Capture();
        e_Player_UpdateScore = new Event_Player_UpdateScore();
        e_Player_UseSkill = new Event_Player_UseSkill();
        e_Player_Surrender = new Event_Player_Surrender();
    }

    #region CONNECT
    
    public void Connect_To_Server(string _ip = "127.0.0.1", string _port = "8000")
    {
        if (m_Client != null)
        {
            m_Client.GetStream().Close();
            m_Client.Close();
        }
        m_Client = new TcpClient();
        Window_Popup wp = Window_Popup.CreateWindow("서버 연결", "서버에 연결중...", WINDOW_BUTTON_TYPE.NONE, null, null);
        
        try
        {
            // 연결 시도, 실패시 SocketException
            m_Client.Connect(_ip, int.Parse(_port));
            m_Connected = true;
            Initialize_Events();

            // 패킷 핸들러 오브젝트 초기화
            m_Packet.Init();

            // 연결 됐으면 바로 자신의 선택값 보내주기
            Task task = new Task();
            UInt64 protocol = 0;
            m_Packet.PackProtocol(ref protocol, (UInt64)PROTOCOL.CONNECTED);
            task.buffer = Packer.PackPacket(ref task.datasize, protocol, m_Client_CharacterIndex);
            m_Packet.SendEnqueue(task);

        }
        catch (SocketException e)
        {
            wp.Set_Text("연결 실패", e.Message);
            wp.Set_Closable(true);
        }
    }

    public void DIsconnect()
    {
        if (m_Client != null)
        {
            m_Packet.End_Thread();
            Debug.Log("close...");
            m_Client.Close();
            Debug.Log("dispose...");
            m_Client.Dispose();
        }
        m_Client = null;
        m_Connected = false;
    }

    #endregion
}

/// <summary>
/// 패킷 관리자, 패킷을 보내거나 받거나 한다
/// 패킷은 Packer에서 먼저 패킹 작업이 이루어져야한다
/// 패킷을 받을 때에도 Packer에서 가공해서 인게임에서 써야 한다
/// </summary>
public class Manager_Packet
{
    public static Manager_Packet Instance;

    public Queue<Task> m_SendQueue; // 서버에게 보내는 패킷 모음, 선입 선출
    public Queue<Task> m_RecvQueue; // 서버에게서 받는 패킷 모음, 선입 선출

    Manager_Network m_NetworkManager; // 모체
    Task_Handler m_Task_Handler;
    Thread t_Receiver;

    public Manager_Packet(Manager_Network _network)
    {
        Instance = this;
        m_NetworkManager = _network;
        m_Task_Handler = new Task_Handler();
        m_SendQueue = new Queue<Task>();
        m_RecvQueue = new Queue<Task>();
    }

    public void Init()
    {
        if (t_Receiver != null)
            t_Receiver.Abort();

        m_SendQueue = new Queue<Task>();
        m_RecvQueue = new Queue<Task>();
        t_Receiver = new Thread(Thread_Recv);
        t_Receiver.Start();
    }

    public void End_Thread()
    {
        t_Receiver.Abort();
        t_Receiver = null;
    }

    public void Update()
    {
        SendAll();
        RecvAll();
    }

    // Recv 전용 스레드
    void Thread_Recv()
    {
        m_RecvQueue.Clear();
        while (true)
        {
            Task task = new Task(); // 새로운 버퍼를 만들고
            PacketRecv(ref task.buffer); // 받을 준비 하고
            m_RecvQueue.Enqueue(task); // 받으면 받기 큐에 그것을 투입
        }
    }
    
    public byte[] SetBuffer(UInt64 _protocol, byte[] _buffer, ref int _size)
    {
        byte[] data = new byte[1024];
        int place = 0;

        Buffer.BlockCopy(BitConverter.GetBytes(_protocol), 0, data, place, sizeof(UInt64));
        place += sizeof(UInt64);

        Buffer.BlockCopy(_buffer, 0, data, place, _buffer.Length);
        place += _buffer.Length;

        _size = place;

        return data;
    }

    #region SEND

    // 프로토콜 포장
    public void PackProtocol(ref UInt64 _protocol, UInt64 __protocol)
    {
        _protocol = _protocol | __protocol;
    }

    public void PacketSend(Task _task)
    {
        NetworkStream ns;

        ns = m_NetworkManager.m_Client.GetStream();

        ns.Write(_task.buffer, 0, _task.datasize);
    }

    // 보내기 큐에 패킷 투입
    public void SendEnqueue(Task _task)
    {
        m_SendQueue.Enqueue(_task);
    }

    // 보내기 큐를 비울 때까지 계속해서 보내기
    public void SendAll()
    {
        while (m_SendQueue.Count > 0)
        {
            Task task = new Task();

            task = m_SendQueue.Dequeue();

            PacketSend(task);
            Debug.Log("Sended.");
        }
    }

    #endregion

    #region RECV

    // 서버에게서 패킷 받기
    public void PacketRecv(ref byte[] _buf)
    {
        byte[] size = new byte[4];

        NetworkStream ns = m_NetworkManager.m_Client.GetStream();

        int recv = ns.Read(size, 0, 4);
        int packsize = BitConverter.ToInt16(size, 0);

        recv = ns.Read(_buf, 0, packsize);
    }

    // 받기 큐를 비울 때까지 계속해서 받기
    void RecvAll()
    {
        while (m_RecvQueue.Count > 0)
        {
            Task task = new Task();

            task = m_RecvQueue.Dequeue();

            Debug.Log("Received...");
            m_Task_Handler.Perform_Task(m_NetworkManager, task); // 받은 패킷과 프로토콜을 전달, 인게임 요소들에 반영
        }
    }

    #endregion





}
