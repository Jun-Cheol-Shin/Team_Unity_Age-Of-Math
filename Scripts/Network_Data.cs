using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Network.Data
{
    /// <summary>
    /// 프로토콜
    /// </summary>
    public enum PROTOCOL : UInt64
    {
        // ETC
        DEBUG = 1,
        NOTICE,

        // TITLE
        CONNECTED, // 연결됨
        MATCHED, // 매치 성사됨
        READY, // 게임을 할 준비 완료됨

        // INGAME
        GAME_START, // 레디 체크 후 게임 시작
        TIMER, // 타이머 갱신
        END_GAME, // 게임 끝
        TILE_ACCESSABLE, // 클라: 여기 접근 가능? 묻기
        TILE_UNACCEPTABLE, // 서버: 응 안돼
        TILE_MARK, // 서버: 얘 여기에 마크 박음
        TILE_MARK_ERASE, // 서버 : 마크 지우셈
        TILE_MOVE, // 서버: 얘 여기로 움직임
        TILE_CAPTURE, // 서버: 얘 여기 정복함
        TILE_QUESTION, // 서버: 여기 문제야
        ANSWER, // 클라: 답을 줄게
        SCORE, // x번 플레이어의 현재 점수
        SKILL, // 스킬
        SURRENDER, // 항복

        // DISCONNECT
        ESCAPED, // 얘 접속이 안됨
        DISCONNECT // 얘 끊어짐
    }

    #region 이벤트

    public class Protocol_Recv_Event : UnityEvent<PROTOCOL> { };
    public class Event_GameStart : UnityEvent { };
    public class Event_GameEnd : UnityEvent { };
    public class Event_Timer : UnityEvent<ulong> { };
    public class Event_QuestionBox : UnityEvent<Quiz> { };
    public class Event_Player_Mark : UnityEvent<UInt16, UInt64, UInt64> { };
    public class Event_Player_Mark_Erase : UnityEvent<UInt16> { };
    public class Event_Player_Move : UnityEvent<UInt16, UInt64, UInt64> { };
    public class Event_Player_Capture : UnityEvent<UInt16, UInt16> { };
    public class Event_Player_Answered : UnityEvent<bool, bool> { };
    public class Event_Player_UpdateScore : UnityEvent<UInt16, ScoreData> { };
    public class Event_Player_UseSkill : UnityEvent<UInt16, UInt16> { };
    public class Event_Player_Surrender : UnityEvent { };

    #endregion

    /// <summary>
    /// 버퍼
    /// </summary>
    public class Task
    {
        public byte[] buffer = new byte[4096];
        public int datasize = 0;
    }

    /// <summary>
    /// 문제
    /// </summary>
    public class Quiz
    {
        public string m_Question;
        public string m_Answer_1;
        public string m_Answer_2;
        public string m_Answer_3;
        public string m_Answer_4;
    }

    /// <summary>
    /// 점수 데이터
    /// </summary>
    public struct ScoreData
    {
        public ulong quiz_score;
        public short combo;
        public short current_tile;
        public ulong total_score;
        public float multiplier;
    }

    /// <summary>
    /// 패킷 제작기
    /// </summary>
    public class Packer
    {
        public static byte[] PackPacket(ref int _size, UInt64 _protocol, string _string)
        {
            byte[] data = new byte[1024];
            int place = 0;

            int NickNamesize = _string.Length * 2;

            place += sizeof(int);

            Buffer.BlockCopy(BitConverter.GetBytes(_protocol), 0, data, place, sizeof(UInt64));
            place += sizeof(UInt64);
            _size += sizeof(UInt64);

            Buffer.BlockCopy(BitConverter.GetBytes(NickNamesize), 0, data, place, sizeof(int));
            place += sizeof(int);
            _size += sizeof(int);

            Buffer.BlockCopy(Encoding.Unicode.GetBytes(_string), 0, data, place, _string.Length * 2);
            place += NickNamesize;
            _size += NickNamesize;

            place = 0;

            Buffer.BlockCopy(BitConverter.GetBytes(_size), 0, data, place, sizeof(int));

            _size += sizeof(int);

            return data;
        }

        public static byte[] PackPacket(ref int _size, UInt64 _protocol, UInt64 _data)
        {
            byte[] data = new byte[1024];
            int place = 0;

            place += sizeof(int);

            Buffer.BlockCopy(BitConverter.GetBytes(_protocol), 0, data, place, sizeof(UInt64));
            place += sizeof(UInt64);
            _size += sizeof(UInt64);

            Buffer.BlockCopy(BitConverter.GetBytes(_data), 0, data, place, sizeof(UInt64));
            place += sizeof(UInt64);
            _size += sizeof(UInt64);

            place = 0;

            Buffer.BlockCopy(BitConverter.GetBytes(_size), 0, data, place, sizeof(int));

            _size += sizeof(int);

            return data;
        }

        public static byte[] PackPacket(ref int _size, UInt64 _protocol, ushort _data)
        {
            byte[] data = new byte[1024];
            int place = 0;

            place += sizeof(int);

            Buffer.BlockCopy(BitConverter.GetBytes(_protocol), 0, data, place, sizeof(UInt64));
            place += sizeof(UInt64);
            _size += sizeof(UInt64);

            Buffer.BlockCopy(BitConverter.GetBytes(_data), 0, data, place, sizeof(ushort));
            place += sizeof(ushort);
            _size += sizeof(ushort);

            place = 0;

            Buffer.BlockCopy(BitConverter.GetBytes(_size), 0, data, place, sizeof(int));

            _size += sizeof(int);

            return data;
        }

        public static byte[] PackPacket(ref int _size, UInt64 _protocol, UInt64 _x, UInt64 _y)
        {
            byte[] data = new byte[1024];
            int place = 0;

            place += sizeof(int);

            Buffer.BlockCopy(BitConverter.GetBytes(_protocol), 0, data, place, sizeof(UInt64));
            place += sizeof(UInt64);
            _size += sizeof(UInt64);

            Buffer.BlockCopy(BitConverter.GetBytes(_x), 0, data, place, sizeof(UInt64));
            place += sizeof(UInt64);
            _size += sizeof(UInt64);

            Buffer.BlockCopy(BitConverter.GetBytes(_y), 0, data, place, sizeof(UInt64));
            place += sizeof(UInt64);
            _size += sizeof(UInt64);

            place = 0;

            Buffer.BlockCopy(BitConverter.GetBytes(_size), 0, data, place, sizeof(int));

            _size += sizeof(int);

            return data;
        }

    }

    public class Sender
    {
        /// <summary>
        /// 나 살아있다고 알리기
        /// </summary>
        public static void Send_Debug()
        {
            Task task = new Task();
            UInt64 protocol = (UInt64)PROTOCOL.DEBUG;
            task.buffer = Packer.PackPacket(ref task.datasize, protocol, "");

            Manager_Packet.Instance.SendEnqueue(task);
        }

        /// <summary>
        /// 인게임 씬 준비 완료했다고 알리기
        /// </summary>
        public static void Send_Ready()
        {
            Task task = new Task();
            UInt64 protocol = (UInt64)PROTOCOL.READY;
            task.buffer = Packer.PackPacket(ref task.datasize, protocol, "");

            Manager_Packet.Instance.SendEnqueue(task);
        }

        /// <summary>
        /// 타일 접근 시도하기, 성공시 그곳에 마크 박고 문제 풀기 시작
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        public static void Send_Tile_Access(UInt64 _x, UInt64 _y)
        {
            Task task = new Task();
            UInt64 protocol = (UInt64)PROTOCOL.TILE_ACCESSABLE;
            task.buffer = Packer.PackPacket(ref task.datasize, protocol, _x, _y);

            Manager_Packet.Instance.SendEnqueue(task);
        }

        /// <summary>
        /// 문제에 대한 답안 보내기, 성공시 그곳으로 이동
        /// </summary>
        /// <param name="_answer_index"></param>
        public static void Send_Answer(ushort _answer_index)
        {
            Task task = new Task();
            UInt64 protocol = (UInt64)PROTOCOL.ANSWER;
            task.buffer = Packer.PackPacket(ref task.datasize, protocol, _answer_index);

            Manager_Packet.Instance.SendEnqueue(task);
        }

        /// <summary>
        /// 게임 종료 확인했다고 보내기
        /// </summary>
        public static void Send_Surrender()
        {
            Task task = new Task();
            UInt64 protocol = (UInt64)PROTOCOL.ESCAPED;
            task.buffer = Packer.PackPacket(ref task.datasize, protocol, "");

            Manager_Packet.Instance.SendEnqueue(task);
        }

        /// <summary>
        /// 게임 종료 확인했다고 보내기
        /// </summary>
        public static void Send_EndGame()
        {
            Task task = new Task();
            UInt64 protocol = (UInt64)PROTOCOL.END_GAME;
            task.buffer = Packer.PackPacket(ref task.datasize, protocol, "");

            Manager_Packet.Instance.SendEnqueue(task);
        }
    }

    /// <summary>
    /// 패킷 분해기
    /// </summary>
    public class Unpacker
    {
        public static void UnPackPacket(byte[] _data, ref string _msg)
        {
            int msgSize = 0;

            int place = 0;

            place += sizeof(UInt64);

            msgSize = BitConverter.ToInt32(_data, place);
            place += sizeof(int);

            _msg = Encoding.Unicode.GetString(_data, place, msgSize);
            place += msgSize;
        }
        public static void UnPackPacket(byte[] _data, ref UInt16 _short)
        {
            int place = 0;

            place += sizeof(UInt16);

            _short = BitConverter.ToUInt16(_data, place);
            place += sizeof(UInt16);
        }
        public static void UnPackPacket(byte[] _data, ref UInt64 _int)
        {
            int place = 0;

            place += sizeof(UInt64);

            _int = BitConverter.ToUInt64(_data, place);
            place += sizeof(UInt64);
        }
        public static void UnPackPacket(byte[] _data, ref UInt16 _short, ref UInt64 _int)
        {
            int place = 0;

            place += sizeof(UInt64);

            _short = BitConverter.ToUInt16(_data, place);
            place += sizeof(UInt16);

            _int = BitConverter.ToUInt64(_data, place);
            place += sizeof(UInt64);
        }
        public static void UnPackPacket(byte[] _data, ref UInt16 _tile_key, ref UInt16 _player_index)
        {
            int place = 0;

            place += sizeof(UInt64);

            _tile_key = BitConverter.ToUInt16(_data, place);
            place += sizeof(UInt16);

            _player_index = BitConverter.ToUInt16(_data, place);
            place += sizeof(UInt16);
        }
        public static void UnPackPacket(byte[] _data, ref Quiz _quiz)
        {
            // 문제, 답안1, 답안2, 답안3, 답안4
            int msgSize = 0;
            int place = 0;

            place += sizeof(UInt64);

            msgSize = BitConverter.ToInt32(_data, place);
            place += sizeof(int);

            _quiz.m_Question = Encoding.Unicode.GetString(_data, place, msgSize);
            place += msgSize;

            msgSize = BitConverter.ToInt32(_data, place);
            place += sizeof(int);

            _quiz.m_Answer_1 = Encoding.Unicode.GetString(_data, place, msgSize);
            place += msgSize;

            msgSize = BitConverter.ToInt32(_data, place);
            place += sizeof(int);

            _quiz.m_Answer_2 = Encoding.Unicode.GetString(_data, place, msgSize);
            place += msgSize;

            msgSize = BitConverter.ToInt32(_data, place);
            place += sizeof(int);

            _quiz.m_Answer_3 = Encoding.Unicode.GetString(_data, place, msgSize);
            place += msgSize;

            msgSize = BitConverter.ToInt32(_data, place);
            place += sizeof(int);

            _quiz.m_Answer_4 = Encoding.Unicode.GetString(_data, place, msgSize);
            place += msgSize;
        }
        public static void UnPackPacket(byte[] _data, ref UInt16 _player_index, ref UInt64 _x, ref UInt64 _y)
        {
            int place = 0;

            place += sizeof(UInt64);

            _player_index = BitConverter.ToUInt16(_data, place);
            place += sizeof(UInt16);

            _x = BitConverter.ToUInt64(_data, place);
            place += sizeof(UInt64);

            _y = BitConverter.ToUInt64(_data, place);
            place += sizeof(UInt64);
        }
        public static void UnPackPacket(byte[] _data, ref bool _is_client, ref bool _is_correct)
        {
            int place = 0;

            place += sizeof(UInt64);

            _is_client = BitConverter.ToInt16(_data, place) == 1;
            place += sizeof(short);

            _is_correct = BitConverter.ToInt16(_data, place) == 1;
            place += sizeof(short);
        }
        public static void UnPackPacket(byte[] _data, ref UInt16 _player_index, ref UInt64 _quiz, ref short _tile, ref short _combo, ref UInt64 _total_score, ref float _tile_multiplier)
        {
            int place = 0;

            place += sizeof(UInt64);

            _player_index = BitConverter.ToUInt16(_data, place);
            place += sizeof(UInt16);

            _quiz = BitConverter.ToUInt64(_data, place);
            place += sizeof(UInt64);

            _tile = (short)BitConverter.ToUInt16(_data, place);
            place += sizeof(UInt16);

            _combo = (short)BitConverter.ToUInt16(_data, place);
            place += sizeof(UInt16);

            _total_score = BitConverter.ToUInt64(_data, place);
            place += sizeof(UInt64);

            _tile_multiplier = BitConverter.ToUInt64(_data, place) / 10.0f;
            place += sizeof(UInt64);
        }
    }

}
