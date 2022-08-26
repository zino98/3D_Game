using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Transform PlayerTransform;
    private Vector3 offset;
    void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - PlayerTransform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = PlayerTransform.position + offset;
    }
}
