using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
   public static SelectManager instace;
    GameObject selectedCharacter = null;
    int selectNum;
    public float spinSpeed;
    [SerializeField]
    private Transform showPosition;


    


    private void Awake() {
        //instace = this;
    }

    private void Update() {
        if (selectedCharacter != null) {
            //selectedCharacter.gameObject.transform.rotation.y + (Time.deltaTime * spinSpeed);
            selectedCharacter.gameObject.transform.Rotate(Vector3.up * (Time.deltaTime*spinSpeed));
        }
    }


    public void SelectChar(string name) {

        if(selectedCharacter!=null)
        Destroy(selectedCharacter.gameObject);
        GameObject playerModel = Resources.Load("Models/" + name) as GameObject;
        selectedCharacter = Instantiate(playerModel, showPosition);

        //  Instantiate(playerModel, genedPlaye.transform);
    }

   


}
