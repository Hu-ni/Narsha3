using Assets.Scripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    private int localPlayerActorNumber;
    private int myTeam;
    private int myCharacter;
    private int myTeamCharacter;
    private int[] playerCharacters;

    private GameObject[] gameCharacters;

    public int globalTime_Sec=0;
    public int globalTime_Min;
    public static GameManager instance = null;
    public Text displayWin;
    public Text displayTime;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        StartCoroutine(RuntimeTime());

        myTeam = PlayerCharacterInfo.myTeam;
        myCharacter = PlayerCharacterInfo.myCharacter;
        playerCharacters = PlayerCharacterInfo.playerCharacters;

        SpawnCharacter(myCharacter);

        StartCoroutine(RuntimeTime());
    }

    private void SpawnCharacter(int charNum)
    {
        Vector3 spawnPos = new Vector3(0, 0, 0);
        Vector3 spawnAgl = new Vector3(0, 0, 0);
        // Team 1
        if (myTeam == 1)
        {
            spawnPos = new Vector3(-50, 0, -30);
            spawnAgl = new Vector3(0, 90, 0);
        }
        // Team 2
        else
        {
            spawnPos = new Vector3(50, 0, 80);
            spawnAgl = new Vector3(0, -90, 0);
        }

        PhotonNetwork.Instantiate("Character" + charNum, spawnPos, Quaternion.Euler(spawnAgl));
    }

    IEnumerator RuntimeTime()
    {
        while (true)
        {

            DisplayTime();
            globalTime_Sec++;
            DisplayTime();
            yield return new WaitForSeconds(1f);
            yield return null;
        }
    }

    private void DisplayTime()
    {
        int min = globalTime_Sec / 60;
        globalTime_Min = min;
        int sec = globalTime_Sec % 60;
        if(sec<10)
            displayTime.text = min + "분 " +"0"+ sec + " 초";
        else
            displayTime.text = min + "분 " +  sec + " 초";
    }
   
    public void OnGameOver(int code)
    {
       // MinionManager.instance.StopGen();
        string format;
        switch (code)
        {
            case 1:
                {
                    format = "[레드]";
                    break;
                }
            case 0:
                {
                    format = "[블루]";
                    break;
                }
            default:
                {
                    format = "[ERROR]";
                    break;
                }
        }

        displayWin.text = format + "팀의 승리!";

        
    }

}
