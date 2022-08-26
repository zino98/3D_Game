using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerLogic : MonoBehaviour
{
    public int TotalItemCount;
    public int Stage;
    public Text PlayerItemCount;
    public Text StageItemCount;

    void Awake()
    {
        StageItemCount.text = " / " + TotalItemCount.ToString();
    }

    // Update is called once per frame
    public void GetItemCount(int count)
    {
        PlayerItemCount.text = count.ToString();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            SceneManager.LoadScene("Level" + Stage);
        }
    }
}
