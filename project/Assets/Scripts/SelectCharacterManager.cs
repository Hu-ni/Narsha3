using Assets.Scripts;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectCharacterManager : MonoBehaviourPunCallbacks
{
    private int localPlayerActorNumber;
    private int myTeam;
    private int myCharacter;
    private int myTeamCharacter;

    private int[] playerCharacters;

    private float time;

    public Text timeText;
    public Image[] playerImage;
    public Button[] buttons;

    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        localPlayerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        time = 10;

        myCharacter = -1;
        myTeamCharacter = -1;

        Teaming();
    }

    // 팀 배정하기
    private void Teaming()
    {
        // 본인 표시
        playerImage[localPlayerActorNumber - 1].color = Color.blue;
        myTeam = localPlayerActorNumber % 2;
        Debug.Log("ActNum:" + localPlayerActorNumber + " / myTeam:" + myTeam);

        // 닉네임 표시
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            playerImage[i].GetComponentInChildren<Text>().text = players[i].NickName;
        }

        // 1ㄷ1
        if (players.Length == 2)
        {
            playerCharacters = new int[2] { -1, -1 };

            playerImage[2].gameObject.SetActive(false);
            playerImage[3].gameObject.SetActive(false);
        }
        // 2ㄷ2
        else
        {
            playerCharacters = new int[4] { -1, -1, -1, -1 };
        }
    }

    // 캐릭터 클릭
    public void OnClickCharacter(int charNum)
    {
        // 선택된 캐릭터면 선택x
        if (myCharacter == charNum || myTeamCharacter == charNum)
        {
            return;
        }

        // 이미 캐릭터를 선택했으면 선택했던 캐릭터 check를 품
        if (myCharacter != -1)
        {
            this.photonView.RPC("UnCheckCharacter", RpcTarget.All, localPlayerActorNumber, myTeam, myCharacter);
        }

        myCharacter = charNum;
        //CheckCharacter(myTeam, charNum);
        this.photonView.RPC("CheckCharacter", RpcTarget.All, localPlayerActorNumber, myTeam, charNum);
    }

    // 선택한 캐릭터 체크하기
    [PunRPC]
    public void CheckCharacter(int actorNumber, int teamNum, int charNum)
    {
        // 체크한 플레이어를 보여줌
        playerCharacters[actorNumber - 1] = charNum;
        playerImage[actorNumber - 1].color = Color.black;

        // 우리팀의 선택이면 캐릭터를 체크함
        if (myTeam == teamNum)
        {
            if (actorNumber != localPlayerActorNumber)
            {
                myTeamCharacter = charNum;
            }
            buttons[charNum].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            buttons[charNum].GetComponentInChildren<RawImage>().color = new Color(1, 1, 1, 1);
        }
    }

    // 캐릭터 체크풀기
    [PunRPC]
    public void UnCheckCharacter(int actorNumber, int teamNum, int charNum)
    {
        if (myTeam == teamNum)
        {
            buttons[charNum].GetComponent<Image>().color = new Color(1, 1, 1);
            buttons[charNum].GetComponentInChildren<RawImage>().color = new Color(0, 0, 0, 0);
        }
    }

    void Update()
    {
        if (time == -1)
            return;

        time -= Time.deltaTime;
        timeText.text = ((int)time).ToString();
        if (time <= 0)
        {
            time = -1;

            // 만약에 한명이라도 선택을 안 하면 튕기기
            for (int i = 0; i < playerCharacters.Length; i++)
            {
                if (playerCharacters[i] == -1)
                {
                    ExitRoom();
                    return;
                }
            }

            GameStart();
        }
    }

    // 게임 시작
    public void GameStart()
    {
        PlayerCharacterInfo.myTeam = myTeam;
        PlayerCharacterInfo.myCharacter = myCharacter;
        PlayerCharacterInfo.playerCharacters = playerCharacters;

        PhotonNetwork.IsMessageQueueRunning = false;
        SceneManager.LoadScene("GameScene");
    }

    // 방나가기
    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    // 방을 나갔을 때 로비로
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainScene");
    }

    // 방에서 한명이라도 나가면 튕기기
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ExitRoom();
    }

}
