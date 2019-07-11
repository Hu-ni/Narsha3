using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoreScript : Character
{

    public float CoreHp = 100;

    public override void Attack(GameObject target)
    {
        throw new System.NotImplementedException();
    }


    public override void Skill_E()
    {
        throw new System.NotImplementedException();
    }

    public override void Skill_Q()
    {
        throw new System.NotImplementedException();
    }

    public override void Skill_W()
    {
        throw new System.NotImplementedException();
    }

    public override void TakeDamage(float dmg)
    {

    }


    WaitForSeconds cannonGap = new WaitForSeconds(5f);
    public GameObject cannonBall;
    public Transform shootPoint;
    [SerializeField]
    List<GameObject> innerRange = new List<GameObject>();
    GameObject currentTarget;
    Coroutine coroutine = null;


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
        bool isStay = false;
        foreach (Collider gm in colliders)
        {

            if ((gm.gameObject.CompareTag("Champion") || gm.gameObject.CompareTag("Minion")) && currentTarget == null)
            {
                if (gm.gameObject.CompareTag("Champion"))
                {

                    Debug.Log("플레이어가 요기있네");
                    currentTarget = gm.gameObject;

                    coroutine = StartCoroutine(CannonRun());
                    Debug.Log("캐논 루틴 실행");

                }
                else if (gm.gameObject.CompareTag("Minion"))
                {
                    Debug.Log("적 미니언 요기있네");
                    currentTarget = gm.gameObject;

                    coroutine = StartCoroutine(CannonRun());
                    Debug.Log("캐논 루틴 실행");
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Detect");
        if (other.CompareTag("Champion") || other.CompareTag("Minion"))
            innerRange.Add(other.gameObject);

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
    

    public void TakeDamage(int code)
    {
        CoreHp -= 5;
        this.transform.Find("Canvas/Slider").GetComponent<Slider>().value = CoreHp;
        if (CoreHp <= 0)
        {
            CoreHp = 0;
            GameManager.instance.OnGameOver(code);
            gameObject.SetActive(false);
        }
        Debug.Log("코어 다침! 때린 캐릭터의 팀 코드>> " + code);

    }

    public override void AttackDamage()
    {
        throw new System.NotImplementedException();
    }

    public override void Move(bool isRunning)
    {
        throw new System.NotImplementedException();
    }

    public override void Dead()
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator Skill_P()
    {
        throw new System.NotImplementedException();
    }
}
