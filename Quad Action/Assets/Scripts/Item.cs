using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon }
    public Type type;
    public int value;

    Rigidbody rigid;
    SphereCollider sphere;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphere = GetComponent<SphereCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    /*void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            rigid.isKinematic = true;
            sphere.enabled = false;
        }
    }*/
}
