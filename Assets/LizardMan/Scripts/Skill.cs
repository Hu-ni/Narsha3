using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Skill : MonoBehaviour
{
    public float mana;

    public float cooltime;
    private float cool;

    public bool isActive = true;

    public GameObject model;

    public IEnumerator SpendCoolTime()
    {
        isActive = false;
        cool = cooltime;
        while (cool > 0)
        {
            cool -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        isActive = true;
    }

    public void ModelActive(bool isActive)
    {
        model.SetActive(isActive);
    }

    public void ResetPosition(Vector3 pos)
    {
        model.transform.position = pos;
    }
}
