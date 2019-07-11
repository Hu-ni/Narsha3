using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Transform tr;
    Transform cameraTr;

    void Start()
    {
        tr = this.transform;
        cameraTr = Camera.main.transform;
    }
    void Update()
    {
        tr.LookAt(cameraTr.position * -1);
    }
}