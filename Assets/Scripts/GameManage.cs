using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GameManage : MonoBehaviour {

    private const float RadiusOffset = 10;
    private const float RadiusDelta = 1.5f;
    private const float AngleDelta = 30;

    private AudioSource gameMusic;
    private readonly Queue<float> beats = new Queue<float>();

    public GameObject showTransformCarrier;
    private ShowTransform showTransform;

    public GameObject sampleLightCarrier;
    private Light sampleLight;
    private readonly Color[] colorsForBeats = {Color.yellow, Color.cyan, Color.magenta};
    private int colorsIndex;

    public GameObject player;
    public GameObject bossAttack1;
    private readonly Dictionary<GameObject, GameCoordinate> objectMap =
        new Dictionary<GameObject, GameCoordinate>();

    //test all possible positions, can be removed
    public GameObject testg;

    //張文胤
    [SerializeField]
    float moveSpeed;

    /**
     * 傳入：遊戲座標（第 M 圈，第 N 格）
     * 回傳：對應的 Unity 座標（X，Y=0，Z）
     * 共有 12 圈，最內圈為第 0 圈，最外圈為第 11 圈
     * 每圈皆有 12 格，+X 方向為第 0 格，逆時針數
     */
    private static Vector3 GameCoordToCartesianCoord(GameCoordinate g) {
        var r = RadiusOffset + g.M() * RadiusDelta;
        var theta = g.N() % 12 * AngleDelta;
        theta = theta * Mathf.PI / 180;
        var x = r * Mathf.Cos(theta);
        var z = r * Mathf.Sin(theta);
        return new Vector3(x, 0, z);
    }

    private void Start() {

        gameMusic = GetComponent<AudioSource>();
        gameMusic.loop = true;
        if (!gameMusic.playOnAwake) gameMusic.Play();

        showTransform = showTransformCarrier.GetComponent<ShowTransform>();
        sampleLight = sampleLightCarrier.GetComponent<Light>();

        // 測試用，音樂完成後應替換成真實資料（單位：秒）// test, change 5 to 2, change 60 to 200
        for (float i = 5; i <= 200; i += 2)
            beats.Enqueue(i - 0.2f);

        var initialPosition = new GameCoordinate(11, 0);
        objectMap.Add(player, initialPosition);
        player.transform.position = GameCoordToCartesianCoord(initialPosition);

        initialPosition = new GameCoordinate(5, 6);
        objectMap.Add(bossAttack1, initialPosition);
        bossAttack1.transform.position = GameCoordToCartesianCoord(initialPosition);

        ShowAllPosition();
    }

    //test all possible position
    private void ShowAllPosition()
    {
        for (int i = 0; i < 12; ++i)
        {
            for (int j = 0; j < 12; ++j)
            {
                var tmpP = new GameCoordinate(i,j);
                Instantiate(testg, GameCoordToCartesianCoord(tmpP),Quaternion.identity);
            }
        }
    }

    private void FixedUpdate() {

        // 讓玩家的鏡頭前方永遠朝向中心點，好判斷跟計算
        player.transform.LookAt(Vector3.zero);
        
        if (beats.Count != 0 && gameMusic.time >= beats.Peek()) {
            // 下一個拍點到了
            beats.Dequeue();
            // 啟動玩家動作偵測
            showTransform.turnedOn = true;
            // 變換燈光顏色（示範）
            sampleLight.color = colorsForBeats[colorsIndex];
            colorsIndex = (colorsIndex + 1) % 3;
        }

        if (showTransform.detectedActions.Count != 0) {
            // 處理玩家的動作

            GameCoordinate newPosition;
            switch (showTransform.detectedActions.Dequeue()) {
                case PlayerAction.MoveFront:
                    newPosition = new GameCoordinate(
                        objectMap[player].M() - 1, objectMap[player].N()
                    );
                    objectMap[player] = newPosition;
                    StartCoroutine(MoveSmoothly(GameCoordToCartesianCoord(newPosition)));
                    //改成 Coroutine的方式
                    //player.transform.position = GameCoordToCartesianCoord(newPosition);
                    break;
                case PlayerAction.MoveBack:
                    newPosition = new GameCoordinate(
                        objectMap[player].M() + 1, objectMap[player].N()
                    );
                    objectMap[player] = newPosition;
                    StartCoroutine(MoveSmoothly(GameCoordToCartesianCoord(newPosition)));
                    //player.transform.position = GameCoordToCartesianCoord(newPosition);
                    break;
                case PlayerAction.MoveLeft:
                    newPosition = new GameCoordinate(
                        objectMap[player].M(), objectMap[player].N() - 1
                    );
                    objectMap[player] = newPosition;
                    StartCoroutine(MoveSmoothly(GameCoordToCartesianCoord(newPosition)));
                    //player.transform.position = GameCoordToCartesianCoord(newPosition);
                    break;
                case PlayerAction.MoveRight:
                    newPosition = new GameCoordinate(
                        objectMap[player].M(), objectMap[player].N() + 1
                    );
                    objectMap[player] = newPosition;
                    StartCoroutine(MoveSmoothly(GameCoordToCartesianCoord(newPosition)));
                    //player.transform.position = GameCoordToCartesianCoord(newPosition);
                    break;
                case PlayerAction.NoAction:
                    break;
            }
            BossAttack();
        }

    }

    private IEnumerator MoveSmoothly(Vector3 targetPosition)
    {
        float dist = Vector3.Distance(player.transform.position, targetPosition);
        Vector3 dir = (targetPosition - player.transform.position);
        //Debug.Log(dir);
        while (dist > 0.5f)
        {
            player.transform.Translate(dir * moveSpeed, Space.World);
            dist = Vector3.Distance(player.transform.position, targetPosition);
            yield return null;
        }
        yield return null;
    }

    private void BossAttack() {
        var attacked = objectMap[bossAttack1].Equals(objectMap[player]);
        if (attacked) {
            // 玩家被攻擊，我們還沒決定會發生什麼事
        }
    }

}
