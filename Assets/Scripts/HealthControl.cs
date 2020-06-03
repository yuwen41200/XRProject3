using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthControl : MonoBehaviour
{
    public GameObject[] playerHP;
    public GameObject[] bossHP;

    private int playerIdx;
    private int bossIdx;
    // Start is called before the first frame update
    void Start()
    {
        playerIdx = 5;
        bossIdx = 5;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPlayerHP(float i)
    {
        playerIdx = Mathf.FloorToInt(i);
        RefreshPlayerHP();
    }

    public void SetBossHP(float i)
    {
        bossIdx = Mathf.FloorToInt(i);
        RefreshBossHP();
    }

    public void AddPlayerHP(int i)
    {
        playerIdx += i;
        if (playerIdx > 5)
            playerIdx = 5;
        RefreshPlayerHP();
    }

    public void MinusPlayerHP(int i)
    {
        playerIdx -= i;
        if (playerIdx < 0)
            playerIdx = 0;
        RefreshPlayerHP();
    }

    public void AddBossHP(int i)
    {
        bossIdx += i;
        if (bossIdx > 5)
            bossIdx = 5;
        RefreshBossHP();
    }

    public void MinusBossHP(int i)
    {
        bossIdx -= i;
        if (bossIdx < 0)
            bossIdx = 0;
        RefreshBossHP();
    }

    private void RefreshPlayerHP()
    {
        for (int i = 0; i < 5; ++i)
        {
            if (i < playerIdx)
                playerHP[i].SetActive(true);
            else
                playerHP[i].SetActive(false);
        }
    }

    private void RefreshBossHP()
    {
        for (int i = 0; i < 5; ++i)
        {
            if (i < bossIdx)
                bossHP[i].SetActive(true);
            else
                bossHP[i].SetActive(false);
        }
    }
}
