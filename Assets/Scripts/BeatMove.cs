using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatMove : MonoBehaviour
{
    public bool isMove;
    [SerializeField]
    private float speed;
    [SerializeField]
    private int direction;

    // default is 0 (local position)
    public float judgePoint;

    public GameObject dectingMoveObj;
    public GameObject dectingAtkObj;
    public float detectPoint;

    private BeatPool bp;
    public bool isFirstInDetectPoint;

    // Start is called before the first frame update
    void Start()
    {
        // 25 is the half of interation in ShowTransform's DetectMovement
        detectPoint = speed * Time.fixedDeltaTime * 25;
        isFirstInDetectPoint = true;
    }

    void OnEnable()
    {
        isMove = true;
        isFirstInDetectPoint = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMove)
        {
            Vector3 curPos = gameObject.GetComponent<RectTransform>().localPosition;
            curPos.x += direction*speed*Time.fixedDeltaTime;
            gameObject.GetComponent<RectTransform>().localPosition = curPos;

            // 超過判斷點，開始計算動作
            if ((curPos.x - judgePoint) * direction > -detectPoint && isFirstInDetectPoint == true)
            {
                isFirstInDetectPoint = false;
                if (direction == 1)
                {
                    GameObject obj = Instantiate(dectingMoveObj, curPos, Quaternion.identity);
                    obj.GetComponent<ShowTransform>().direction = direction;
                }
                else if(direction == -1)
                {
                    GameObject obj = Instantiate(dectingAtkObj, curPos, Quaternion.identity);
                    obj.GetComponent<ShowTransform>().direction = direction;
                }
                //Destroy(this.gameObject);
            }
            // 超過判斷點，消失
            if ((curPos.x - judgePoint) * direction > 0)
            {
                bp.Recycle(this.gameObject);
                //Destroy(this.gameObject);
            }
        }
    }

    public void SetSpeed(float v)
    {
        this.speed = v;
    }



    /*
     * left is -1, right is 1 
     */
    public void SetDirection(int dir)
    {
        if (dir < 0)
            this.direction = -1;
        else
            this.direction = 1;
    }

    public void SetJudgePoint(float x)
    {
        this.judgePoint = x;
    }

    public void SetBeatPool(BeatPool b)
    {
        this.bp = b;
    }
}
