using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinionController :MonoBehaviour,UnitBasic
{

    public float lookRadius;

    private enum MinionState { idle, move, longattack, nearattack, die };
    private MinionState minionState = MinionState.idle;

    private float attackNearRadius = 3f; //근거리 공격 거리
    private float attackLongRadius = 4f; //원거리 공격 거리
    private float selectattackRadius; //미니언 종류에 따른 공격 거리 설정

    private int NearMinionAttack = 30; //근거리 미니언 공격력
    private int LongDistanceMinionAttack = 45; //원거리 미니언 공격력
    private int selectAttack; //미니언 종류에 따른 공격력 설정

    private float minionHP;
    private float currentHP;
    private bool isDie = false;

    private Animator animator;
    [SerializeField]
    private Transform target; //챔피언 거리
    [SerializeField]
    private GameObject minionobj;
    private NavMeshAgent agent;

    private int ownExp;
    private int increaseRate;
    [SerializeField]
   private int teamCode;

    public event Action<float> OnHealthPercentChanged = delegate { };


    void Awake()
    {
        animator = this.gameObject.GetComponentInChildren<Animator>();
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        for(int i = 0; i < towers.Length; i++)
        {
            if (towers[i].gameObject.GetComponent<UnitBasic>().GetTeamCode() != teamCode)
            {
                target = towers[i].transform;
                break;
            }
        }

        agent = this.gameObject.GetComponent<NavMeshAgent>();

        if (transform.name.Contains("NearMinion"))
        {
            minionHP = 220;
            ownExp = 25;
            increaseRate = 10;
            //minionHP = 10; //테스트용 HP
            selectattackRadius = attackNearRadius;
            selectAttack = NearMinionAttack;
        }


        else if (transform.name.Contains("LongDistanceMinion"))
        {
            minionHP = 130;
            ownExp = 15;
            increaseRate = 7;
            selectattackRadius = attackLongRadius;
            selectAttack = LongDistanceMinionAttack;
        }

        currentHP = minionHP;
    }

    // Start is called before the first frame update
    void Start()
    {
        agent.SetDestination(target.position);
        StartCoroutine(checkState());
        StartCoroutine(minionAction());
    }

    void Update()
    {
        /*if (transform.name.Contains("Team1"))
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.z + 180f, transform.rotation.z));
        else
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.y, transform.rotation.z));*/
    }

    IEnumerator checkState() //미니언 상태 설정
    {
        while (!isDie)
        {
            float champdistance = Vector3.Distance(target.position, transform.position); //챔피언과 미니언 간의 거리 구하기
         
            Collider[] allobjects = Physics.OverlapSphere(transform.position, 10f);
            List<Transform> targetList = new List<Transform>();
            
            float minDistance = 10f; //최소 거리 값 저장 변수

            for (int i = 0; i < allobjects.Length; i++)
            {
                if (allobjects[i].name.Contains("Minion")) //미니언만 대소 비교를 함
                {

                    if (allobjects[i].name.Substring(0, 5).Equals(transform.name.Substring(0, 5))) //같은 팀 미니언이라면 거리 대소비교 안함
                    {
                        continue;
                    }

                    else
                    {
                        float distance = Vector3.Distance(transform.GetChild(0).GetChild(0).transform.position, allobjects[i].transform.GetChild(0).GetChild(0).position); //챔피언 - 미니언 간 거리 계산

                        if(minDistance > distance && distance != 0)
                        {
                            minDistance = distance;
                            minionobj = allobjects[i].gameObject;
                        }

                    }

                }

                else if (allobjects[i].CompareTag("Champion"))
                {
                    if (allobjects[i].gameObject.GetComponent<Character>().team != this.teamCode)
                    {

                        targetList.Add(allobjects[i].gameObject.transform);
                    }
                   
                }

                else if (allobjects[i].CompareTag("Tower"))
                {
                    if(allobjects[i].gameObject.GetComponent<UnitBasic>().GetTeamCode() != this.teamCode)
                    {

                        targetList.Add(allobjects[i].gameObject.transform);
                    }
                }

            }

            if (targetList.Count > 0)
            {
                Transform MIN = targetList[0];
                for (int k = 0; k < targetList.Count; k++)
                {
                    if (Vector3.Distance(MIN.transform.position, transform.position) > Vector3.Distance(targetList[k].transform.position, transform.position))
                    {
                        MIN = targetList[k];
                    }
                }

                Debug.Log("MIN : " + MIN.name);
                target = MIN;

                float objDistance = Vector3.Distance(MIN.transform.position, transform.position);

                if (objDistance <= selectattackRadius)
                {
                    agent.isStopped = true;
                    if (selectattackRadius == attackLongRadius)
                        minionState = MinionState.longattack;
                    else
                        minionState = MinionState.nearattack;
                }
                else
                {
                    agent.isStopped = false;
                    agent.SetDestination(target.transform.position);
                    minionState = MinionState.move;
                }
                setMinionRotation(target.gameObject);
            }



            /*else if (champdistance <= minDistance && champdistance <= 5f) //챔피언 거리가 가깝다면
            {
                agent.isStopped = false;
                agent.SetDestination(target.transform.position);
                setMinionRotation(target.gameObject);

                if (champdistance <= selectattackRadius)
                {
                    agent.isStopped = true;
                    if (selectattackRadius == attackLongRadius)
                        minionState = MinionState.longattack;
                    else
                        minionState = MinionState.nearattack;
                }
            }*/

            else if (minDistance <= selectattackRadius) //미니언 거리가 가깝다면
            {
                agent.isStopped = false;
                agent.SetDestination(minionobj.transform.position);
                setMinionRotation(minionobj);

                if (selectattackRadius == attackLongRadius)
                {
                    agent.isStopped = true;
                    minionState = MinionState.longattack;
                }
                else if (selectattackRadius == attackNearRadius)
                {
                    agent.isStopped = true;
                    minionState = MinionState.nearattack;
                }
                else //아무 것도 아니라면 이동
                {
                    agent.isStopped = false;
                    agent.SetDestination(minionobj.transform.position);
                    setMinionRotation(minionobj);

                    minionState = MinionState.move;
                }
            }
            targetList.Clear();
            yield return new WaitForSeconds(0.2f);
        }

        yield return null;
 
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.name.Contains("Sword"))
        {
            if (transform.name.Substring(0, 5).Equals(other.transform.root.name.Substring(0, 5)))
            {
                return;
            }
            currentHP -= selectAttack;
            float currentHealthPercent = (float)currentHP / (float)minionHP;
            OnHealthPercentChanged(currentHealthPercent);
        }

        else if (other.name.Contains("Projectile"))
        {
            if (transform.name.Substring(0, 5).Equals(other.transform.root.name.Substring(0, 5)))
            {
                return;
            }

            currentHP -= selectAttack;
            float currentHealthPercent = (float)currentHP / (float)minionHP;
            OnHealthPercentChanged(currentHealthPercent);
        }

        if (currentHP <= 0)
        {
            Debug.Log("minion die");
            agent.isStopped = true;
            minionState = MinionState.die;
        }

    }

    IEnumerator minionAction() //미니언 애니메이션 설정
    {
        while (!isDie)
        {
            switch (minionState)
            {
                case MinionState.move:

                    if (animator)
                    {
                        animator.SetBool("IsWalk", true);
                    }
                    break;

                case MinionState.nearattack:

                    if (animator)
                    {
                        animator.SetBool("IsWalk", false);
                        animator.SetTrigger("NearAttack");
                    }
                    break;

                case MinionState.longattack:

                    if (animator)
                    {
                        animator.SetBool("IsWalk", false);
                        animator.SetTrigger("LongAttack");
                    }
                    break;

                case MinionState.die:

                    if (animator)
                    {
                        animator.SetTrigger("IsDie");
                        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
                        isDie = true;
                    }
                    break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        while (true)
        {
            GameObject obj = GameObject.Find(gameObject.name.Substring(22));
            if (obj)
                Destroy(obj);
            else
                break;
        }
        Destroy(gameObject);
        yield return null;
    }

    IEnumerator WaitForAnimation(Animation animation)
    {
        Debug.Log(1);
        do
        {
            yield return null;
        } while (animation.isPlaying);
        yield return null;
    }

    private void setMinionRotation(GameObject obj) //미니언 방향 설정
    {
        float smooth = 10f;

        Vector3 direction = (obj.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * smooth);

        /*if (transform.name.Contains("Team1")) //Team1인 경우 y축 180도 회전해야 함.
        {
            var newRotation = new Vector3(obj.transform.position.x, 180f, obj.transform.position.y);
            transform.rotation = Quaternion.LookRotation(agent, velocity.normalized);
        }
        else if(transform.name.Contains("Team2"))
        {
            var newRotation = obj.transform.position;
            transform.rotation = Quaternion.LookRotation()
        }*/

    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, selectattackRadius);
    }

    public void TakeDamage(float dmg, GameObject from)
    {
        currentHP -= dmg;
        float currentHealthPercent = (float)currentHP / (float)minionHP;
        OnHealthPercentChanged(currentHealthPercent);
        if (currentHP <=0)
        {

            int expNow = ownExp + (increaseRate * GameManager.instance.globalTime_Min);
            // Debug.Log("적립된 EXP>> " + (ownExp + (increaseRate * GameManager.instance.globalTime_Min)));//1분 이하면 exp증가율이 없다...
            from.GetComponent<Character>().GetEXP(expNow);
            minionState = MinionState.die;
        }
    }
    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;
        float currentHealthPercent = (float)currentHP / (float)minionHP;
        OnHealthPercentChanged(currentHealthPercent);
        if (currentHP <= 0)
        {
            minionState = MinionState.die;
        }
    }

    public void SetTeamCode(int code)
    {
        teamCode = code;
    }

    public int GetTeamCode()
    {
        return teamCode;
    }
}
