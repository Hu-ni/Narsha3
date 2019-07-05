//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
////using CustomMethodSet.conviMethods;

//public class SwordMan : Character
//{
//    private float anger_MAX = 100;
//    private float anger = 0;



//    private Animator animator;
//    PlayerController playerController;
//    private bool passiveActivated;


//    private float skillDamage;

//    override
//    public void Set()
//    {

//        //List<Dictionary<string, object>> data = CSVReader.Read("CharactorInfo");

//        //status.hp = Convert.ToInt32(data[1]["HP"]);
//        //status.mp = Convert.ToInt32(data[1]["MP"]);
//        //status.power = Convert.ToInt32(data[1]["POWER"]);
//        //status.speed = Convert.ToInt32(data[1]["SPEED"]);
//        //status.attackSpeed = 0.75f;

//        currentStatus = status;
//        Debug.Log("------Status of SwordMan------");
//        Debug.Log("HP>>    " + status.hp);
//        Debug.Log("MP>>    " + status.mp);
//        Debug.Log("POWER>> " + status.power);
//        Debug.Log("SPEED>> " + status.speed);
//    }

//    private void Awake()
//    {
//        animator = this.GetComponent<Animator>();

//    }
//    private void Start()
//    {
//        passiveActivated = false;
//        Set();
//        playerController = gameObject.GetComponent<PlayerController>();

//    }

//    override
//    public void Attack()
//    {
//        Debug.Log("SwordMan: Attack");
//        animator.SetTrigger("Attack");

//    }



//    override
//    public void Damage()
//    {
//        Debug.Log("SwordMan: Ouch!");
//    }

//    override
//    public void Move()
//    {
//        Debug.Log("SwordMan: Move!");
//    }



//    override
//    public void Skill_Q()
//    {


//        Debug.Log("SowrdMan: Skill_Q");
//    }

//    override
//    public void Skill_W()
//    {
//        Debug.Log("SowrdMan: Skill_W");
//        skillDamage = 70 + (20 * status.power);
//        animator.SetTrigger("Skill1");

//    }

//    override
//    public void Skill_E()
//    {
//        Debug.Log("SowrdMan: Skill_E");

//    }

    

//    IEnumerator PassiveActivate()
//    {
//        //Debug.Log("Passive Activate");
//        //passiveActivated = true;
//        //float original_Power = status.power;
//        //float original_AtkSpeed = status.attackSpeed;

//        //status.power = status.power * (1.0f + 30.0f / 100);
//        //status.attackSpeed = status.attackSpeed * (1.0f + 30.0f / 100);
//        //Debug.Log("POWER + 30%    >> " + status.power);
//        //Debug.Log("atkSpeed + 30% >> " + status.attackSpeed);

//        //yield return new WaitForSeconds(10f);

//        //status.power = original_Power;
//        //status.attackSpeed = original_AtkSpeed;
//        //passiveActivated = false;
//        //Debug.Log("after POWER    >> " + status.power);
//        //Debug.Log("after atkSpeed >> " + status.attackSpeed);
//        yield return null;

//    }

//    private void Update()
//    {
//        AnimeProccess();
//    }
//    public void AnimeProccess()
//    {
//        animator.SetBool("Walk", playerController.isRunning);
//    }

//    public override void SendDamage(GameObject target)
//    {
//        anger += 25;
//        //분노가 100이 되면 30%증가 패시브를 발동시킨다.
//        if (anger >= 100)
//        {

//            anger = 100;
//            StartCoroutine(PassiveActivate());
//        }
//        target.GetComponent<Character>().TakeDamage(skillDamage);
//        Debug.Log("데미지 줬다.");
//    }

//    public override void TakeDamage(float dmg)
//    {
//        currentStatus.hp -= dmg;
//    }

//    public override void Dead()
//    {
//    }
//}