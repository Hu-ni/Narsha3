using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.DateTime;

public class Golem : Character
{
    //길 찾기 AI
    private UnityEngine.AI.NavMeshAgent agent;

    //리스폰 시간
    public float respawnTime = 3;

    //애니메이터
    private Animator animator;
    private PlayerController playerController;

    private float skillDamage;

    [Header("Skill Time")]
    //스킬 쿨타임
    public float Q_Time = 0;
    public float W_Time = 0;
    public float E_Time = 0;
    
    [Header("- Status")]
    //상태 변수들
    public bool dead = false;
    public bool evasion = false;
    public bool attack = false;
    public bool stun = false;
    public bool skillq = false;

    [Header("- Count")]
    //킬, 데스, 스텟 포인트
    public int Kill_Count = 0;
    public int Dead_Count = 0;
    public int Stat_Point = 0;

    [Header("Extra_Stat")]
    //포인트에 따른 추가 스탯
    public int Extra_HP = 0;
    public int Extra_Resource = 0;
    public int Extra_Attack = 0;
    public int Extra_MoveSpeed = 0;
    public int Extra_AttackSpeed = 0;

    //경험치 필요량
    private int exp_need;

    //패시브 초기화 시간, 패시브 추가 방어력, 스킬 적중시 여러명이 맞아도 패시브 중첩이 1번만 쌓이게 하기 위한 변수
    private float Passive_Time = 0;
    private int Passive_ExtraDefense = 0;
    private bool AddPassiveCheck = false;

    ~Golem()
    {
        StopCoroutine("ChargeHP");
        StopCoroutine("ChargeMP");
        StopCoroutine("PassiveReset");
    }

    private void Awake()
    {
        //초기화
        Set();

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //캐릭터 이동속도 설정
        agent.speed = SPEED();

        //애니메이터
        animator = GetComponent<Animator>();
        animator.SetFloat("Speed", SPEED());
        animator.SetFloat("AttackSpeed", ATTACKSPEED());
    }

    private void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();
        StartCoroutine("ChargeHP");
        StartCoroutine("ChargeMP");
        StartCoroutine("PassiveReset");
    }

    #region 초기화
    public override void Set()
    {
        status.hp = 630;
        status.mp = 200;
        status.power = 83;
        status.speed = 2.8f;
        status.attackspeed = 0.67f;
        status.attackRange = 1.5f;

        currentStatus = status;

        //레벨업에 필요한 경험치 설정
        EXP();
    }
    #endregion

    #region 이동, 공격
    public override void Move(bool isRunning)
    {
        if (dead)
            return;
        
        animator.SetBool("Walk", isRunning);
    }

    public override void Attack()
    {
        if (attack)
        {
            return;
        }
        playerController.SetMoveable(0);
        StartCoroutine("crtAttack");
    }

    IEnumerator crtAttack()
    {
        animator.SetBool("Attack", true);
        attack = true;
        //idle이나 walk 애니메이션에서 공격 애니메이션으로 넘어올때까지 기다리기
        while (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("idle") || animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("walk"))
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        //공격 애니메이션 길이만큼 기다리기
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / animator.GetFloat("AttackSpeed"));
        playerController.SetMoveable(1);
        animator.SetBool("Attack", false);

        attack = false;
        yield return null;
    }

    public void AttackDamage()
    {
        if (skillq)
            skillDamage = 80 + (this.currentStatus.hp * 0.06f * (Extra_Attack == 0 ? 1 : 1+Extra_Attack)) + (5 * (this.currentStatus.power / 100.0f * (Extra_Attack == 0 ? 1 : 1+Extra_Attack)));
        else
            skillDamage = this.currentStatus.power;
        Debug.Log(skillDamage);
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, GetComponent<CapsuleCollider>().bounds.size / 2, transform.forward, out hit, transform.rotation, this.currentStatus.attackRange))
        {
            if (hit.transform.tag == "Enemy")
            {
                SendDamage(hit.transform.gameObject);
                if (skillq)
                {
                    skillq = false;
                    //둔화
                    float enemyspeed = hit.transform.GetComponent<Character>().currentStatus.speed;
                    hit.transform.GetComponent<Character>().currentStatus.speed *= 0.65f;
                    float now = 0;
                    while(now <= 2)
                    {
                        now += Time.deltaTime;
                    }
                    hit.transform.GetComponent<Character>().currentStatus.speed *= enemyspeed;

                    //둔화
                    //hit.transform.GetComponent<Character>().Slow(2, 35); //시간, 퍼센트
                }
            }
            else if (hit.transform.tag == "Minion")
            {
                //미니언 데미지 주기
                //hit.transform.GetComponent<Minion>().Damage();
            }
        }
    } 
    #endregion

    #region 스킬
    public void AddExtraDefense()
    {
        if (Passive_ExtraDefense >= 6)
            return;

        Passive_ExtraDefense++;
    }

    public override void Skill_Q()
    {
        if (Q_Time > 0 || this.currentStatus.mp < 25)
            return;

        skillq = true;
        UseMP(25);
        AddPassiveCheck = false;
    }

    public override void Skill_W()
    {
        if (W_Time > 0 || this.currentStatus.mp < 30)
            return;

        playerController.SetMoveable(0);
        transform.LookAt(agent.pathEndPosition);

        AddPassiveCheck = false;

        UseMP(30);
        animator.SetTrigger("Skill_W");
        StartCoroutine("UseW");
    }

    public override void Skill_E()
    {
        if (E_Time > 0 || this.currentStatus.mp < 50)
            return;

        playerController.SetMoveable(0);
        transform.LookAt(agent.pathEndPosition);

        AddPassiveCheck = false;

        UseMP(50);
        animator.SetBool("Skill_E", true);
        StartCoroutine("UseE");
    }

    #region 스킬 히트 박스 활성화, 비활성화
    public void Skill_W_HitBox_Enable()
    {
        float Attack = currentStatus.power;
        skillDamage = 90 + (this.status.hp * 0.045f * GetSkillCoefficient()) + (15 * (Attack / 100.0f * GetSkillCoefficient()));
        transform.Find("Skill_W_HitBox").gameObject.SetActive(true);
    }

    public void Skill_W_HitBox_Disable()
    {
        skillDamage = 0;
        transform.Find("Skill_W_HitBox").gameObject.SetActive(false);
    }
    #endregion

    #region 스킬 코루틴
    IEnumerator PassiveReset()
    {
        while (true)
        {
            if (Passive_ExtraDefense == 0)
            {
                yield return null;
            }
            else
            {
                Passive_Time += Time.deltaTime;
                if(Passive_Time >= 4)
                {
                    Passive_Time = 0;
                    Passive_ExtraDefense = 0;
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }

    IEnumerator UseW()
    {
        W_Time = 5;
        agent.ResetPath();

        while (W_Time > 0)
        {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("attack_2") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.94f)
            {
                animator.SetBool("Walk", false);
                playerController.SetMoveable(1);
            }
            W_Time -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    IEnumerator UseE()
    {
        E_Time = 16;

        Vector3 position = gameObject.transform.Find("Skill_E_Point").transform.position;

        agent.speed = SPEED() * 1.5f;
        agent.SetDestination(position);
        
        while (E_Time > 0)
        {
            if (Vector3.Distance(position, gameObject.transform.position) <= 0.1f || (16 - E_Time) > (7 / agent.speed))
            {
                if (Mathf.Approximately(agent.speed, (SPEED() * 1.5f)))
                {
                    Collider[] colls = Physics.OverlapSphere(transform.position, 2);
                    foreach(Collider coll in colls)
                    {
                        if (coll.gameObject.tag == "Enemy" && coll.gameObject != gameObject)
                        {
                            //coll.getcomponent<character>().stun(1);
                            skillDamage = 75 + (this.status.hp * 0.1f * GetSkillCoefficient()) + (10 * (this.currentStatus.power / 100.0f * GetSkillCoefficient()));
                            SendDamage(coll.gameObject);
                        }
                    }
                    animator.SetBool("Skill_E", false);
                    animator.SetBool("Walk", false);
                    agent.speed = SPEED();
                    agent.ResetPath();
                    playerController.SetMoveable(1);
                }
            }

            E_Time -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    #endregion
    #endregion

    #region 상태이상
    public override void Dead()
    {
        Dead_Count++;
        dead = true;

        animator.SetBool("Dead", true);

        Debug.Log("DeadCnt : " + Dead_Count);

        StartCoroutine("Respawn");
    }

    public void Slow(float SlowTime, float SlowValue)
    {
        StartCoroutine(crtSlow(SlowTime, SlowValue));
    }

    IEnumerator crtSlow(float SlowTime, float SlowValue)
    {
        agent.speed *= (1 - (0.01f * SlowValue));
        while(SlowTime <= 0)
        {
            SlowTime -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        agent.speed = SPEED();
    }

    public void Stun(float StunTime)
    {
        animator.SetFloat("StunTime", StunTime);
        StartCoroutine("crtStun");
    }

    IEnumerator crtStun()
    {
        stun = true;

        animator.SetBool("Stun", true);
        yield return new WaitForSeconds(animator.GetInteger("StunTime"));
        animator.SetBool("Stun", false);
        stun = false;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);
        transform.Find("Golem").gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnTime);
        animator.SetBool("Dead", false);
        dead = false;
        this.currentStatus.hp = this.status.hp;
        transform.position = new Vector3(0, 0, 0);
        transform.Find("Golem").gameObject.SetActive(true);
    }
    #endregion

    #region 추가 스탯 관리
    //체력재생, 10퍼 추가체력
    private void AddExtraHP()
    {
        if (Extra_HP >= 10)
        {
            return;
        }
        Extra_HP++;
        Stat_Point--;
        this.status.hp *= 1.1f;
    }

    //자원재생, 10퍼 추가자원
    private void AddExtraResource()
    {
        if (Extra_Resource >= 10)
        {
            return;
        }
        Extra_Resource++;
        Stat_Point--;
        this.status.mp *= 1.1f;
    }
    //기본공격력 20만큼
    private void AddExtraAttack()
    {
        if (Extra_Attack >= 10)
        {
            return;
        }
        Extra_Attack++;
        Stat_Point--;
        this.currentStatus.power = POWER();
    }
    //스킬 계수를 위한 함수
    private int GetSkillCoefficient()
    {
        return Extra_Attack == 0 ? 1 : (1+Extra_Attack);
    }
    //5%
    private void AddExtraMoveSpeed()
    {
        if (Extra_MoveSpeed >= 10)
        {
            return;
        }
        Extra_MoveSpeed++;
        Stat_Point--;
        animator.SetFloat("Speed", SPEED());
    }
    //15퍼
    private void AddExtraAttackSpeed()
    {
        if (Extra_AttackSpeed >= 10)
        {
            return;
        }
        Extra_AttackSpeed++;
        Stat_Point--;
        animator.SetFloat("AttackSpeed", ATTACKSPEED());
    }
    #endregion

    #region 스탯 관련
    //체력 및 마나 재생
    IEnumerator ChargeHP()
    {
        while (true)
        {
            if (!dead && !Mathf.Approximately(this.currentStatus.hp, this.status.hp))
            {
                this.currentStatus.hp += (4 * Extra_HP);
                yield return new WaitForSeconds(1);
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator ChargeMP()
    {
        while (true)
        {
            if (!dead && !Mathf.Approximately(this.currentStatus.mp, this.status.mp))
            {
                this.currentStatus.mp += (4 * Extra_Resource);
                yield return new WaitForSeconds(1);
            }
            else
            {
                yield return null;
            }
        }
    }
    //경험치 설정
    public float EXP()
    {
        exp_need = (100 + (this.currentStatus.level - 1) * 75);
        return exp_need;
    }

    public void GetEXP(int EXP)
    {
        //레벨 제한 20
        if (this.currentStatus.level == 20)
            Debug.Log("최대 레벨이라 더 이상 경험치를 획득할 수 없습니다.");

        this.currentStatus.exp += EXP;
        while (this.currentStatus.exp >= exp_need)
        {
            this.currentStatus.exp -= exp_need;
            LevelUp();
            this.EXP();
        }
    }

    public void LevelUp()
    {
        this.currentStatus.level++;
        this.Stat_Point += 2;
    }

    public float POWER()
    {
        currentStatus.power = status.power + ((Extra_Attack + Extra_Resource) * status.power * 0.2f);
        return currentStatus.power;
    }

    public void UseMP(int MP)
    {
        this.currentStatus.mp -= MP;
    }

    public float ATTACKSPEED()
    {
        currentStatus.attackspeed = status.attackspeed + (Extra_AttackSpeed * status.attackspeed * 0.15f);
        return currentStatus.attackspeed;
    }

    public float SPEED()
    {
        currentStatus.speed = status.speed + (Extra_MoveSpeed * this.status.speed * 0.05f);
        agent.speed = currentStatus.speed;
        return currentStatus.speed;
    }
    #endregion

    #region 데미지 연산

    public override void SendDamage(GameObject target)
    {
        //패시브
        if (skillq || !animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("attack_1"))
        {
            if (!AddPassiveCheck)
            {
                AddPassiveCheck = true;
                AddExtraDefense();
            }
            Passive_Time = 0;
        }

        target.GetComponent<Character>().TakeDamage(skillDamage);
        Debug.Log("데미지 줬다.");
    }

    public override void TakeDamage(float damage)
    {
        if (evasion)
            return;
        
        float DefenseDamage = damage * (0.9f - (Passive_ExtraDefense * 0.05f));
        this.currentStatus.hp -= DefenseDamage;
        if (this.currentStatus.hp <= 0)
        {
            Dead();
            dead = true;
        }
    }

    public override void Damage()
    {
        if (evasion)
            return;

        float DefenseDamage = skillDamage * (0.9f - (Passive_ExtraDefense * 0.05f));

        this.currentStatus.hp -= DefenseDamage;
        Debug.Log(this.currentStatus.hp);
        if (this.currentStatus.hp <= 0)
        {
            Dead();
            dead = true;
        }
    }

    #endregion
}