using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Transform Player;
    private Vector3 offset;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - Player.position;

    }
    void Update()
    {
        transform.position = Player.position + offset;
    }
}
