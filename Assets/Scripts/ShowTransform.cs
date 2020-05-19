using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    Vector3 curPosition;
    List<Vector3> PositionList = new List<Vector3>();
    List<Vector3> SpeedList = new List<Vector3>();


    [SerializeField]
    float speedTreshold;

    [SerializeField]
    Vector3 curSpeed;


    // Start is called before the first frame update
    void Start()
    {
        objTransform = obj.GetComponent<Transform>();
        CameraRigObj = GameObject.Find("[CameraRig]");
        CameraRigTransform = CameraRigObj.GetComponent<Transform>();
        init();
    }


    private void FixedUpdate()
    {
        CalSpeed();
        if(PositionList.Count > 100)
        {
            Debug.Log("Start Detect !");
            DetectMovement();
        }
        ShowMsg();

        if (Input.GetKey(KeyCode.Space))
        {
            PositionList.Clear();
            SpeedList.Clear();
            init();
            Debug.Log("Clear");
        }

    }




    void CalSpeed()
    {
        curPosition = objTransform.position - CameraRigTransform.position;
        curSpeed = (curPosition - PositionList[PositionList.Count-1]) / Time.fixedDeltaTime;

        //if (Mathf.Abs(curSpeed.y) > speedTreshold && Mathf.Abs(SpeedList[PositionList.Count - 1].y) > speedTreshold )
        //{
        //    if(Mathf.Sign(curSpeed.y) == Mathf.Sign(SpeedList[PositionList.Count - 1].y)) Debug.Log("upup!");
        //}
            
        PositionList.Add(curPosition);
        SpeedList.Add(curSpeed);


    }
    void DetectMovement()
    {
        //  [0] is up, [1] is down, [2] is left, [3] is right
        int[] validSpeed = new int[4];
        bool hasEnoughSpeed = false;

        for(int i = 1; i < SpeedList.Count; i++)
        {
            if (SpeedList[i].y > speedTreshold)
            {
                validSpeed[0]++;
            }
            if (SpeedList[i].y < -speedTreshold)
            {
                validSpeed[1]++;
            }
            if (SpeedList[i].x < -speedTreshold)
            {
                validSpeed[2]++;
            }
            if ( SpeedList[i].x > speedTreshold)
            {
                validSpeed[3]++;
            }
        }

        int Maxindex = 0 , MaxValue = 0;
        for (int i = 0; i < 4; i++)
        {
            if(validSpeed[i] > MaxValue)
            {
                Maxindex = i;
                MaxValue = validSpeed[i];
            }
        }
        if (validSpeed[Maxindex] > 10)
        {
            if (Maxindex == 0) Debug.Log("we detect up!");
            if (Maxindex == 1) Debug.Log("we detect down!");
            if (Maxindex == 2) Debug.Log("we detect left!");
            if (Maxindex == 3) Debug.Log("we detect right!");
        } 

        PositionList.Clear();
        SpeedList.Clear();
        init();

    }
    void ShowMsg()
    {
        text.text = objTransform.position.ToString() + "\n" + curSpeed;
    }

    void init()
    {
        curPosition = objTransform.position - CameraRigTransform.position;
        curSpeed = Vector3.zero;
        PositionList.Add(curPosition);
        SpeedList.Add(curSpeed);

    }
}
