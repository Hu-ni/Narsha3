using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public abstract class Character : MonoBehaviour
{
    //팀 구별
    [SerializeField]
    public int team;
    //캐릭터 원래 스탯(아무것도 적용되지 않은 스탯)
    public Status status;
    //사용될 스탯(현재 스탯)
    public Status currStatus;
    //애니메이터
    public Animator animator;
    //스킬계수
    public int powerLevel=1;

    [Header("- Status")]
    //상태 변수들
    public bool dead = false;
    public bool evasion = false;
    public bool attack = false;
    public bool stun = false;

    public GameObject target;

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
    public int exp_need;
    
    public abstract void Attack(GameObject target);
    public abstract void AttackDamage();
    public abstract void Move(bool isRunning);
    public abstract void Dead();
    public abstract void Skill_Q();
    public abstract void Skill_W();
    public abstract void Skill_E();
    public abstract IEnumerator Skill_P();

    ////스킬 이펙트 저장
    //public GameObject SkillQ;
    //public GameObject SkillW;
    //public GameObject SkillE;

    public Skill skillQ;
    public Skill skillW;
    public Skill skillE;

  
    #region 추가 스탯 관리
    public void AddExtraHP()
    {
        if (Extra_HP >= 10)
        {
            return;
        }
        Extra_HP++;
        Stat_Point--;
        this.status.hp *= 1.1f;
    }

    public void AddExtraResource()
    {
        if (Extra_Resource >= 10)
        {
            return;
        }
        Extra_Resource++;
        Stat_Point--;
        this.currStatus.power = POWER();
    }

    public void AddExtraAttack()
    {
        if (Extra_Attack >= 10)
        {
            return;
        }
        Extra_Attack++;
        powerLevel++;
        Stat_Point--;
        this.currStatus.power = POWER();
    }

    private void AddExtraMoveSpeed(Animator animator, NavMeshAgent agent)
    {
        if (Extra_MoveSpeed >= 10)
        {
            return;
        }
        Extra_MoveSpeed++;
        Stat_Point--;
        animator.SetFloat("Speed", SPEED());
        agent.speed = SPEED();
        this.currStatus.speed = SPEED();
    }

    private void AddExtraAttackSpeed(Animator animator)
    {
        if (Extra_AttackSpeed >= 10)
        {
            return;
        }
        Extra_AttackSpeed++;
        Stat_Point--;
        animator.SetFloat("AttackSpeed", ATTACKSPEED());
        this.currStatus.attackspeed = ATTACKSPEED();
    }
    #endregion

    #region 스탯 관련
    //체력 및 마나 재생
    public IEnumerator ChargeHP()
    {
        while (true)
        {
            if (!dead && !Mathf.Approximately(this.currStatus.hp, this.status.hp))
            {
                this.currStatus.hp += (4 * Extra_HP);
                yield return new WaitForSeconds(1);
            }
            else
            {
                yield return null;
            }
        }
    }

    public IEnumerator ChargeMP()
    {
        while (true)
        {
            if (!dead && !Mathf.Approximately(this.currStatus.mp, this.status.mp))
            {
                this.currStatus.mp += (4 * Extra_Resource);
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
        exp_need = (100 + (this.currStatus.level - 1) * 75);
        return exp_need;
    }

    public void GetEXP(int EXP)
    {
        //레벨 제한 20
        if (this.currStatus.level == 20)
        {
            Debug.Log("최대 레벨이라 더 이상 경험치를 획득할 수 없습니다.");
            return;
        }

        this.currStatus.exp += EXP;
        while (this.currStatus.exp >= exp_need)
        {
            this.currStatus.exp -= exp_need;
            LevelUp();
            this.EXP();
        }
    }
    public void ReSpawn()
    {
        currStatus.hp = status.hp;
        gameObject.SetActive(true);

    }

    public void LevelUp()
    {
        this.currStatus.level++;
        this.Stat_Point += 2;
    }

    public virtual float POWER()
    {
        currStatus.power = status.power + (Extra_Attack * status.power * 0.2f);
        return currStatus.power;
    }

    public void UseMP(int MP)
    {
        this.currStatus.mp -= MP;
    }

    public float ATTACKSPEED()
    {
        currStatus.attackspeed = status.attackspeed + (Extra_AttackSpeed * status.attackspeed * 0.15f);
        return currStatus.attackspeed;
    }

    public float SPEED()
    {
        currStatus.speed = status.speed + (Extra_MoveSpeed * this.status.speed * 0.05f);
        return currStatus.speed;
    }
    #endregion

    #region CC기(스턴, 슬로우)
    public void Slow(float SlowTime, float SlowValue)
    {
        StartCoroutine(crtSlow(GetComponent<NavMeshAgent>(), SlowTime, SlowValue));
    }

    IEnumerator crtSlow(NavMeshAgent agent, float SlowTime, float SlowValue)
    {
        Debug.Log("Slow");
        agent.speed *= (1f - (0.01f * SlowValue));
        while (SlowTime >= 0)
        {
            SlowTime -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        agent.speed = SPEED();
    }

    public void Stun(float StunTime)
    {
        StartCoroutine(crtStun(GetComponent<Animator>(), StunTime));
    }

    IEnumerator crtStun(Animator animator, float StunTime)
    {
        stun = true;
        GetComponent<PlayerController>().SetMoveable(false);
        animator.SetBool("Walk", false);
        yield return new WaitForSeconds(StunTime);
        GetComponent<PlayerController>().SetMoveable(true);
        stun = false;
    }
    #endregion
    
    public abstract void TakeDamage(float dmg);
}
