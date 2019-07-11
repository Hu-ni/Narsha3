using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject target;
    public float damage;
    private void Update()
    {
        transform.LookAt(new Vector3(target.transform.position.x, 1, target.transform.position.z));
        transform.Translate(Vector3.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Character>().TakeDamage(damage);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Character>().TakeDamage(damage);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.gameObject.CompareTag("Enemy") && !other.gameObject.Equals(gameObject));
        if (other.gameObject.CompareTag("Enemy") && !other.gameObject.Equals(gameObject))
        {
            Debug.Log("충돌" + other.gameObject.name);
            other.gameObject.GetComponent<Character>().TakeDamage(damage);
            gameObject.SetActive(false);
        }
    }
}
