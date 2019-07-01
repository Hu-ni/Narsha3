
using System;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public Character character;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            character.GetComponent<Character>().SendDamage(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            character.GetComponent<Character>().SendDamage(other.gameObject);
        }
    }
}
