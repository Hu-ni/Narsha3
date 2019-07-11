using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.SceneManagement;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    public readonly string gameVersion = "1.0";

    private bool isMatching = false;

    public GameObject[] panels;
    public GameObject matchingPanel;

    public enum ActivePanel
    {
        LOGIN = 0,
        LOBBY = 1
    }

    private string nickName;
    public InputField nickNameField;
    public Text nickNameText;

    private List<RoomInfo> roomList = new List<RoomInfo>();

    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        nickNameField.text = "User" + UnityEngine.Random.Range(0, 100);
        ChangePanel(ActivePanel.LOGIN);
    }

    // 로그인
    public void OnClickLogin()
    {
        nickName = nickNameField.text;
        PhotonNetwork.GameVersion = this.gameVersion;
        PhotonNetwork.NickName = nickName;
        PhotonNetwork.ConnectUsingSettings();
    }

    // 마스터에 연결됨 [오버라이드]
    public override void OnConnectedToMaster()
    {
        // Debug.Log("마스터에 연결됨");
        ChangePanel(ActivePanel.LOBBY);
        nickNameText.text = PhotonNetwork.NickName;
        PhotonNetwork.JoinLobby();
    }

    // 로비에 진입 [오버라이드]
    public override void OnJoinedLobby()
    {
        isMatching = false;

        matchingPanel.SetActive(false);
    }

    // panel 변경
    private void ChangePanel(ActivePanel panelEnum)
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
        panels[(int)panelEnum].SetActive(true);
    }

    private int curMaxPlayer;

    // 자동매칭 시작
    public void OnClickAutoMatching(int maxPlayer)
    {
        // 로비가 아닐 시 매칭 X
        if (!PhotonNetwork.InLobby)
        {
            // Debug.Log("연결 안 됨");
            return;
        }

        // 이미 매칭 중이라면 매칭 X
        if (isMatching)
        {
            // Debug.Log("이미 매칭 중");
            return;
        }

        curMaxPlayer = maxPlayer;
        JoinRoomMaxPlayer(maxPlayer);   // 방에 참가
    }

    // 방에 참가 방이 없으면 직접 만들어 참가
    private void JoinRoomMaxPlayer(int maxPlayer)
    {
        isMatching = true;

        matchingPanel.SetActive(true);
        matchingPanel.GetComponentInChildren<Text>().text = "검색중";

        // 방을 검색해서 
        foreach (RoomInfo room in roomList)
        {
            if (room.MaxPlayers == maxPlayer)
            {
                if (room.MaxPlayers == room.PlayerCount)
                {
                    continue;
                }

                PhotonNetwork.JoinRoom(room.Name, null);
                return;
            }
        }

        // 방이 없을  때 직접 만듬
        PhotonNetwork.CreateRoom(null, new RoomOptions
        {
            MaxPlayers = (byte)maxPlayer
        });
    }

    // 방에 참가했을 때 [오버라이드]
    public override void OnJoinedRoom()
    {
        matchingPanel.SetActive(true);
        matchingPanel.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerList.Length + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        CheckRoomFull();
    }

    // 방 참가 실패 [오버라이드]
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        JoinRoomMaxPlayer(curMaxPlayer);    // 다시 참가
    }

    // 누군가 방에 들어왔을 때 [오버라이드]
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        matchingPanel.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerList.Length + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        CheckRoomFull();
    }

    // 누군가 방을 떠났을 때 [오버라이드]
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        matchingPanel.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerList.Length + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        base.OnPlayerLeftRoom(otherPlayer);
    }

    // 방이 가득차면 게임시작
    private void CheckRoomFull()
    {
        int currPlayer = PhotonNetwork.PlayerList.Length;
        int maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;
        if (currPlayer == maxPlayer)
        {
            StartGame();
        }
    }

    // 매칭 취소 버튼 클릭 시
    public void OnClickMatchingCanel()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            matchingPanel.GetComponentInChildren<Text>().text = "취소중";
        }
    }

    // 방을 떠났을 때 [오버라이드]
    public override void OnLeftRoom()
    {
        // 방을 떠나면 다시 마스터와 연결 시도 후 로비로 진입
    }

    // 게임 시작
    private void StartGame()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        SceneManager.LoadScene("SelectCharacterScene");
    }

    // 방 업데이트 시 방 목록 새로고침 [오버라이드]
    public override void OnRoomListUpdate(List<RoomInfo> newRoomList)
    {
        // Debug.Log("방 새로고침");
        for (int i = 0; i < newRoomList.Count; i++)
        {
            RoomInfo newRoom = newRoomList[i];
            if (roomList.Contains(newRoom))
            {
                roomList.Remove(newRoom);
                if (newRoom.PlayerCount == 0)
                {
                    continue;
                }
            }

            if (newRoom.PlayerCount != 0)
                roomList.Add(newRoom);
        }
    }
}