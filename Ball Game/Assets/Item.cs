using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float RotateSpeed;
    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(Vector3.up, RotateSpeed * Time.deltaTime, Space.World);
    }

   
}
