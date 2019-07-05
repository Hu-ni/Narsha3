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

    private bool isAttack;

    private void Start()
    {
        target = transform.position;
        Moveable = true;
        character = GetComponent<Character>();
        attackRange = character.currentStatus.attackRange;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        if (!Moveable)
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit))
            {
                GetComponent<Character>().Move(true);
                target = hit.point;
                navMeshAgent.SetDestination(target);
                if (hit.transform.gameObject.CompareTag("Enemy") || hit.transform.gameObject.CompareTag("Minion"))
                {
                    if (Vector3.Distance(transform.position, hit.point) <= attackRange)
                    {
                        Attack();
                    }
                    else
                    {
                        isAttack = true;
                    }
                }
            }
        }

        if (Vector3.Distance(transform.position, target) < attackRange && isAttack)
        {
            Attack();
        }else if (Vector3.Distance(transform.position, target) <= navMeshAgent.stoppingDistance + 0.1f)
        {
            GetComponent<Character>().Move(false);
        }

        Inputs();
    }

    private void Attack()
    {
        GetComponent<Character>().Attack();
        navMeshAgent.SetDestination(transform.position);
        transform.LookAt(target);
        GetComponent<Character>().Move(false);
        isAttack = false;
    }

    public void SetMoveable(int mode)
    {
        navMeshAgent.destination = transform.position;
        if (mode == 1)
        {
            Moveable = true;
        }
        else
        {
            Moveable = false;
        }
    }

    private void Inputs()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            character.Skill_Q();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            character.Skill_W();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            character.Skill_E();
        }
    }
}