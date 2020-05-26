using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class ShowTransform : MonoBehaviour
{
    [SerializeField]
    GameObject obj;
    Transform objTransform;
    [SerializeField]
    Text text;

    [SerializeField]
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

    public bool turnedOn; // This flag will be set by GameManagement
    public Queue<PlayerAction> detectedActions; // Used by GameManagement

    [SerializeField]
    private Text testDirection;


    [SerializeField]
    private SteamVR_Behaviour_Pose pose;

    // Start is called before the first frame update
    void Start()
    {
        objTransform = obj.GetComponent<Transform>();
        CameraRigObj = GameObject.Find("[CameraRig]");
        CameraRigTransform = CameraRigObj.GetComponent<Transform>();
        init();
        turnedOn = false;
        detectedActions = new Queue<PlayerAction>();
    }


    private void FixedUpdate()
    {
        ShowMsg();
        if (!turnedOn) return;

        Cal_Update_Data();
        // test, change 100 to 50
        if (PositionList.Count > 50)
        {
            Debug.Log("Start Judge !");
            DetectMovement();
            turnedOn = false;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            PositionList.Clear();
            SpeedList.Clear();
            RotationList.Clear();
            init();
            Debug.Log("Clear");
        }

    }

    void Cal_Update_Data()
    {
        curPosition = objTransform.position - CameraRigTransform.position;
        curSpeed = (curPosition - PositionList[PositionList.Count-1]) / Time.fixedDeltaTime;
        curRotation = pose.GetAngularVelocity();

        //if (Mathf.Abs(curSpeed.y) > speedTreshold && Mathf.Abs(SpeedList[PositionList.Count - 1].y) > speedTreshold )
        //{
        //    if(Mathf.Sign(curSpeed.y) == Mathf.Sign(SpeedList[PositionList.Count - 1].y)) Debug.Log("upup!");
        //}

        PositionList.Add(curPosition);
        SpeedList.Add(curSpeed);
        RotationList.Add(curRotation);
    }

    void DetectMovement()
    {
        //  [0] is up, [1] is down, [2] is left, [3] is right
        int[] validSpeed = new int[4];

        for(int i = 1; i < SpeedList.Count; i++)
        {
            if (SpeedList[i].y > speedThreshold)
            {
                validSpeed[0]++;
            }
            if (SpeedList[i].y < -speedThreshold)
            {
                validSpeed[1]++;
            }

            /*  try to use other data to detect left and right*/
            if (SpeedList[i].x < -speedThreshold)
            {
                validSpeed[2]++;
            }
            if ( SpeedList[i].x > speedThreshold)
            {
                validSpeed[3]++;
            }
        }

        //  [0] is up, [1] is down, [2] is left, [3] is right
        int[] validRotation = new int[4];
        //  X- => up, Y- => left
        for(int i = 1; i < RotationList.Count; i++)
        {
            if (RotationList[i].x < -rotationThreshold)
            {
                validRotation[0]++;
            }
            if (RotationList[i].x > rotationThreshold)
            {
                validRotation[1]++;
            }
            if (RotationList[i].y < -rotationThreshold)
            {
                validRotation[2]++;
            }
            if (RotationList[i].y > rotationThreshold)
            {
                validRotation[3]++;
            }
        }

        // Debug.Log(validSpeed[0] + "," + validSpeed[1]+ "," +validRotation[2]+ "," +validRotation[3]);

        int Maxindex = 0 , MaxValue = 0;

        if(validSpeed[0] > validSpeed[1])
        {
            Maxindex = 0;
            MaxValue = validSpeed[0];
        }else{
            Maxindex = 1;
            MaxValue = validSpeed[1];
        }
        if(validRotation[2] > MaxValue)
        {
            Maxindex = 2;
            MaxValue = validRotation[2];
        }
        if(validRotation[3] > MaxValue){
            Maxindex = 3;
            MaxValue = validRotation[3];
        }

        if (MaxValue > 7)
        {
            if (Maxindex == 0) {
                detectedActions.Enqueue(PlayerAction.MoveBack);
                testDirection.text = "UP";
                Debug.Log("we detect up!");
            }
            else if (Maxindex == 1) {
                detectedActions.Enqueue(PlayerAction.MoveFront);
                testDirection.text = "Down";
                Debug.Log("we detect down!");
            }
            else if (Maxindex == 2) {
                detectedActions.Enqueue(PlayerAction.MoveLeft);
                testDirection.text = "Left";
                Debug.Log("we detect left!");
            }
            else if (Maxindex == 3) {
                detectedActions.Enqueue(PlayerAction.MoveRight);
                testDirection.text = "Right";
                Debug.Log("we detect right!");
            }
            else {
                testDirection.text = "None";
                detectedActions.Enqueue(PlayerAction.NoAction);
            }
        }
        else {
            detectedActions.Enqueue(PlayerAction.NoAction);
        }

        PositionList.Clear();
        SpeedList.Clear();
        RotationList.Clear();
        init();
    }

    void ShowMsg()
    {
        text.text = objTransform.position.ToString() + "\n" + curSpeed + "\n" +
        pose.GetAngularVelocity();
    }

    void init()
    {
        curPosition = objTransform.position - CameraRigTransform.position;
        curSpeed = curRotation = Vector3.zero;
        PositionList.Add(curPosition);
        SpeedList.Add(curSpeed);
        RotationList.Add(curRotation);
    }

    private void Update() {
        // 測試用，用鍵盤控制玩家動作
        if (Input.GetKeyDown(KeyCode.W))
            detectedActions.Enqueue(PlayerAction.MoveFront);
        else if (Input.GetKeyDown(KeyCode.S))
            detectedActions.Enqueue(PlayerAction.MoveBack);
        else if (Input.GetKeyDown(KeyCode.A))
            detectedActions.Enqueue(PlayerAction.MoveLeft);
        else if (Input.GetKeyDown(KeyCode.D))
            detectedActions.Enqueue(PlayerAction.MoveRight);
    }

}

public enum PlayerAction {
    MoveFront, MoveBack, MoveLeft, MoveRight,
    AttackUp, AttackDown, AttackLeft, AttackRight,
    NoAction
}
