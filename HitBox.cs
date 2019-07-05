using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    Character character;
    // Start is called before the first frame update
    void Start()
    {
        character = gameObject.GetComponentInParent<Character>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("적이 들어왔다.");
             character.SendDamage(other.gameObject);
        }
   
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("적이 들어왔다.");
            character.SendDamage(collision.gameObject);
        }

    }
}
