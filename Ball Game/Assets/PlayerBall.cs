using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBall : MonoBehaviour
{
    private Rigidbody rigid;
    private bool IsJump;
    public float JumpPower;
    public int ItemCount;
    public GameManagerLogic Manager;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();    
    }

    void Update()
    {
        if(Input.GetButtonDown("Jump") && !IsJump)
        {
            IsJump = true;
            rigid.AddForce(new Vector3(0, JumpPower, 0), ForceMode.Impulse);
        }
    }
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        rigid.AddForce(new Vector3(h, 0, v), ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            IsJump = false;
        }
    }

    void OnTriggerEnter(Collider other)

    {
        if (other.tag == "Item")
        {
            ItemCount++;

            other.gameObject.SetActive(false);

            Manager.GetItemCount(ItemCount);
        }

        else if (other.tag == "Finish")
        {
            if(ItemCount == Manager.TotalItemCount)
            {
                if (Manager.Stage == 1)
                {
                    SceneManager.LoadScene("Level0");
                }
                else
                {
                    SceneManager.LoadScene("Level" + (Manager.Stage + 1));
                }
            }
            else
            {
                SceneManager.LoadScene("Level" + Manager.Stage);
            }
        }
    }
}
