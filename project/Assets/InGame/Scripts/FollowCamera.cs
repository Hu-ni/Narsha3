using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    public static FollowCamera instance;
    public Transform playerTrasnfrom;
    private Vector3 cameraOffset;

    
    [Range(0.01f, 1.0f)]
    public float smoothness = 0.5f;
    // Start is called before the first frame update
    private void Awake() {
        instance = this;
    }
    void Start()
    {
        playerTrasnfrom = GameObject.FindGameObjectWithTag("Model").gameObject.transform;
        cameraOffset = transform.position - playerTrasnfrom.transform.position;        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 newPos = playerTrasnfrom.position + cameraOffset;
        transform.position = Vector3.Slerp(transform.position, newPos, smoothness);
    }
}
