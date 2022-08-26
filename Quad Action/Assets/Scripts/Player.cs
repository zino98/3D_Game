using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public Camera followCamera;
    public GameObject[] grenades;

    public int hasGrenades;
    public int ammo;
    public int coin;
    public int health;

    public int MaxAmmo;
    public int MaxCoin;
    public int MaxHealth;
    public int MaxGrenade;

    private float h;
    private float v;
    private int equipWeaponIndex = -1;

    private bool wDown;
    private bool jDown;
    private bool iDown;
    private bool fDown;
    private bool gDown;
    private bool rDown;
    private bool sDown1;
    private bool sDown2;
    private bool sDown3;

    private bool isJump;
    private bool isDodge;
    private bool isSwap;
    private bool isFireReady = true;
    private bool isReload;
    private bool isBorder;
    private float fireDelay;

    private Vector3 moveVec;
    private Vector3 dodgeVec;

    private Animator anim;
    private Rigidbody rigid;
    private GameObject nearObject;
    private Weapon equipWeapon;
    public GameObject grenadeObj;


    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Swap();
        Interaction();

        FreezeRotation();
        StopToWall();
    }

    void GetInput()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec = new Vector3(h, 0, v).normalized; // 방향값 보정

        if(!isBorder)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        if(isDodge)
        {
            moveVec = dodgeVec;
        }

        if (isSwap || !isFireReady || isReload)
            moveVec = Vector3.zero;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec); // 키보드

        Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); // 마우스
        RaycastHit rayHit;

        if(Physics.Raycast(ray, out rayHit, 100))
        {
            Vector3 nextVec = rayHit.point - transform.position;
            transform.LookAt(transform.position + nextVec);
        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 3, LayerMask.GetMask("Wall"));
    }
    void Jump()
    {
        if(jDown && !isJump && !isDodge && !isSwap && moveVec == Vector3.zero)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            isJump = true;

            anim.SetBool("isJump", true);
            anim.SetTrigger("DoJump");
        }
    }

    void Grenade()
    {
        if (hasGrenades == 0)
            return;

        if(gDown && !isReload && !isSwap && !isDodge)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); // 마우스
            RaycastHit rayHit;

            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 7;

                GameObject intantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody grenadeRigid = intantGrenade.GetComponent<Rigidbody>();
                grenadeRigid.AddForce(nextVec, ForceMode.Impulse);
                grenadeRigid.AddTorque(Vector3.left * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);

            }
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;                        // 장전시간
        isFireReady = equipWeapon.rate < fireDelay;         

        if(fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "DoSwing" : "DoShot");

            fireDelay = 0;
        }
    }

    void Reload()
    {
        if (equipWeapon == null)
            return;

        if (ammo == 0)
            return;

        if (equipWeapon.type == Weapon.Type.Melee)
            return;

        if(rDown && !isJump && !isDodge && !isSwap && isFireReady)
        {
            anim.SetTrigger("DoReload");
            isReload = true;

            Invoke("ReloadOut", 2);
             
        }
    }

    void ReloadOut()
    {
        int reAmmo = (ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo);
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;

        isReload = false;

    }

    void Dodge()
    {
        if (jDown && !isJump && moveVec != Vector3.zero &&!isSwap)
        {
            speed = speed * 2;
            dodgeVec = moveVec;

            anim.SetTrigger("DoDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        speed = speed * 0.5f;
        isDodge = false;
    }

    void Swap()
    {

        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;

        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;

        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            isSwap = true;

            anim.SetTrigger("DoSwap");

            Invoke("SwapOut", 0.4f);
        }

    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interaction()
    {

        if(iDown && nearObject != null && !isJump && !isDodge)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponsIndex = item.value;
                hasWeapons[weaponsIndex] = true;

                Destroy(nearObject);
            }

            else if (nearObject.tag == "Grenade")
            {
                for (int i = 0; i < 4; i++)
                {
                    grenades[i].SetActive(true);
                }

                Destroy(nearObject);
            }


        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            isJump = false;
            anim.SetBool("isJump", false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();

            switch(item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > MaxAmmo)
                        ammo = MaxAmmo;
                    break;

                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > MaxCoin)
                        coin = MaxCoin;
                    break;

                case Item.Type.Heart:
                    health += item.value;
                    if (health > MaxHealth)
                        health = MaxHealth;
                    break;

                case Item.Type.Grenade:
                    hasGrenades += item.value;
                    if (hasGrenades > MaxGrenade)
                        hasGrenades = MaxGrenade;
                    break;
            }
            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon" || other.tag == "Grenade")
        {
            nearObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Weapon" || other.tag == "Grenade")
        {
            nearObject = null;
        }
    }
}
