using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Players : MonoBehaviour
{
    [SerializeField] public Character character;

    
    public GameObject visualGraphic;
    
 
    private PlayerController playerController;

    private void Awake() {
        
        playerController = gameObject.GetComponent<PlayerController>();
        

    }

    private void Start() {

       
           
        
    }

   
        
}
