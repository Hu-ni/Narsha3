using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LeftTime : MonoBehaviour {
    
    public Text Time;
    int oneSecond = 5;
    IEnumerator coroutine;

    // Use this for initialization
    void Start () {
        StartCoroutine("Timer");
    }
	
	// Update is called once per frame
	void Update () {
        Time.text = ""+oneSecond;
	}

    IEnumerator Timer() {
        if (oneSecond == 0)
            StopCoroutine(coroutine);
        yield return new WaitForSeconds(1f);
        oneSecond--;
        StartCoroutine("Timer");
    }
}
