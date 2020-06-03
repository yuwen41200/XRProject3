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
        // 依照答案轉方向
    }

    // Update is called once per frame
    void Update()
    {
        // 轉向user   
    }

    public void SetAnswer(int actualAns) {
        answerDir = actualAns;
        // 移動怪物身上的2D圖片
    }

    public void AnswerCheck(int givenDir) {
        if (Mathf.Abs(givenDir - answerDir) <= 1 || Mathf.Abs(givenDir - answerDir) == 11)
        {
            Debug.Log("Answer Correct!");
        }
        else
        {
            Debug.Log("Answer Incorrect!");
        }
        Destroy(gameObject,0.5f);
    }
}
