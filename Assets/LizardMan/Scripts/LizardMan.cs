using System.Collections;
using UnityEngine;



public class LizardMan : Character
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
    public GameObject SkillQ;
    public GameObject SkillW;
    public GameObject SkillE;

    [Header("- Status")]
    //상태 변수들
    public bool dead = false;
    public bool evasion = false;
    public bool attack = false;
    public bool stun = false;

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

    ~LizardMan()
    {
        StopCoroutine("ChargeHP");
        StopCoroutine("ChargeMP");
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
    }

    #region 초기화
    public override void Set()
    {
        status.hp = 540;
        status.mp = 0;
        status.power = 71;
        status.speed = 3;
        status.attackspeed = 0.7f;
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
        while (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "idle" || animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "walk")
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
        skillDamage = this.currentStatus.power;
        RaycastHit hit;
        if(Physics.BoxCast(transform.position, GetComponent<CapsuleCollider>().bounds.size / 2, transform.forward, out hit, transform.rotation, this.currentStatus.attackRange))
        {
            if(hit.transform.tag == "Enemy")
            {
                SendDamage(hit.transform.gameObject);
            }
            else if (hit.transform.tag == "Minion")
            {
                //미니언 데미지 주기
            }
        }
    }

    #endregion

    #region 스킬
    public override void Skill_Q()
    {
        if (Q_Time > 0)
            return;

        animator.SetBool("Walk", false);

        playerController.SetMoveable(0);
        transform.LookAt(agent.pathEndPosition);

        SkillQ.SetActive(false);
        SkillQ.SetActive(true);
        animator.SetBool("Skill_Q", true);
        Skill_Q_HitBox_Enable();
        StartCoroutine("UseQ");
    }

    public override void Skill_W()
    {
        if (W_Time > 0)
            return;

        animator.SetBool("Walk", false);

        playerController.SetMoveable(0);
        transform.LookAt(agent.pathEndPosition);

        SkillW.SetActive(false);
        SkillW.SetActive(true);
        animator.SetTrigger("Skill_W");
        animator.SetBool("Walk", false);
        StartCoroutine("UseW");
    }

    public override void Skill_E()
    {
        if (E_Time > 0)
            return;
        
        animator.SetBool("Walk", false);

        evasion = true;
        playerController.SetMoveable(0);
        transform.LookAt(agent.pathEndPosition);

        SkillE.SetActive(false);
        SkillE.SetActive(true);
        animator.SetTrigger("Skill_E");
        StartCoroutine("UseE");
    }

    #region 스킬 히트 박스 활성화, 비활성화
    public void Skill_Q_HitBox_Enable()
    {
        skillDamage = 65 + (15 * (this.currentStatus.power / 100.0f * GetSkillCoefficient()));
        transform.Find("Skill_Q_HitBox").gameObject.SetActive(true);
    }

    public void Skill_Q_HitBox_Disable()
    {
        skillDamage = 0;
        transform.Find("Skill_Q_HitBox").gameObject.SetActive(false);
    }

    public void Skill_W_HitBox_Enable()
    {
        skillDamage = 70 + (15 * (this.currentStatus.power / 100.0f * GetSkillCoefficient()));
        transform.Find("Skill_W_HitBox").gameObject.SetActive(true);
    }

    public void Skill_W_HitBox_Disable()
    {
        skillDamage = 0;
        transform.Find("Skill_W_HitBox").gameObject.SetActive(false);
    }
    #endregion

    #region 스킬 코루틴
    IEnumerator UseQ()
    {
        Q_Time = 6;
        Vector3 position = gameObject.transform.Find("Skill_Q_Point").transform.position;

        agent.speed = SPEED() * 1.5f;
        agent.SetDestination(position);

        while (Q_Time > 0)
        {
            //
            if (Vector3.Distance(position, gameObject.transform.position) <= 0 || (6 - Q_Time) > (4 / agent.speed))
            {
                if (agent.speed == (SPEED() * 1.5f))
                {
                    animator.SetBool("Skill_Q", false);
                    Skill_Q_HitBox_Disable();
                    agent.speed = SPEED();
                    agent.ResetPath();
                    playerController.SetMoveable(1);
                }
            }

            Q_Time -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    IEnumerator UseW()
    {
        W_Time = 5;
        agent.ResetPath();
        while (W_Time > 0)
        {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("attack_2") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
            {
                playerController.SetMoveable(1);
            }
            W_Time -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    IEnumerator UseE()
    {
        E_Time = 7;
        while (E_Time > 0)
        {
            if (7 - E_Time >= 0.5f)
            {
                evasion = false;
            }
            if(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("jump") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
            {
                playerController.SetMoveable(1);
            }
            E_Time -= Time.deltaTime;
            yield return new WaitForSecondsRealtime(Time.deltaTime);
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
        while (SlowTime <= 0)
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
        transform.Find("char_model").gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnTime);
        animator.SetBool("Dead", false);
        dead = false;
        this.currentStatus = this.status;
        transform.position = new Vector3(0, 0, 0);
        transform.Find("char_model").gameObject.SetActive(true);
    }
    #endregion

    #region 추가 스탯 관리
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

    private void AddExtraResource()
    {
        if (Extra_Resource >= 10)
        {
            return;
        }
        Extra_Resource++;
        Stat_Point--;
        this.currentStatus.power = POWER();
    }
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
        return Extra_Attack == 0 ? 1 : (1 + Extra_Attack);
    }
    private void AddExtraMoveSpeed()
    {
        if (Extra_MoveSpeed >= 10)
        {
            return;
        }
        Extra_MoveSpeed++;
        Stat_Point--;
        animator.SetFloat("Speed", SPEED());
        agent.speed = SPEED();
    }
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
        return currentStatus.speed;
    }
    #endregion

    #region 데미지 연산
    
    public override void SendDamage(GameObject target)
    {
        target.GetComponent<Character>().TakeDamage(skillDamage);
        Debug.Log("데미지 줬다.");
    }

    public override void TakeDamage(float damage)
    {
        if (evasion)
            return;

        this.currentStatus.hp -= damage;
        if (this.currentStatus.hp <= 0)
        {
            Dead();
            dead = true;
        }
        Debug.Log(this.currentStatus.hp);
    }

    public override void Damage()
    {
        if (evasion)
            return;

        this.currentStatus.hp -= skillDamage;
        if (this.currentStatus.hp <= 0)
        {
            Dead();
            dead = true;
        }
        Debug.Log(this.currentStatus.hp);
    }
    #endregion
}
