using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //선택된 캐릭터 코드
    private int SelectedCharactorCode;
    public Transform spawnPoint;

    private void Awake() {
        SelectedCharactorCode = 1;
        GeneratePlayer(SelectedCharactorCode);
    }

    public void GeneratePlayer(int code) {
       
        List<Dictionary<string, object>> data = CSVReader.Read("CharactorInfo");
        for(int i = 0; i <= data.Count; i++) {
            if (Convert.ToInt32(data[i]["CODE"]) == SelectedCharactorCode) {
                Debug.Log("히어로 이름: ["+data[i]["NAME"].ToString()+"]");


                //프리팹 게임오브젝트화
                GameObject player = Resources.Load("Player") as GameObject;
                
              
                //하위 모델(비주얼 그래픽)
                GameObject playerModel = Resources.Load("Models/" + data[i]["NAME"].ToString()) as GameObject;
                
                
                //실제 생성
                GameObject genedPlayer=Instantiate(player, spawnPoint.transform.position, Quaternion.identity);

                GameObject Model = Instantiate(playerModel,genedPlayer.transform);
                

                //Model.transform.parent = genedPlayer.transform;
                //genedPlayer.GetComponent<Player>().character = (Character)genedPlayer.AddComponent<SwordMan>();
                genedPlayer.GetComponent<Players>().visualGraphic = Model;
                FollowCamera.instance.playerTrasnfrom = genedPlayer.transform;
                //Debug.Log("플레이어 생성....");
                //Debug.Log(genedPlayer.GetComponent<Player>().GetComponent<Animator>().runtimeAnimatorController.ToString());

                
                

                

                
                break;
            }
        }
    }
    
}
