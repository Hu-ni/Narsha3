using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public Transform target;
    public float Damage = 50;
    [SerializeField]
    private float speed;

    public void SetTarget(Transform transform)
    {
        this.target = transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        //target = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (target.gameObject.activeSelf == false)
            {
                Destroy(gameObject);
            }
            Vector3 dir = target.position - transform.position;
            float distance = speed * Time.deltaTime;
            transform.Translate(dir.normalized * distance, Space.World);
            transform.LookAt(target);
        }
        catch
        {

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target.gameObject)
        {
            if (other.gameObject.CompareTag("Champion"))
            {
                other.gameObject.GetComponent<Character>().TakeDamage(Damage);
            }
            else if (other.gameObject.CompareTag("Minion"))
            {
                other.gameObject.GetComponent<MinionController>().TakeDamage(Damage);
            }
            Debug.Log("타깃적중...");
            Destroy(gameObject);
        }
    }
}
