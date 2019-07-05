using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Character : MonoBehaviour
{
        //캐릭터 원래 스탯(아무것도 적용되지 않은 스탯)
        public Status status;
        //사용될 스탯(현재 스탯)
        public Status currentStatus;

        public int powerLevel;

        public abstract void Set();
        public abstract void Attack();
        public abstract void Damage();
        public abstract void Move(bool isRunning);
        public abstract void Dead();
        public abstract void Skill_Q();
        public abstract void Skill_W();
        public abstract void Skill_E();

        public abstract void SendDamage(GameObject target);
        public abstract void TakeDamage(float dmg);
}
