using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour,UnitBasic
{
    private float HP = 300;
    WaitForSeconds cannonGap = new WaitForSeconds(5f);
    public GameObject cannonBall;
    public Transform shootPoint;
    [SerializeField]
    List<GameObject> innerRange = new List<GameObject>();
    public GameObject currentTarget;
    Coroutine coroutine = null;
    public int TeamCode;

    // Start is called before the first frame update
    void Start()
    {
        currentTarget = null;
   
    }

    // Update is called once per frame
    void Update()
    {
        DetectorUpdate();
    }
    public float detectRange;

    

    void DetectorUpdate()
    {   
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, detectRange, 9);//9번 레이어에 해당하는 collider만 처리하겠다.
        bool isStay = false ;
        foreach(Collider gm in colliders)
        {
         
            if ((gm.gameObject.CompareTag("Champion")||gm.gameObject.CompareTag("Minion"))&&currentTarget==null)
            {
                if (gm.gameObject.CompareTag("Champion"))
                {
                    if (gm.gameObject.GetComponent<Character>().team != TeamCode)
                    {
                        Debug.Log("플레이어가 요기있네");
                        currentTarget = gm.gameObject;

                        coroutine = StartCoroutine(CannonRun());
                        Debug.Log("캐논 루틴 실행");
                    }
                }
                else if (gm.gameObject.CompareTag("Minion"))
                {
                    if (gm.gameObject.GetComponent<UnitBasic>().GetTeamCode() != TeamCode)
                    {
                        Debug.Log("적 미니언 요기있네");
                        currentTarget = gm.gameObject;

                        coroutine = StartCoroutine(CannonRun());
                        Debug.Log("캐논 루틴 실행");
                    }
                }
               
            }
            if (currentTarget != null)
            {
                if (gm.gameObject == currentTarget)
                {
                    isStay = true;
                    Debug.Log("다 아는놈이구만");
                }
            }
        
        }
        if (!isStay) { currentTarget = null; if (coroutine != null) { StopCoroutine(coroutine); coroutine = null; } }
    }

    IEnumerator CannonRun()
    {
        while (true)
        {
            Debug.Log("터렛 공격중...");
            GameObject gened = Instantiate(cannonBall, shootPoint.position, Quaternion.identity);
            gened.GetComponent<CannonBall>().target = currentTarget.transform;
            yield return cannonGap;
        }
    }

    public void getDamage(float dmg,GameObject from)
    {
        if (from.CompareTag("Champion"))
        {
            innerRange.Insert(0, from.gameObject);//챔피언은 우선순위 최우선
        }
        else if (from.CompareTag("Minion"))
        {
            innerRange.Add(from.gameObject);
        }
    }
    
 
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }

    public void TakeDamage(float dmg, GameObject from)
    {
        throw new System.NotImplementedException();
    }

    public void SetTeamCode(int code)
    {
        this.TeamCode = code;
    }

    public int GetTeamCode()
    {
        return this.TeamCode;
    }
}
