using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using Network.Data;

public class Task_Handler
{

    // 버퍼에서 프로토콜 얻어내기
    public void GetProtocol(byte[] _data, ref UInt64 _protocol)
    {
        int place = 0;
        _protocol = BitConverter.ToUInt64(_data, place);
        place += sizeof(UInt64);
    }

    public void Perform_Task(Manager_Network _manager, Task _task)
    {
        UInt64 protocol = 0;
        string msg = new string('\0', 4096);

        UInt16 player_index = 0;
        UInt64 x = 0, y = 0;

        GetProtocol(_task.buffer, ref protocol);
        _manager.e_ProtocolRecv.Invoke((PROTOCOL)protocol);

        switch((PROTOCOL)protocol)
        {
            case PROTOCOL.DEBUG: // ACK 확인
                Debug.Log("Protocol == debug");
                Sender.Send_Debug();
                break;
            case PROTOCOL.CONNECTED:
                Window_Popup.CreateWindow("대기열 갓겜", "매칭 대기중...", WINDOW_BUTTON_TYPE.CANCEL, new UnityAction( () =>
                {
                    _manager.DIsconnect();
                }), null);
                break;
            case PROTOCOL.MATCHED:
                // Window_Popup.CreateWindow("매칭 성공", "매칭 성공!\n씬 변경...", false, null);
                Window_Popup.Force_CloseWindow();
                Unpacker.UnPackPacket(_task.buffer, ref Manager_Network.Instance.m_Client_Position, ref Manager_Network.Instance.m_OtherPlayer_CharacterIndex);
                Debug.Log("나는 = " + Manager_Network.Instance.m_Client_Position + "번, 너는 " + Manager_Network.Instance.m_OtherPlayer_CharacterIndex + "번");
                Manager_Sound.Instance.FadeOut_BGM(1f);
                Fader fader = Fader.Start_Fade("전장으로 향하는 중...");
                fader.m_FCE.AddListener(new UnityAction(() =>
                {
                    SceneManager.LoadSceneAsync("Ingame");
                }));
                break;
            case PROTOCOL.GAME_START:
                Debug.Log("Protocol == game start");
                _manager.e_GameStart.Invoke();
                break;
            case PROTOCOL.TIMER:
                ulong timer = 0;
                Unpacker.UnPackPacket(_task.buffer, ref timer);
                // Debug.Log("Protocol == timer: " + timer);
                _manager.e_Timer.Invoke(timer);
                break;
            case PROTOCOL.TILE_MARK:
                Unpacker.UnPackPacket(_task.buffer, ref player_index, ref x, ref y);
                Debug.Log("Protocol == mark: " + player_index + " - " + x + ", " + y);
                _manager.e_Player_Mark.Invoke(player_index, x, y);
                break;
            case PROTOCOL.TILE_MARK_ERASE:
                Unpacker.UnPackPacket(_task.buffer, ref player_index);
                Debug.Log("Protocol == mark_erase: " + player_index);
                _manager.e_Player_Mark_Erase.Invoke(player_index);
                break;
            case PROTOCOL.TILE_MOVE:
                Unpacker.UnPackPacket(_task.buffer, ref player_index, ref x, ref y);
                Debug.Log("Protocol == move: " + player_index + " - " + x + ", " + y);
                _manager.e_Player_Move.Invoke(player_index, x, y);
                break;
            case PROTOCOL.TILE_CAPTURE:
                UInt16 tile_key = 0;
                Unpacker.UnPackPacket(_task.buffer, ref tile_key, ref player_index);
                Debug.Log("Protocol == capture: " + player_index + " - " + x + ", " + y);
                _manager.e_Player_Capture.Invoke(tile_key, player_index);
                break;
            case PROTOCOL.TILE_QUESTION:
                Quiz quiz = new Quiz();
                Unpacker.UnPackPacket(_task.buffer, ref quiz);
                Debug.Log("Protocol == quiz: " + quiz.m_Question + " :: " + quiz.m_Answer_1 + ", " + quiz.m_Answer_2 + ", " + quiz.m_Answer_3 + ", " + quiz.m_Answer_4);
                _manager.e_QueationBox.Invoke(quiz);
                break;
            case PROTOCOL.SCORE:
                ScoreData data = new ScoreData();
                Unpacker.UnPackPacket(_task.buffer, ref player_index, ref data.quiz_score, ref data.current_tile, ref data.combo, ref data.total_score, ref data.multiplier);
                Debug.Log("Protocol == score: " + data.ToString());
                _manager.e_Player_UpdateScore.Invoke(player_index, data);
                break;
            case PROTOCOL.SKILL:
                UInt16 skill = 0;
                Unpacker.UnPackPacket(_task.buffer, ref player_index, ref skill);
                Debug.Log("Protocol == skill: " + skill);
                _manager.e_Player_UseSkill.Invoke(player_index, skill);
                break;
            case PROTOCOL.SURRENDER:
                Unpacker.UnPackPacket(_task.buffer, ref msg);
                Debug.Log("Protocol == surrender");
                Window_Result.m_Surrender_Win = true;
                Window_Popup.CreateWindow("부전승", "상대가 항복했어요.", WINDOW_BUTTON_TYPE.OK, null, null);
                _manager.e_GameEnd.Invoke();
                Sender.Send_EndGame();
                _manager.DIsconnect();
                break;
            case PROTOCOL.END_GAME: // 게임 끝이다요
                Debug.Log("Protocol == end game");
                _manager.e_GameEnd.Invoke();
                Sender.Send_EndGame();
                _manager.DIsconnect();
                break;
        }

    }
}