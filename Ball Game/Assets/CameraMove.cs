using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamer : MonoBehaviour
{
    private Transform PlayerTransform;
    private Vector3 offset;
    void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - PlayerTransform.position;
    }

    
    void LateUpdate()
    {
        transform.position = PlayerTransform.position + offset;
    }
}
