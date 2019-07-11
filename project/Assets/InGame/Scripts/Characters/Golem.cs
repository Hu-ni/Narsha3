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
 
    private PlayerController playerController;
    
    [Header("Skill Effect")]
    //스킬 이펙트[Character에 들어감...]
    
    public bool skillq = false;
    
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
        status.hp = 630;
        status.mp = 200;
        status.power = 83;
        status.speed = 2.8f;
        status.attackspeed = 0.67f;
        status.attackRange = 10f;

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
        StartCoroutine("Skill_P");
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
        while (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("idle") || animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("walk"))
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
        float damage;
        if (skillq)
            damage = 80 + (this.currStatus.hp * 0.06f * (Extra_Attack == 0 ? 1 : 1+Extra_Attack)) + (5 * (this.currStatus.power / 100.0f * (Extra_Attack == 0 ? 1 : 1+Extra_Attack)));
        else
            damage = this.currStatus.power;
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, GetComponent<CapsuleCollider>().bounds.size / 2, transform.forward, out hit, transform.rotation, this.currStatus.attackRange))
        {
            if (hit.transform.tag == "Champion")
            {
                Debug.Log("고올렘");
                if(hit.transform.gameObject == this.target)
                {
                    Debug.Log("골렘데미지주기");
                    target.transform.gameObject.GetComponent<Character>().TakeDamage(damage);
                }
                if (skillq)
                {
                    skillq = false;
                    //둔화
                    hit.transform.GetComponent<Character>().Slow(2, 35); //시간, 퍼센트
                    hit.transform.gameObject.GetComponent<Character>().TakeDamage(damage);
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
        Active skill = skillQ.GetComponent<Active>();
        if (!skill.isActive || this.currStatus.mp < skill.mana)
            return;

        StartCoroutine(skill.SpendCoolTime());

        animator.SetBool("Walk", false);

        playerController.SetMoveable(false);
        transform.LookAt(agent.pathEndPosition);

        skill.ModelActive(false);
        skill.ModelActive(true);

        skillq = true;
        AddPassiveCheck = false;

        UseMP(25);
        animator.SetTrigger("Skill_Q");
        StartCoroutine("UseQ");
    }

    public override void Skill_W()
    {
        Active skill = skillW.GetComponent<Active>();
        if (!skill.isActive || this.currStatus.mp < skill.mana)
            return;

        StartCoroutine(skill.SpendCoolTime());

        animator.SetBool("Walk", false);

        playerController.SetMoveable(false);
        transform.LookAt(agent.pathEndPosition);

        skill.ModelActive(false);
        skill.ModelActive(true);
        
        AddPassiveCheck = false;
        
        UseMP((int)skill.mana);
        animator.SetTrigger("Skill_W");
        StartCoroutine("UseW");
    }

    public override void Skill_E()
    {
        Active skill = skillE.GetComponent<Active>();
        if (!skill.isActive || this.currStatus.mp < skill.mana)
            return;

        StartCoroutine(skill.SpendCoolTime());

        playerController.SetMoveable(false);
        transform.LookAt(agent.pathEndPosition);

        skill.ModelActive(false);
        skill.ModelActive(true);

        AddPassiveCheck = false;
        
        UseMP((int)skill.mana);
        animator.SetTrigger("Skill_E");

        StartCoroutine("UseE");
    }

    #region 스킬 히트 박스 활성화, 비활성화
    public void Skill_W_HitBox_Enable()
    {
        skillW.GetComponent<Active>().damage = 90 + (this.status.hp * 0.045f * powerLevel) + (15 * (this.currStatus.power / 100.0f * powerLevel));
        skillW.transform.Find("Skill_W_HitBox").gameObject.SetActive(true);
    }

    public void Skill_W_HitBox_Disable()
    {
        skillW.transform.Find("Skill_W_HitBox").gameObject.SetActive(false);
    }
    #endregion

    #region 스킬 코루틴
    public override IEnumerator Skill_P()
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

    IEnumerator UseQ()
    {
        agent.ResetPath();

        while (!skillQ.GetComponent<Active>().isActive)
        {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("attack_1") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
            {
                playerController.SetMoveable(true);
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    IEnumerator UseW()
    {
        agent.ResetPath();
        
        while (!skillW.GetComponent<Active>().isActive)
        {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("attack_2") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f)
            {
                playerController.SetMoveable(true);
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    IEnumerator UseE()
    {
        Vector3 position = gameObject.transform.Find("Skill_E_Point").transform.position;

        this.currStatus.speed = 11;
        agent.SetDestination(position);
        
        while (!skillE.GetComponent<Active>().isActive)
        {
            if (Vector3.Distance(position, gameObject.transform.position) <= agent.stoppingDistance)
            {
                if (Mathf.Approximately(this.currStatus.speed, 11) && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("jump") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.97f)
                {
                    float damage;
                    Collider[] colls = Physics.OverlapSphere(transform.position, 2);
                    foreach (Collider coll in colls)
                    {
                        if (coll.gameObject.tag == "Enemy" && !coll.gameObject.Equals(gameObject))
                        {
                            coll.transform.gameObject.GetComponent<Character>().Stun(3);
                            damage = 75 + (this.status.hp * 0.1f * powerLevel) + (10 * (this.currStatus.power / 100.0f * powerLevel));
                            coll.gameObject.GetComponent<Character>().TakeDamage(damage);
                        }
                    }
                    animator.SetBool("Skill_E", false);
                    animator.SetBool("Walk", false);
                    this.currStatus.speed = SPEED();
                    agent.ResetPath();
                    playerController.SetMoveable(true);
                }
            }
            
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
    
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);
        transform.Find("Golem").gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnTime);
        animator.SetBool("Dead", false);
        dead = false;
        this.currStatus = this.status;
        transform.position = new Vector3(0, 0, 0);
        transform.Find("Golem").gameObject.SetActive(true);
    }
    #endregion
    
    
    #region 데미지 연산
    public override void TakeDamage(float damage)
    {
        if (evasion)
            return;
        
        float DefenseDamage = damage * (0.9f - (Passive_ExtraDefense * 0.05f));
        this.currStatus.hp -= DefenseDamage;
        if (this.currStatus.hp <= 0)
        {
            Dead();
            dead = true;
        }
        Debug.Log(this.currStatus.hp);
    }

    #endregion

  
}