using System;
using System.Collections;
using UnityEngine;

public class Knight : Character
{
    ~Knight()
    {
        StopCoroutine(Skill_P());
        StopCoroutine(ChargeHP());
        StopCoroutine(ChargeMP());
    }

    private Quaternion currRotate;

    private readonly string Run = "run";
    private readonly string Attack1 = "attack_1";
    private readonly string Attack2 = "attack_2";
    private readonly string Attack3 = "attack_3";
    private readonly string Death = "death";

    private float EnchancePower;
    private bool isEnchance = false;
    private bool isMove = false;

    private void Awake()
    {
        status.hp = 650;
        status.mp = 170;
        status.power = 77;
        status.attackspeed = 0.81f;
        status.attackRange = 8;
        status.speed = 3.3f;
        
        EXP();

        currStatus = status;

        animator = GetComponentInChildren<Animator>();

        StartCoroutine(Skill_P());
        StartCoroutine(ChargeHP());
        StartCoroutine(ChargeMP());
    }

    private void Update()
    {
        Skill_P();
    }

    public override void Move(bool isRunning)
    {
        isMove = isRunning;
        animator.SetBool(Run, isRunning);
    }

    public override void Attack(GameObject target)
    {
        animator.SetTrigger(Attack3);

        this.target = target;
        AttackDamage();
    }

    public override void AttackDamage()
    {
        if (target.CompareTag("Champion"))
        {
            target.GetComponent<Character>().TakeDamage(currStatus.power);
        }
        else if (target.CompareTag("Core"))
        {
            target.GetComponent<CoreScript>().TakeDamage(team);
        }
        
    }

    public override void Dead()
    {
        animator.SetTrigger(Death);
        gameObject.SetActive(false);
        Invoke("ReSpawn", 10f);
        Debug.Log(gameObject.name + "은(는) 10초뒤에 리스폰됩니다.");

    }

    

    IEnumerator CheckAnimation(String name)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(name))
        {
            yield return null;
        }

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f)
        {
            yield return null;
        }
    }

    
    public override void Skill_Q()
    {
        if (currStatus.mp <= skillQ.mana || !skillQ.isActive) return;
        skillQ.ModelActive(false);
        skillQ.ModelActive(true);

        skillQ.GetComponent<Active>().damage =
            isEnchance ? (70 + (1.15f * powerLevel)) + EnchancePower : (70 + (1.15f * powerLevel));
        currStatus.mp -= skillQ.mana;
        
        animator.SetTrigger(Attack1);
        skillQ.StartCoroutine("SpendCoolTime");
    }

    public override void Skill_W()
    {
        if (currStatus.mp <= skillW.mana || !skillW.isActive) return;
        skillW.ModelActive(false);
        skillW.ModelActive(true);

        skillW.GetComponent<Active>().damage =
            isEnchance ? (60 + (1.2f * powerLevel)) + EnchancePower : (60 + (1.2f * powerLevel));
        currStatus.mp -= skillW.mana;
        
        animator.SetTrigger(Attack2);
        skillW.StartCoroutine("SpendCoolTime");
    }

    public override void Skill_E()
    {
        if (currStatus.mp <= skillE.mana || !skillE.isActive) return;

        currStatus.mp -= skillE.mana;
        skillE.ModelActive(true);
        StartCoroutine(E_Passive());
        StartCoroutine(skillE.SpendCoolTime());
        skillE.GetComponent<Buff>().StartCoroutine("Effect");
    }

    public override IEnumerator Skill_P()
    {
        while (!dead)
        {
            if (currRotate == transform.rotation && isMove)
            {
                if (status.speed * 1.3f > currStatus.speed)
                {
                    currStatus.speed += currStatus.speed * (Time.deltaTime * 15f);
                    if(status.speed * 1.3f < currStatus.speed)
                        currStatus.speed = status.speed * 1.3f;

                }
            }
            else
            {
                currRotate = transform.rotation;
                currStatus.speed = status.speed;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator E_Passive()
    {
        isEnchance = true;
        status.speed *= 1.4f;
        while (skillE.GetComponent<Buff>().isEffect)
        {
            EnchancePower = 40 * currStatus.speed; 
            yield return new WaitForFixedUpdate();
        }

        status.speed = 3.3f;
    }
    
    public override void TakeDamage(float damage)
    {
        Debug.Log("기사 쳐맞음");
        currStatus.hp -= damage;
        if (currStatus.hp <= 0)
        {
            currStatus.hp = 0;
            Dead();
        }
    }
}
