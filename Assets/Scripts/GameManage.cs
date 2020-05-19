using System.Collections.Generic;
using UnityEngine;

public class GameManage : MonoBehaviour {

    private const float RadiusOffset = 10;
    private const float RadiusDelta = 1.5f;
    private const float AngleDelta = 30;

    private AudioSource gameMusic;
    private readonly Queue<float> beats = new Queue<float>();

    public GameObject showTransformCarrier;
    private ShowTransform showTransform;

    /**
     * 傳入：遊戲座標（第 M 圈，第 N 格）
     * 回傳：對應的 Unity 座標（X，Y，Z=0）
     * 共有 12 圈，最內圈為第 0 圈，最外圈為第 11 圈
     * 每圈皆有 12 格，+X 方向為第 0 格，逆時針數
     */
    public static Vector3 IndexToPosition(byte m, byte n) {
        var r = RadiusOffset + m * RadiusDelta;
        var theta = n % 12 * AngleDelta;
        var x = r * Mathf.Cos(theta);
        var y = r * Mathf.Sin(theta);
        return new Vector3(x, y, 0);
    }

    private void Start() {

        gameMusic = GetComponent<AudioSource>();
        gameMusic.loop = true;
        if (!gameMusic.playOnAwake) gameMusic.Play();

        showTransform = showTransformCarrier.GetComponent<ShowTransform>();

        // 測試用，音樂完成後應替換成真實資料（單位：秒）
        for (float i = 5; i <= 60; i += 5)
            beats.Enqueue(i - 0.2f);

    }

    private void Update() {

        if (beats.Count != 0 && gameMusic.time >= beats.Peek()) {
            // 下一個拍點到了
            beats.Dequeue();
            showTransform.turnedOn = true;
        }

        if (showTransform.detectedActions.Count != 0) {
            // 處理玩家的動作
            switch (showTransform.detectedActions.Dequeue()) {
                case PlayerAction.MoveFront:
                    break;
                case PlayerAction.MoveBack:
                    break;
                case PlayerAction.MoveLeft:
                    break;
                case PlayerAction.MoveRight:
                    break;
                case PlayerAction.NoAction:
                    break;
            }
        }

    }

}
