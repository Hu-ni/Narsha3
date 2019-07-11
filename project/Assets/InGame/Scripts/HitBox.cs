using System;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    //public Character character;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Enemy"))
    //    {
    //        other.gameObject.GetComponent<Character>().TakeDamage(gameObject.GetComponentInParent<Character>().skillDamage);
    //    }
    //}

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.CompareTag("Enemy"))
    //    {
    //        other.gameObject.GetComponent<Character>().TakeDamage(gameObject.GetComponentInParent<Character>().skillDamage);
    //    }
    //}
    public Active skill;

    private void Awake()
    {
        skill = GetComponentInParent<Active>();
    }

    private void OnTriggerEnter(Collider other)
    {
        DamageProccess(other.gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        DamageProccess(other.gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(gameObject.name);
        DamageProccess(other);
        
    }

    public void DamageProccess(GameObject other)
    {
        if (other.gameObject.CompareTag("Champion"))
        {

            other.gameObject.GetComponent<Character>().TakeDamage(skill.damage);
        }
        else if (other.gameObject.CompareTag("Minion"))
        {
            other.gameObject.GetComponent<MinionController>().TakeDamage(skill.damage);
        }
    }

    
}