using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int team;
    public GameObject target;
    public float damage;
   
    private void Update()
    {
        Debug.Log("총알");
        transform.LookAt(new Vector3(target.transform.position.x, 1, target.transform.position.z));
        transform.Translate(Vector3.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Champion")&&other.gameObject.GetComponent<Character>().team!=this.team)
        {
            other.gameObject.GetComponent<Character>().TakeDamage(damage);
        }
        if (other.gameObject.CompareTag("Core"))
        {
            other.gameObject.GetComponent<CoreScript>().TakeDamage(this.team);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Champion") && other.gameObject.GetComponent<Character>().team != this.team)
        {
            other.gameObject.GetComponent<Character>().TakeDamage(damage);
        }
        if (other.gameObject.CompareTag("Core"))
        {
            other.gameObject.GetComponent<CoreScript>().TakeDamage(this.team);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.gameObject.CompareTag("Champion") && !other.gameObject.Equals(gameObject && other.gameObject.GetComponent<Character>().team != this.team));
        if (other.gameObject.CompareTag("Champion") && !other.gameObject.Equals(gameObject) && other.gameObject.GetComponent<Character>().team != this.team)
        {
            Debug.Log("충돌" + other.gameObject.name);
            other.gameObject.GetComponent<Character>().TakeDamage(damage);
            gameObject.SetActive(false);
        }
        if (other.gameObject.CompareTag("Core"))
        {
            other.gameObject.GetComponent<CoreScript>().TakeDamage(this.team);
        }
    }
}
