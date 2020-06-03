using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPath : MonoBehaviour
{
    [SerializeField]
    int answerDir;
    
    [SerializeField]
    int atkTreshold;

    [SerializeField]
    GameManagement gameManagement;
    public bool isBind;
    // Start is called before the first frame update
    void Start()
    {
        isBind = false;
        gameManagement = GameObject.Find("GameManagementCarrier").GetComponent<GameManagement>();
        // set Parent to the Boss/Monster
        GameObject boss = GameObject.FindWithTag("Boss");
        gameObject.transform.SetParent(boss.transform);
        gameObject.transform.position = boss.transform.position;

        // 依照答案轉圖片方向
    }

    // Update is called once per frame
    void Update()
    {
        // 圖片轉向user
    }

    public void SetAnswer(int actualAns) {
        answerDir = actualAns;
        // 移動怪物身上的2D圖片
    }

    public void AnswerMatch(int[] voteResult,int MaxValue,int Maxindex){
        int motion = 2;
        int[] minusRelated = MinusRelated(voteResult);
        if (minusRelated[0] > atkTreshold && minusRelated[2] > atkTreshold)
        {
            if (minusRelated[0] > minusRelated[2]) motion = 11;
            else motion = 10;
        }
        else if (minusRelated[0] > atkTreshold && minusRelated[3] > atkTreshold)
        {
            if (minusRelated[0] > minusRelated[3]) motion = 1;
            else motion = 2;
        }
        else if (minusRelated[1] > atkTreshold && minusRelated[2] > atkTreshold)
        {
            if (minusRelated[1] > minusRelated[2]) motion = 7;
            else motion = 8;
        }
        else if (minusRelated[1] > atkTreshold && minusRelated[3] > atkTreshold)
        {
            if (minusRelated[1] > minusRelated[3]) motion = 5;
            else motion = 4;
        }
        else if (minusRelated[0] > atkTreshold) motion = 0;
        else if (minusRelated[1] > atkTreshold) motion = 6;
        else if (minusRelated[2] > atkTreshold) motion = 9;
        else if (minusRelated[3] > atkTreshold) motion = 3;
        else motion = 999;

        if (AnswerCheck(motion))
        {
            // we get right SwordPath Answer, add point then do something
        }
        else
        {
            // we get wrong SwordPath Answer, add point then do something
        }

        Destroy(gameObject, 0.5f);
    }

    private bool AnswerCheck(int givenDir) {
        if (Mathf.Abs(givenDir - answerDir) <= 1 || Mathf.Abs(givenDir - answerDir) == 11)
        {
            Debug.Log("Answer Correct!");
            return true;
        }
        else
        {
            Debug.Log("Answer Incorrect!");
            return false;
        }
    }

    private int[] MinusRelated(int[] vote)
    {
        int[] minusResult = new int[4];
        minusResult[0] = vote[0] - vote[1];
        minusResult[1] = -minusResult[0];
        minusResult[2] = vote[2] - vote[3];
        minusResult[3] = -minusResult[2];
        return minusResult;
    }
}


//if (Maxindex == 0)
//{
//    Debug.Log("we detect up!");
//}
//else if (Maxindex == 1)
//{
//    Debug.Log("we detect down!");
//}
//else if (Maxindex == 2)
//{
//    Debug.Log("we detect left!");
//}
//else if (Maxindex == 3)
//{
//    Debug.Log("we detect right!");
//}
//else
//{
//    Debug.Log("we detect nothing!");
//}