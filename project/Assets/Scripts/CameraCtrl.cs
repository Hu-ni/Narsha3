using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    private Transform tr;
    private Camera mianCamera;
    private float cameraSpeed = 20;

    void Start()
    {
        tr = this.GetComponent<Transform>();
        mianCamera = Camera.main;
    }

    void Update()
    {
        // 휠로 줌
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (tr.position.y < 50)
                tr.position += new Vector3(0, cameraSpeed * Time.deltaTime, 0);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (tr.position.y > 20)
                tr.position += new Vector3(0, -cameraSpeed * Time.deltaTime, 0);
        }

        // 카메라 워크
        Vector3 screenPos = mianCamera.ScreenToViewportPoint(Input.mousePosition);


        if (screenPos.x <= 0.1)
        {
            if (tr.position.x > -50)
            {
                float ac = screenPos.x <= 0 ? 2f : 0.5f;
                tr.position += new Vector3(-cameraSpeed * ac * Time.deltaTime, 0, 0);
            }
        }
        // TODO: 그 뭐냐 오른쪽으로 마우스 했을 떄 속도 안 올라감
        if (screenPos.x >= 0.8)
        {
            if (tr.position.x < 60)
            {
                float ac = screenPos.x >= 0.9 ? 2f : 0.5f;
                tr.position += new Vector3(cameraSpeed * ac * Time.deltaTime, 0, 0);
            }
        }

        if (screenPos.y <= 0.1)
        {
            if (tr.position.z > -50)
            {
                float ac = screenPos.y <= 0 ? 2f : 0.5f;
                tr.position += new Vector3(0, 0, -cameraSpeed * ac * Time.deltaTime);
            }
        }

        if (screenPos.y >= 0.8)
        {
            if (tr.position.z < 60)
            {
                float ac = screenPos.y >= 0.9 ? 2f : 0.5f;
                tr.position += new Vector3(0, 0, cameraSpeed * ac * Time.deltaTime);
            }
        }
    }
}
