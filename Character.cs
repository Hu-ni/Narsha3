using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * It's declare abstract Method which is based on each Characters
 */


public abstract class Character : MonoBehaviour
{
    public Animator myAnim;

    [SerializeField]
    public Status status;
    public Status currStatus;

    public int powerLevel;
    
    public abstract void Set();
    
    public abstract void Move(bool isRunning);
    public abstract void Attack();
    public abstract void Damage();

    public abstract void Dead();

    public abstract void Skill_Q();
    public abstract void Skill_W();
    public abstract void Skill_E();
    public abstract void Skill_P();
    
    public abstract void SendDamage(GameObject target);
    public abstract void TakeDamage(float damage);



}
