using System;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private const float RadiusOffset = 10;
    private const float RadiusDelta = 1.5f;
    private const float AngleDelta = 30;

    private AudioSource gameMusic;
    private readonly float[] beats = {5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60};

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
    }

    private void Update() {
        if (Array.IndexOf(beats, gameMusic.time) < 0) return;
        Debug.Log("Active Frame");
    }

}
