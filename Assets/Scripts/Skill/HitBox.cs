
using System;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public Active skill;

    private void Awake()
    {
        skill = GetComponentInParent<Active>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
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
        Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log(other.gameObject.name);
            other.gameObject.GetComponent<Character>().TakeDamage(skill.damage);
        }
    }
}
