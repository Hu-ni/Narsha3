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

    //public float skillDamage;
    
    [Header("Skill Effect")]
    public GameObject SkillQ;
    public GameObject SkillW;
    public GameObject SkillE;

    ~LizardMan()
    {
        StopCoroutine("ChargeHP");
        StopCoroutine("ChargeMP");
    }

    private void Awake()
    {
        //초기화
        status.hp = 540;
        status.mp = 0;
        status.power = 71;
        status.speed = 3;
        status.attackspeed = 0.7f;
        status.attackRange = 7f;

        currStatus = status;

        //레벨업에 필요한 경험치 설정
        EXP();

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

    #region 이동, 공격
    public override void Move(bool isRunning)
    {
        if (dead)
            return;
        
        animator.SetBool("Walk", isRunning);
    }

    public override void Attack(GameObject target)
    {
        this.target = target;
        if (attack)
        {
            return;
        }
        playerController.SetMoveable(false);
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
        playerController.SetMoveable(true);
        animator.SetBool("Attack", false);

        attack = false;
        yield return null;
    }

    public override void AttackDamage()
    {
        Debug.Log("공격이다.");
      
            if(target.transform.tag == "Champion" && target.transform.gameObject == this.target)
            {
                target.transform.gameObject.GetComponent<Character>().TakeDamage(this.currStatus.power);
            }
            else if (target.transform.tag == "Minion")
            {
                   target.GetComponent<MinionController>().TakeDamage(currStatus.power, gameObject);
           
            }
            else if (target.transform.CompareTag("Core"))
            {
                target.transform.gameObject.GetComponent<CoreScript>().TakeDamage(team);
            }
        
    }

    #endregion

    #region 스킬
    public override void Skill_Q()
    {
        Active skill = SkillQ.GetComponent<Active>();
        if (!skill.isActive)
            return;

        StartCoroutine(skill.SpendCoolTime());

        animator.SetBool("Walk", false);

        playerController.SetMoveable(false);
        transform.LookAt(agent.pathEndPosition);

        skill.ModelActive(false);
        skill.ModelActive(true);

        animator.SetBool("Skill_Q", true);
        Skill_Q_HitBox_Enable();
        StartCoroutine("UseQ");
    }

    public override void Skill_W()
    {
        Active skill = SkillW.GetComponent<Active>();
        if (!skill.isActive)
            return;

        StartCoroutine(skill.SpendCoolTime());

        animator.SetBool("Walk", false);

        playerController.SetMoveable(false);
        transform.LookAt(agent.pathEndPosition);

        skill.ModelActive(false);
        skill.ModelActive(true);

        animator.SetTrigger("Skill_W");
        animator.SetBool("Walk", false);
        StartCoroutine("UseW");
    }

    public override void Skill_E()
    {
        Active skill = SkillE.GetComponent<Active>();
        if (!skill.isActive)
            return;

        StartCoroutine(skill.SpendCoolTime());

        animator.SetBool("Walk", false);

        evasion = true;
        playerController.SetMoveable(false);
        transform.LookAt(agent.pathEndPosition);

        skill.ModelActive(false);
        skill.ModelActive(true);

        animator.SetTrigger("Skill_E");
        StartCoroutine("UseE");
    }

    #region 스킬 히트 박스 활성화, 비활성화
    public void Skill_Q_HitBox_Enable()
    {
        SkillQ.GetComponent<Active>().damage = 65 + (15 * (this.currStatus.power / 100.0f * powerLevel));
        SkillQ.transform.Find("Skill_Q_HitBox").gameObject.SetActive(true);
    }

    public void Skill_Q_HitBox_Disable()
    {
        SkillQ.transform.Find("Skill_Q_HitBox").gameObject.SetActive(false);
    }

    public void Skill_W_HitBox_Enable()
    {
        SkillW.GetComponent<Active>().damage = 70 + (15 * (this.currStatus.power / 100.0f * powerLevel));
        SkillW.transform.Find("Skill_W_HitBox").gameObject.SetActive(true);
    }

    public void Skill_W_HitBox_Disable()
    {
        SkillW.transform.Find("Skill_W_HitBox").gameObject.SetActive(false);
    }
    #endregion

    #region 스킬 코루틴

    public override IEnumerator Skill_P()
    {
        yield return null;
    }

    IEnumerator UseQ()
    {
        Vector3 position = gameObject.transform.Find("Skill_Q_Point").transform.position;

        float Q_Anim_Time = 0;

        this.currStatus.speed = SPEED() * 1.5f;
        agent.SetDestination(position);

        while (!SkillQ.GetComponent<Active>().isActive)
        {
            //Debug.Log(Vector3.Distance(position, gameObject.transform.position) + "\n" + (Q_Anim_Time > (4 / agent.speed)));
            if (Vector3.Distance(position, gameObject.transform.position) <= 0 || Q_Anim_Time > (4 / agent.speed))
            {
                if (Mathf.Approximately(agent.speed, (SPEED() * 1.5f)))
                {
                    animator.SetBool("Skill_Q", false);
                    Skill_Q_HitBox_Disable();
                    this.currStatus.speed = SPEED();
                    agent.ResetPath();
                    playerController.SetMoveable(true);
                }
            }
            else
            {
                Q_Anim_Time += Time.deltaTime;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator UseW()
    {
        agent.ResetPath();
        while (!SkillW.GetComponent<Active>().isActive)
        {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("attack_2") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
            {
                playerController.SetMoveable(true);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator UseE()
    {
        SkillE.GetComponent<Active>().cooltime = 7;
        float evasionTime = 0;
        while (!SkillE.GetComponent<Active>().isActive)
        {
            if (7 - evasionTime >= 0.5f)
            {
                evasion = false;
            }
            else
            {
                evasionTime += Time.deltaTime;
            }
            if(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("jump") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
            {
                playerController.SetMoveable(true);
            }
            yield return new WaitForFixedUpdate();
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

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);
        transform.Find("char_model").gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnTime);
        animator.SetBool("Dead", false);
        dead = false;
        this.currStatus = this.status;
        transform.position = new Vector3(0, 0, 0);
        transform.Find("char_model").gameObject.SetActive(true);
    }
    #endregion

    #region 스탯 관련

    public override float POWER()
    {
        currStatus.power = status.power + ((Extra_Attack + Extra_Resource) * status.power * 0.2f);
        return currStatus.power;
    }
    #endregion

    #region 데미지 연산

    public override void TakeDamage(float damage)
    {
        if (evasion)
            return;

        this.currStatus.hp -= damage;
        if (this.currStatus.hp <= 0)
        {
            Dead();
            dead = true;
        }
        Debug.Log(this.currStatus.hp);
    }
    #endregion
}
