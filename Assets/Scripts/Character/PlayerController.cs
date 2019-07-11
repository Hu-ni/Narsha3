using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private float attackRange;
    Character character;

    [SerializeField]
    private bool Moveable;

    private Vector3 target;
    private GameObject hited;

    private bool isAttack;
    private void Start() {
        Moveable = true;
        character = GetComponent<Character>();
        attackRange = character.currStatus.attackRange;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    private void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        navMeshAgent.speed = character.currStatus.speed;
        
        if (Input.GetMouseButtonDown(1) && Moveable) {
            if(Physics.Raycast(ray,out hit)) {
                target = hit.point;
                character.Move(true);
                navMeshAgent.SetDestination(hit.point);
                if (hit.transform.gameObject.CompareTag("Enemy") || hit.transform.gameObject.CompareTag("Minion"))
                {
                    if (Vector3.Distance(transform.position, hit.point) < attackRange)
                    {
                        Attack(hit.transform.gameObject);
                    }
                    else
                    {
                        hited = hit.transform.gameObject;
                        isAttack = true;
                    }
                }
            }
        }

        if (Vector3.Distance(transform.position, target) < attackRange * 3 && isAttack)
        {
            Attack(hited);
        }
        else if (Vector3.Distance(transform.position, target) <= navMeshAgent.stoppingDistance)
        {
            character.Move(false);
        }

        Inputs();
    }
    
    private void Attack(GameObject obj)
    {
        character.Attack(obj);
        character.AttackDamage();
        navMeshAgent.SetDestination(transform.position);
        transform.LookAt(target);
        character.Move(false);
        isAttack = false;
    }

    public void SetMoveable(bool isMove)
    {
        navMeshAgent.destination = transform.position;
        Moveable= isMove;
    }

    private void Inputs()
    {
        if (Input.GetKeyDown(KeyCode.Q) && Moveable)
        {
            character.Skill_Q();
        }

        if (Input.GetKeyDown(KeyCode.W) && Moveable)
        {
            character.Skill_W();
        }

        if (Input.GetKeyDown(KeyCode.E) && Moveable)
        {
            character.Skill_E();
        }
    }
}
