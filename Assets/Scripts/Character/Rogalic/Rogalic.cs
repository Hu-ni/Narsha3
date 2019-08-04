﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogalic : Character
 {
     ~Rogalic()
     {
         StopCoroutine(ChargeHP());
         StopCoroutine(ChargeMP());
     }

     private static readonly String Walk = "walk";
     private static readonly String Attack2 = "attack_2";
     private static readonly String Attack1 = "attack_1";
     private static readonly String Death = "death";
     
     private bool OnSkillE;

     public GameObject bullet;
     
     //캐릭터 능력치 세팅
     private void Awake()
     {
         status.hp = 10;
         status.mp = 600;
         status.power = 72;
         status.attackSpeed = 0.91f;
         status.attackRange = 3.0f;
         status.speed = 3.0f;

         currStatus = status;

         EXP();

         animator = GetComponentInChildren<Animator>();

         StartCoroutine(ChargeHP());
         StartCoroutine(ChargeMP());
     }
     

     public override void Move(bool isRunning)
     {
         animator.SetBool(Walk, isRunning);
     }

     public override void Attack(GameObject target)
     {
         animator.SetTrigger(Attack2);

         this.target = target;
     }

     public override void AttackDamage()
     {
         bullet.SetActive(true);
         bullet.transform.position = new Vector3(transform.position.x, 1, transform.position.z);
         bullet.GetComponent<Bullet>().target = target;
         bullet.GetComponent<Bullet>().damage = currStatus.power;
     }


     public override void Dead()
     {
         animator.SetTrigger(Death);
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
         animator.SetTrigger(Attack1);
         StartCoroutine(CheckAnimation(Attack1));
         
         currStatus.mp -= OnSkillE ? skillQ.mana * 1.1f : skillQ.mana;
         skillQ.GetComponent<Active>().damage = OnSkillE ? 70 + (70 * (0.2f * powerLevel)) * 1.5f : 70 + (70 * (0.2f * powerLevel));
         skillQ.ModelActive(true);

         skillQ.StartCoroutine("SpendCoolTime");
     }

     public override void Skill_W()
     {
         if (currStatus.mp <= skillW.mana || !skillW.isActive) return;
         skillW.ModelActive(false);
         animator.SetTrigger(Attack2);
         StartCoroutine(CheckAnimation(Attack2));

         currStatus.mp -= OnSkillE ? skillW.mana * 1.1f : skillW.mana;
         
             skillW.GetComponent<Active>().damage = OnSkillE ? 85 + (85 * (0.25f * powerLevel)) * 1.5f : 85 + (85 * (0.25f * powerLevel));

//         skillW.transform.position =  new Vector3(transform.position.x, 1, transform.position.z);
         skillW.ModelActive(true);

         skillW.StartCoroutine("SpendCoolTime");
     }

     public override void Skill_E()
     {
         if (currStatus.mp <= skillE.mana || !skillE.isActive) return;
         OnSkillE = !OnSkillE;
         skillE.ModelActive(OnSkillE);
         if(OnSkillE)
            skillE.StartCoroutine("SpendCoolTime");
     }

     public override IEnumerator Skill_P()
     {
         yield return null;
     }

     public override void TakeDamage(float damage)
     {
         currStatus.mp -= damage;
         if (currStatus.mp <= 0)
         {
             currStatus.hp += currStatus.mp;
             currStatus.mp = 0;
         }

         if (currStatus.hp <= 0)
         {
             currStatus.hp = 0;
             dead = true;
             
         }
     }
 }