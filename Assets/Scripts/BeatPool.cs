using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatPool : MonoBehaviour
{
    private Queue<GameObject> pool;
    public GameObject element;

    [SerializeField]
    private Transform parent;

    public int MaxAmount;

    private float beatSpeed;

    [HideInInspector]
    public float spawnEarly;

    [SerializeField]
    private RectTransform JudgePoint;

    // 左右等長直接做鏡像，所以只需要設定一邊
    [SerializeField]
    private RectTransform leftBeatSpawnPos;

    // Start is called before the first frame update
    void Start()
    {
        SetBeatSpeed();
        ComputeSpawnEarly();
        this.PoolInit();
    }

    public void PoolInit()
    {
        pool = new Queue<GameObject>();
        for (int i = 0; i < MaxAmount; ++i)
        {
            GameObject g =  Instantiate(element, parent);
            g.GetComponent<BeatMove>().SetSpeed(beatSpeed);
            g.GetComponent<BeatMove>().SetJudgePoint(JudgePoint.localPosition.x);
            g.GetComponent<BeatMove>().SetBeatPool(this);
            g.SetActive(false);
            pool.Enqueue(g);
        }
    }

    public void ReuseByDirection(int direction)
    {
        GameObject g = pool.Dequeue();
        Vector3 spawnPos = leftBeatSpawnPos.localPosition;
        spawnPos.x *= direction;
        g.GetComponent<RectTransform>().localPosition = spawnPos;
        g.GetComponent<BeatMove>().SetDirection(direction);
        g.GetComponent<BeatMove>().isFirstInDetectPoint = true;
        g.SetActive(true);
    }

    public void Recycle(GameObject g)
    {
        g.SetActive(false);
        pool.Enqueue(g);

    }

    public void SetBeatSpeed()
    {
        //read file here
        beatSpeed = 100;
    }


    private void ComputeSpawnEarly()
    {
        float x1 = leftBeatSpawnPos.localPosition.x;
        float x2 = JudgePoint.localPosition.x;
        spawnEarly = (x1 - x2) / beatSpeed;
    }

    public float GetSpawnEarly()
    {
        return spawnEarly;
    }
}
