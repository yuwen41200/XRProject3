using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class ShowTransform : MonoBehaviour
{
    Transform objTransform;

    [SerializeField]
    Vector3 deltaVel;
    GameObject CameraRigObj;
    Transform CameraRigTransform;

    List<Vector3> PositionList = new List<Vector3>();
    List<Vector3> SpeedList = new List<Vector3>();
    List<Vector3> RotationList = new List<Vector3>();

    [SerializeField]
    float speedThreshold;
    [SerializeField]
    float rotationThreshold;

    Vector3 curSpeed;
    Vector3 curPosition;
    Vector3 curRotation;
    bool isFirstDetect;
    public int direction;

    int[] valid;

    [SerializeField]
    GameManagement gameManagement;
    [SerializeField]
    GameObject theTar;
    [SerializeField]
    private SteamVR_Behaviour_Pose pose;

    // Start is called before the first frame update
    void Start()
    {
        gameManagement = GameObject.Find("GameManagementCarrier").GetComponent<GameManagement>();
        objTransform = FindHandTransform(direction);
        CameraRigObj = GameObject.Find("[CameraRig]");
        pose = FindHandPose(direction);
        theTar = FindRightSwordPath(); 
        CameraRigTransform = CameraRigObj.GetComponent<Transform>();
        isFirstDetect = true;
        initState();
    }


    private void FixedUpdate()
    {
        Cal_Update_Data();
        // test, change 100 to 50
        if (PositionList.Count > 30 && isFirstDetect == true)
        {
            isFirstDetect = false;
            DetectMovement();
            Destroy(gameObject, 1f);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            PositionList.Clear();
            SpeedList.Clear();
            RotationList.Clear();
            initState();
            Debug.Log("Clear");
        }

    }

    void Cal_Update_Data()
    {
        curPosition = objTransform.position - CameraRigTransform.position;
        curSpeed = (curPosition - PositionList[PositionList.Count-1]) / Time.fixedDeltaTime;
        curRotation = pose.GetAngularVelocity();

        PositionList.Add(curPosition);
        SpeedList.Add(curSpeed);
        RotationList.Add(curRotation);

    }

    void DetectMovement()
    {
        //  [0] is up, [1] is down, [2] is left, [3] is right
        CompareForMove();
        CompareForRotate();

        int Maxindex = 0, MaxValue = 0;
        if (valid[0] > valid[1])
        {
            Maxindex = 0;
            MaxValue = valid[0];
        }
        else
        {
            Maxindex = 1;
            MaxValue = valid[1];
        }
        if (valid[2] > MaxValue)
        {
            Maxindex = 2;
            MaxValue = valid[2];
        }
        if (valid[3] > MaxValue)
        {
            Maxindex = 3;
            MaxValue = valid[3];
        }

        //Debug.Log(valid[0] + "," + valid[1] + "," + valid[2] + "," + valid[3]);

        if (direction == 1)
        {
            if (MaxValue > 7)
            {
                if (Maxindex == 0)
                {
                    if (valid[0] - valid[3] > MaxValue / 4 && valid[0] - valid[2] > MaxValue / 4)
                    {
                        gameManagement.detectedActions.Enqueue(PlayerAction.MoveBack);
                        Debug.Log("we detect up!");
                    }
                }
                else if (Maxindex == 1)
                {
                    if (valid[1] - valid[3] > MaxValue / 4 && valid[1] - valid[2] > MaxValue / 4)
                    {
                        gameManagement.detectedActions.Enqueue(PlayerAction.MoveFront);
                        Debug.Log("we detect down!");
                    }
                }
                else if (Maxindex == 2)
                {
                    gameManagement.detectedActions.Enqueue(PlayerAction.MoveLeft);
                    Debug.Log("we detect left!");
                }
                else if (Maxindex == 3)
                {
                    gameManagement.detectedActions.Enqueue(PlayerAction.MoveRight);
                    Debug.Log("we detect right!");
                }
                else
                {
                    gameManagement.detectedActions.Enqueue(PlayerAction.NoAction);
                }
            }
            else
            {
                gameManagement.detectedActions.Enqueue(PlayerAction.NoAction);
            }
        }
        else if (direction == -1)
        {
            // 傳 ans 給 SwordPath 他會自己判斷
            if (theTar != null)theTar.GetComponent<SwordPathManagement>().AnswerMatch(valid,MaxValue,Maxindex);
        }
        Destroy(gameObject, 0.5f);
    }

    void initState()
    {
        curPosition = objTransform.position - CameraRigTransform.position;
        curSpeed = curRotation = Vector3.zero;
        PositionList.Add(curPosition);
        SpeedList.Add(curSpeed);
        RotationList.Add(curRotation);
        valid = new int[4];
    }

    void CompareForMove()
    {
        for (int i = 1; i < SpeedList.Count; i++)
        {
            if (SpeedList[i].y > speedThreshold)
            {
                valid[0]++;
                if (i > 10 && i < 20) 
                {
                    valid[0] = valid[0] + 1;
                    if (SpeedList[i].y > 2.5 * speedThreshold) valid[0] = valid[0] + 4 ;
                } 
                if (SpeedList[i].y > 2 * speedThreshold) valid[0]++;
                if (SpeedList[i].y > 3 * speedThreshold) valid[0]++;
            }
            if (SpeedList[i].y < -speedThreshold)
            {
                valid[1]++;
                if (i > 10 && i < 20)
                {
                    valid[1] = valid[1] + 1;
                    if (SpeedList[i].y > 2.5 * -speedThreshold) valid[1] = valid[1] + 4;
                }
                if (SpeedList[i].y > 2 * -speedThreshold) valid[1]++;
                if (SpeedList[i].y > 3 * -speedThreshold) valid[1]++;
            }
        }
    }
    void CompareForRotate()
    {   //  X- => up, Y- => left
        for (int i = 1; i < RotationList.Count; i++)
        {
            if (RotationList[i].y < -rotationThreshold)
            {
                valid[2]++;
                if (i > 10 && i < 20) valid[2] = valid[2] + 2;
                if (RotationList[i].y < 2 * -rotationThreshold) valid[2]++;
            }
            if (RotationList[i].y > rotationThreshold)
            {
                valid[3]++;
                if (i > 10 && i < 20) valid[3] = valid[3] + 2;
                if (RotationList[i].y < 2 * rotationThreshold) valid[3]++;
            }
        }
    }
    GameObject FindRightSwordPath()
    {
        if (direction == 1) return null;
        GameObject obj = GameObject.FindGameObjectWithTag("Boss");
        // 會找到在BOSS 底下掛有 SwordPathManagement的那個物件
        return obj;
    }
    Transform FindHandTransform(int dir) 
    {
        if (dir == 1) return GameObject.Find("Controller (left)").GetComponent<Transform>();
        else if (dir == -1) return GameObject.Find("Controller (right)").GetComponent<Transform>();
        else return null;
    }
    SteamVR_Behaviour_Pose FindHandPose(int dir)
    {
        if (dir == 1) return GameObject.Find("Controller (left)").GetComponent<SteamVR_Behaviour_Pose>();
        else if (dir == -1) return GameObject.Find("Controller (right)").GetComponent<SteamVR_Behaviour_Pose>();
        else return null;
    }

    private void Update() {
        // 測試用，用鍵盤控制玩家動作
        if (Input.GetKeyDown(KeyCode.W))
            gameManagement.detectedActions.Enqueue(PlayerAction.MoveFront);
        else if (Input.GetKeyDown(KeyCode.S))
            gameManagement.detectedActions.Enqueue(PlayerAction.MoveBack);
        else if (Input.GetKeyDown(KeyCode.A))
            gameManagement.detectedActions.Enqueue(PlayerAction.MoveLeft);
        else if (Input.GetKeyDown(KeyCode.D))
            gameManagement.detectedActions.Enqueue(PlayerAction.MoveRight);
        else if (Input.GetKeyDown(KeyCode.I))
            gameManagement.detectedActions.Enqueue(PlayerAction.AttackUp);
        else if (Input.GetKeyDown(KeyCode.K))
            gameManagement.detectedActions.Enqueue(PlayerAction.AttackDown);
        else if (Input.GetKeyDown(KeyCode.J))
            gameManagement.detectedActions.Enqueue(PlayerAction.AttackLeft);
        else if (Input.GetKeyDown(KeyCode.L))
            gameManagement.detectedActions.Enqueue(PlayerAction.AttackRight);
        else if (Input.GetKeyDown(KeyCode.Space))
            gameManagement.detectedActions.Enqueue(PlayerAction.NoAction);
    }

}

public enum PlayerAction {
    MoveFront, MoveBack, MoveLeft, MoveRight,
    AttackUp, AttackDown, AttackLeft, AttackRight,
    NoAction
}
