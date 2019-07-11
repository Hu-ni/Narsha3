using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionGenerator : MonoBehaviour
{

  [SerializeField]private GameObject minionPrefeb;
    // Start is called before the first frame update
    public WaitForSeconds genGap = new WaitForSeconds(20f);
    void Start()
    {
        Invoke("StartRoutine",10f);
    }

    void StartRoutine() {
        StartCoroutine(GenRoutine());
    }
   IEnumerator GenRoutine() {
        while (true) {
            GenerateMinons();
            yield return genGap;
        }
    }

    public void GenerateMinons() {

        Instantiate(minionPrefeb, transform.position, Quaternion.identity);
    }
}
