using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float maxEdge = 0.9f;
    public float minEdge = 0.2f;

    public float CamSpeed = 0.3f;
    void Start()
    {
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        if (mousePos.x > maxEdge && mousePos.x < 1.0f)
        {
            Camera.main.transform.Translate(new Vector3(CamSpeed, 0, 0));
        }

        if (mousePos.y > maxEdge && mousePos.y < 1.0f)
        {
            Camera.main.transform.Translate(new Vector3(0, CamSpeed, 0));
        }
        
        if (mousePos.x < minEdge && mousePos.x > 0.0f)
        {
            Camera.main.transform.Translate(new Vector3(-CamSpeed, 0, 0));
        }

        if (mousePos.y < minEdge && mousePos.y > 0f)
        {
            Camera.main.transform.Translate(new Vector3(0, -CamSpeed, 0));
        }
    }
}
