using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour {

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

    // Test all possible positions, can be removed
    public GameObject testG;

    // 張文胤
    [SerializeField] private float moveSpeed;
    [SerializeField] private float height;

    private void Start() {

        gameMusic = GetComponent<AudioSource>();
        gameMusic.loop = true;
        if (!gameMusic.playOnAwake) gameMusic.Play();

        showTransform = showTransformCarrier.GetComponent<ShowTransform>();
        sampleLight = sampleLightCarrier.GetComponent<Light>();

        // 測試用，音樂完成後應替換成真實資料（單位：秒）// Test, change 5 to 2, change 60 to 200
        for (float i = 5; i <= 200; i += 2)
            beats.Enqueue(i - 0.2f);

        var initialPosition = new GameCoordinate(5, 0);
        objectMap.Add(player, initialPosition);
        player.transform.position = initialPosition.ToCartesianCoordinate(height);
        player.transform.LookAt(Vector3.zero);

        initialPosition = new GameCoordinate(5, 6);
        objectMap.Add(bossAttack1, initialPosition);
        bossAttack1.transform.position = initialPosition.ToCartesianCoordinate();

        ShowAllPosition();
    }

    // Test all possible position
    private void ShowAllPosition() {
        for (var i = 0; i < 6; ++i) {
            for (var j = 0; j < 24; ++j) {
                var tmpP = new GameCoordinate(i, j);
                Instantiate(testG, tmpP.ToCartesianCoordinate(), Quaternion.identity);
            }
        }
    }

    private void FixedUpdate() {

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
            GameCoordinate newPosition = null;
            switch (showTransform.detectedActions.Dequeue()) {
                case PlayerAction.MoveFront:
                    newPosition = new GameCoordinate(
                        objectMap[player].M() - 1, objectMap[player].N()
                    );
                    objectMap[player] = newPosition;
                    StartCoroutine(MoveSmoothly(newPosition.ToCartesianCoordinate(height)));
                    // 改成 Coroutine 的方式
                    // player.transform.position = GameCoordToCartesianCoord(newPosition);
                    break;
                case PlayerAction.MoveBack:
                    newPosition = new GameCoordinate(
                        objectMap[player].M() + 1, objectMap[player].N()
                    );
                    objectMap[player] = newPosition;
                    StartCoroutine(MoveSmoothly(newPosition.ToCartesianCoordinate(height)));
                    // player.transform.position = GameCoordToCartesianCoord(newPosition);
                    break;
                case PlayerAction.MoveLeft:
                    newPosition = new GameCoordinate(
                        objectMap[player].M(), objectMap[player].N() - 1
                    );
                    objectMap[player] = newPosition;
                    StartCoroutine(MoveSmoothly(newPosition.ToCartesianCoordinate(height)));
                    // player.transform.position = GameCoordToCartesianCoord(newPosition);
                    break;
                case PlayerAction.MoveRight:
                    newPosition = new GameCoordinate(
                        objectMap[player].M(), objectMap[player].N() + 1
                    );
                    objectMap[player] = newPosition;
                    StartCoroutine(MoveSmoothly(newPosition.ToCartesianCoordinate(height)));
                    // player.transform.position = GameCoordToCartesianCoord(newPosition);
                    break;
                case PlayerAction.NoAction:
                    break;
            }
            Debug.Log(newPosition);
            BossAttack();
        }

    }

    private IEnumerator MoveSmoothly(Vector3 destination) {
        while (Vector3.Distance(player.transform.position, destination) > moveSpeed) {
            var direction = Vector3.Normalize(destination - player.transform.position);
            player.transform.Translate(direction * moveSpeed, Space.World);
            // 讓玩家的鏡頭前方永遠朝向中心點，好判斷跟計算
            player.transform.LookAt(Vector3.zero);
            yield return null;
        }
        player.transform.position = destination;
        player.transform.LookAt(Vector3.zero);
    }

    private void BossAttack() {
        var attacked = objectMap[bossAttack1].Equals(objectMap[player]);
        if (attacked) {
            // 玩家被攻擊，我們還沒決定會發生什麼事
        }
    }

}
