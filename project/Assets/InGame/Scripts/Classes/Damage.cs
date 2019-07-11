using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    Damage EnemyScript;
    Damage MyDamage;

    Animator animator;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        EnemyScript = collision.gameObject.GetComponent<Damage>();
        if (EnemyScript == null)
            return;
        string ani_name = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (ani_name.Contains("attack1"))
        {

        }

    }
}
