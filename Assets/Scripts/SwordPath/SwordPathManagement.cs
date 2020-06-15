using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPathManagement : MonoBehaviour
{
    [SerializeField]
    int[] answerDirArr = new int[8];

    [SerializeField]
    GameObject[] eightPiece;

    [SerializeField]
    int atkTreshold;

    [SerializeField]
    GameObject fistHead;
    public Vector3 vel;
    public float gravity;
    GameManagement gameManagement;

    Transform towardT;

    private GameObject boss;
    int countFish = 0;
    public bool goToEndScene;

    public AudioSource attackAS;

    // Start is called before the first frame update
    void Start()
    {
        gameManagement = GameObject.Find("GameManagementCarrier").GetComponent<GameManagement>();
        //SetPieceAns();
        towardT = GameObject.Find("Camera").GetComponent<Transform>();
        // 依照答案轉圖片方向
        //Vector3 roEular = new Vector3(Mathf.PI / 6 * answerDir, 0, 0);
        goToEndScene = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (countFish == 8) StartCoroutine(WeClear());
        if (Input.GetKeyDown(KeyCode.F)) StartCoroutine(WeClear());
    }

    public void AnswerMatch(int[] voteResult, int MaxValue, int Maxindex) {
        Debug.Log(voteResult[0] + " " + voteResult[1] + " " + voteResult[2] + " " + voteResult[3] + " " + MaxValue);
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
            Debug.Log("Answer Correct!");
            attackAS.Play();
            int N = gameManagement.ReturnPlayerN() / 3;
            if (eightPiece[N] != null)
            {
                eightPiece[N].GetComponent<Piece>().Die();
                countFish++;
            }
        }
        else
        {
            // we get wrong SwordPath Answer, add point then do something
        }
        
    }

    private bool AnswerCheck(int givenDir) {
        int N = gameManagement.ReturnPlayerN()/3;
        Debug.Log("Player is in " + N);
        int givenTwisted = (givenDir == 999) ? 999 :(givenDir + 6) % 12;
        if (Mathf.Abs(givenDir - answerDirArr[N]) <= 1 || Mathf.Abs(givenDir - answerDirArr[N]) == 11)
        {
            return true;
        }else if (Mathf.Abs(givenTwisted - answerDirArr[N]) <= 1 || Mathf.Abs(givenTwisted - answerDirArr[N]) == 11)
        {
            return true;
        }
        else
        {
            //Debug.Log("Answer Incorrect!");
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

    private void SetPieceAns()
    {
        int index = 0;
        foreach (GameObject i in eightPiece)
        {
            i.GetComponent<Piece>().SetAns(answerDirArr[index]);
            index++;
        }
    }

    IEnumerator WeClear() {
        countFish++;
        Debug.Log("Clear");
        StartCoroutine(jump()); 
        for (int i = 0; i < 40; i++)
            yield return null;
    }

    IEnumerator jump()
    {
        for (int i = 0; i < 40; i++) {
            fistHead.transform.Translate(vel * Time.fixedDeltaTime, Space.Self);
            vel.y += -gravity;
            fistHead.transform.Rotate(Vector3.down, 2, Space.Self);
            yield return null;
        }
        vel.x = 0;
        for (int i = 0; i < 160; i++)
        {
            fistHead.transform.Translate(vel/2 * Time.fixedDeltaTime, Space.World);
            yield return null;
        }
        goToEndScene = true;
        yield return null;
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
