using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Character
{
    //길 찾기 AI
    private UnityEngine.AI.NavMeshAgent agent;

    //리스폰 시간
    private float respawnTime = 3;

    //애니메이터
    private Animator animator;
    private PlayerController playerController;

    private float skillDamage;

    [Header("Skill Time")]
    //스킬 쿨타임
    public float Q_Time = 0, W_Time = 0, E_Time = 0;

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
    public float Extra_HP = 0;
    public float Extra_Resource = 0;
    public float Extra_Attack = 0;
    public float Extra_MoveSpeed = 0;
    public float Extra_AttackSpeed = 0;

    //경험치 필요량
    private int exp_need;

    //패시브 초기화 시간, 패시브 추가 방어력
    private float Passive_Time = 0;
    private int Passive_ExtraDefense = 0;

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
        animator.SetBool("Attack", false);
        attack = false;
        yield return null;
    }

    #endregion

    #region 스킬
    public override void Skill_Q()
    {
    }

    public override void Skill_W()
    {
        if (W_Time > 0)
            return;

        playerController.SetMoveable(0);
        transform.LookAt(agent.pathEndPosition);

        animator.SetTrigger("Skill_W");
        StartCoroutine("UseW");
    }

    public override void Skill_E()
    {
    }

    #region 스킬 히트 박스 활성화, 비활성화
    public void Skill_Q_HitBox_Enable()
    {
        float Attack = currentStatus.power;
        skillDamage = 65 + (Attack * ((15 * Attack) / 100.0f));
        Debug.Log(skillDamage);
        transform.Find("Skill_Q_HitBox").gameObject.SetActive(true);
    }

    public void Skill_Q_HitBox_Disable()
    {
        skillDamage = 0;
        transform.Find("Skill_Q_HitBox").gameObject.SetActive(false);
    }

    public void Skill_W_HitBox_Enable()
    {
        float Attack = currentStatus.power;
        skillDamage = 70 + (Attack * ((15 * Attack) / 100.0f));
        transform.Find("Skill_W_HitBox").gameObject.SetActive(true);
    }

    public void Skill_W_HitBox_Disable()
    {
        skillDamage = 0;
        transform.Find("Skill_W_HitBox").gameObject.SetActive(false);
    }
    #endregion

    #region 스킬 코루틴
    IEnumerator UseW()
    {
        W_Time = 5;
        agent.ResetPath();

        while (W_Time > 0)
        {
            Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            Debug.Log(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("attack_2") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
            {
                playerController.SetMoveable(1);
            }
            W_Time -= Time.deltaTime;
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

    public void Stun(float StunTime)
    {
        animator.SetFloat("StunTime", StunTime);
        StartCoroutine("Stun");
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
    private void AddExtraHP()
    {
        if (Extra_HP >= 10)
        {
            return;
        }
        Extra_HP++;
        this.status.hp *= 1.1f;
    }

    private void AddExtraResource()
    {
        if (Extra_Resource >= 10)
        {
            return;
        }
        Extra_Resource++;
    }
    private void AddExtraAttack()
    {
        if (Extra_Attack >= 10)
        {
            return;
        }
        Extra_Attack++;
    }
    private void AddExtraMoveSpeed()
    {
        if (Extra_MoveSpeed >= 10)
        {
            return;
        }
        Extra_MoveSpeed++;
    }
    private void AddExtraAttackSpeed()
    {
        if (Extra_AttackSpeed >= 10)
        {
            return;
        }
        Extra_AttackSpeed++;
    }
    #endregion

    #region 스탯 관련
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

        Debug.Log(damage);

        float DefenseDamage = damage * (0.9f - (Passive_ExtraDefense * 0.05f));
        Debug.Log(DefenseDamage);
        this.currentStatus.hp -= DefenseDamage;
        Debug.Log(this.currentStatus.hp);
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