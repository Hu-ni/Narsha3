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

    GameObject hited = null;

    private void Start()
    {
        Moveable = true;
        character = GetComponent<Character>();
        attackRange = character.currStatus.attackRange;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(1) && Moveable)
        {
            if (Physics.Raycast(ray, out hit))
            {
                GetComponent<Character>().Move(true);
                navMeshAgent.SetDestination(hit.point);
                target = hit.point;
                if (hit.transform.gameObject != gameObject && (hit.transform.gameObject.CompareTag("Enemy") || hit.transform.gameObject.CompareTag("Minion")))
                {
                    if (Vector3.Distance(transform.position, hit.point) <= attackRange)
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

        if (Vector3.Distance(transform.position, target) < attackRange && isAttack)
        {
            Attack(hited);
        }else if (Vector3.Distance(transform.position, target) <= navMeshAgent.stoppingDistance)
        {
            GetComponent<Character>().Move(false);
        }

        Inputs();
    }

    private void Attack(GameObject obj)
    {
        GetComponent<Character>().Attack(obj);
        navMeshAgent.SetDestination(transform.position);
        transform.LookAt(target);
        GetComponent<Character>().Move(false);
        isAttack = false;
    }

    public void SetMoveable(bool isMove)
    {
        navMeshAgent.destination = transform.position;
        Moveable = isMove;
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