using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : MonoBehaviour
{

    public GameObject NearMinion;
    public GameObject LongDistanceMinion;
    public GameObject Spawner_Team1;
    public GameObject Spawner_Team2;
    public bool isGameEnd = false;

    #region Singleton

    public static MinionManager instance;

    void Awake()
    {
        instance = this;
    }

    #endregion


    public void StopGen()
    {
        Spawner_Team1.SetActive(false);
        Spawner_Team2.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
