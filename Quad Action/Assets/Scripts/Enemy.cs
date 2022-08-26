using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public Transform target;
    public bool isChase;

    private bool isGrenade;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;
    
    Vector3 reactVec;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;

        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("Chaseout", 2);

    }

    void FixedUpdate()
    {
        nav.SetDestination(target.position);
        FreezePosition();
    }

    void FreezePosition()
    {
        if(isChase) {

            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
        
    }

    void Chaseout()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            health -= weapon.damage;

            Debug.Log("Melee: " + health);
        }

        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            health -= bullet.damage;
            reactVec = transform.position - other.transform.position;

            StartCoroutine("OnDamage");

            Debug.Log("Range: " + health);
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        health -= 100;
        reactVec = transform.position - explosionPos;
        isGrenade = true;

        StartCoroutine("OnDamage");
    }

    IEnumerator OnDamage()
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (health > 0)
            mat.color = Color.white;

        else
        {
            mat.color = Color.gray;
            gameObject.layer = 12;

            anim.SetTrigger("doDie");
            isChase = false;
            nav.enabled = false;

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 10, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }

            else
            {
                reactVec = reactVec.normalized;
                rigid.AddForce(reactVec * 7, ForceMode.Impulse);
            }

            Destroy(gameObject, 3);


        }

    }
}
