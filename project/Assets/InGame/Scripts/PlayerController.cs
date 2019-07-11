using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private float attackRange;
    Character character;

    [SerializeField]
    private bool Moveable;

    private Vector3 target;

    private bool isAttack;
    int ownTeam;

    GameObject hited = null;

    private void Start()
    {
        Moveable = true;
        character = GetComponent<Character>();
        attackRange = character.currStatus.attackRange;
        navMeshAgent = GetComponent<NavMeshAgent>();
        ownTeam = GetComponent<Character>().team;

        if (photonView.IsMine)
        {
            Camera.main.GetComponent<Transform>().position = this.transform.position + new Vector3(0, 20, 0);
        }
    }
    private void Update()
    {
        if (Vector3.Distance(transform.position, target) < attackRange && isAttack)
        {
            Attack(hited);
        }
        else if (Vector3.Distance(transform.position, target) <= navMeshAgent.stoppingDistance)
        {
            GetComponent<Character>().Move(false);
        }

        navMeshAgent.speed = this.character.currStatus.speed;


        if (!this.photonView.IsMine)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                int objId;
                try
                {
                    objId = hit.transform.gameObject.GetPhotonView().ViewID;
                }
                catch
                {
                    objId = -1;
                }
                this.photonView.RPC("Move", RpcTarget.All, hit.point, objId);
            }
        }

        Inputs();
    }

    // 이동
    [PunRPC]
    public void Move(Vector3 hitPoint, int hitViewId)
    {
        GameObject hit;
        try
        {
            hit = PhotonView.Find(hitViewId).gameObject;
        }
        catch
        {
            Debug.Log("Move() catch");
            return;
        }

        target = hitPoint;
        GetComponent<Character>().Move(true);

        if (hit.transform.gameObject != gameObject && (hit.transform.gameObject.CompareTag("Champion") || hit.transform.gameObject.CompareTag("Minion") || hit.transform.gameObject.CompareTag("Core")))
        {
            if (hit.transform.gameObject.CompareTag("Champion"))
            {
                if (hit.transform.gameObject.GetComponent<Character>().team != ownTeam)
                {
                    Debug.Log("적이다!");
                    if (Vector3.Distance(transform.position, hit.transform.position) <= attackRange)
                    {
                        Debug.Log("때릴수 있는 거리네");
                        GetComponent<Character>().Move(false);
                        hited = hit.transform.gameObject;
                        Attack(hit.transform.gameObject);
                    }
                }
            }
            else if (hit.transform.gameObject.CompareTag("Minion"))
            {
                if (hit.transform.gameObject.GetComponent<MinionController>().GetTeamCode() != ownTeam)
                {
                    Debug.Log("적이다! 그것도 미니언");
                    if (Vector3.Distance(transform.position, hit.transform.position) <= attackRange)
                    {
                        Debug.Log("때릴수 있는 거리네");
                        GetComponent<Character>().Move(false);
                        hited = hit.transform.gameObject;
                        Attack(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                Debug.Log("코어인갑다");
                if (Vector3.Distance(transform.position, hit.transform.position) <= attackRange)
                {
                    Debug.Log("때릴수 있는 거리네");
                    GetComponent<Character>().Move(false);
                    hited = hit.transform.gameObject;
                    Attack(hit.transform.gameObject);
                }
            }
        }
        else
        {
            GetComponent<Character>().Move(true);
            navMeshAgent.SetDestination(target);
        }
    }

    // 공격
    private void Attack(GameObject obj)
    {
        Debug.Log("데미지 주자!");
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
        //TODO: 민규 UI에 붙이기
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UIManager.instance.SkillUI_Cooldown(UIManager.SKILL_Q, 3);
            this.photonView.RPC("Skill", RpcTarget.All, "Q");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            UIManager.instance.SkillUI_Cooldown(UIManager.SKILL_W, 3);
            this.photonView.RPC("Skill", RpcTarget.All, "W");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UIManager.instance.SkillUI_Cooldown(UIManager.SKILL_E, 3);
            this.photonView.RPC("Skill", RpcTarget.All, "E");
        }
    }

    [PunRPC]
    public void Skill(string key)
    {
        switch (key)
        {
            case "Q":
                character.Skill_Q();
                break;
            case "W":
                character.Skill_W();
                break;
            case "E":
                character.Skill_E();
                break;
        }
    }
}