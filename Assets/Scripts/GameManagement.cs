﻿using System.Collections;
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
    public GameObject bossAttackMarker;
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

        // Test all possible positions, can be removed
        // ShowAllPosition();

    }

    // Test all possible positions, can be removed
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
                    break;
                case PlayerAction.MoveBack:
                    newPosition = new GameCoordinate(
                        objectMap[player].M() + 1, objectMap[player].N()
                    );
                    objectMap[player] = newPosition;
                    StartCoroutine(MoveSmoothly(newPosition.ToCartesianCoordinate(height)));
                    break;
                case PlayerAction.MoveLeft:
                    newPosition = new GameCoordinate(
                        objectMap[player].M(), objectMap[player].N() - 1
                    );
                    objectMap[player] = newPosition;
                    StartCoroutine(MoveSmoothly(newPosition.ToCartesianCoordinate(height)));
                    break;
                case PlayerAction.MoveRight:
                    newPosition = new GameCoordinate(
                        objectMap[player].M(), objectMap[player].N() + 1
                    );
                    objectMap[player] = newPosition;
                    StartCoroutine(MoveSmoothly(newPosition.ToCartesianCoordinate(height)));
                    break;
                case PlayerAction.NoAction:
                    break;
            }
            Debug.Log(newPosition);
            // 敵人移動現有攻擊、對玩家造成傷害、發動新攻擊
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

        var objectMapKeys = new List<GameObject>(objectMap.Keys);
        var candidates = new List<int>();
        var numberOfNewAttack = GameCoordinate.GetSizeOfM() - objectMap[player].M() - 1;

        foreach (var key in objectMapKeys) {
            if (key == player) continue;
            var newPosition = new GameCoordinate(
                objectMap[key].M() + 1, objectMap[key].N(), true
            );
            if (newPosition.M() < GameCoordinate.GetSizeOfM()) {
                objectMap[key] = newPosition;
                key.transform.position = newPosition.ToCartesianCoordinate();
                var playerAttacked = newPosition.Equals(objectMap[player]);
                if (playerAttacked) {
                    Debug.Log("玩家被攻擊，我們還沒決定會發生什麼事");
                }
            }
            else {
                objectMap.Remove(key);
                Destroy(key);
            }
        }

        for (var i = 0; i < GameCoordinate.GetSizeOfN(); i++)
            candidates.Add(i);

        for (var i = 0; i < numberOfNewAttack; i++) {
            var index = Random.Range(0, candidates.Count);
            var attackPosition = new GameCoordinate(-3, candidates[index], true);
            var newBossAttackMarker = Instantiate(
                bossAttackMarker,
                attackPosition.ToCartesianCoordinate(),
                Quaternion.identity
            );
            objectMap.Add(newBossAttackMarker, attackPosition);
            candidates.RemoveAt(index);
            if (candidates.Count == 0) break;
        }

    }

}
