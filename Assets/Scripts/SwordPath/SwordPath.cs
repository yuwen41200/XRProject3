using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPath : MonoBehaviour
{
    [SerializeField]
    int answerDir;
    // Start is called before the first frame update
    void Start()
    {
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

    public void AnswerMatch(int[] voteResult){
        Debug.Log( "This from SwordPaht ");
        int motion = 2;
        if (AnswerCheck(motion))
        {

        }
        else
        {

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
}
