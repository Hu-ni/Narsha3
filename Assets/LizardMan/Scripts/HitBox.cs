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
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Damage!");
            other.gameObject.GetComponent<Character>().TakeDamage(skill.damage);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Character>().TakeDamage(skill.damage);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(gameObject.name);
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Character>().TakeDamage(skill.damage);
        }
    }
}