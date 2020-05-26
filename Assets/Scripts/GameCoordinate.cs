using UnityEngine;

/**
 * 遊戲座標（第 M 圈，第 N 格）
 * 共有 6 圈，最內圈為第 0 圈，最外圈為第 5 圈
 * 每圈皆有 24 格，+X 方向為第 0 格，逆時針數
 */
public class GameCoordinate {

    private const float RadiusOffset = 10;
    private const float RadiusDelta = 3;
    private const byte SizeOfM = 6;

    private const float AngleDelta = 15;
    private const byte SizeOfN = (byte) (360 / AngleDelta);

    private readonly sbyte m;
    private readonly sbyte n;

    public GameCoordinate(int newM, int newN, bool extended = false) {
        if (extended) {
            var minM = - Mathf.FloorToInt(RadiusOffset / RadiusDelta);
            if (newM < minM) newM = minM;
        }
        else {
            if (newM > SizeOfM - 1) newM = SizeOfM - 1;
            if (newM < 0) newM = 0;
        }
        m = (sbyte) newM;
        newN %= SizeOfN;
        if (newN < 0) newN += SizeOfN;
        n = (sbyte) newN;
    }

    public GameCoordinate() {
        m = 0;
        n = 0;
    }

    public override bool Equals(object obj) {
        if (!(obj is GameCoordinate)) return false;
        var g = (GameCoordinate) obj;
        return (m == g.m && n == g.n);
    }

    public override int GetHashCode() {
        return m * 100 + n;
    }

    public override string ToString() {
        return "GameCoordinate(" + M() + ", " + N() + ")";
    }

    public int M() {
        return m;
    }

    public int N() {
        return n;
    }

    public static int GetSizeOfM() {
        return SizeOfM;
    }

    public static int GetSizeOfN() {
        return SizeOfN;
    }

    /**
     * 換算對應的 Unity 座標（X，Y=0，Z）
     */
    public Vector3 ToCartesianCoordinate(float y = 0) {
        var r = RadiusOffset + M() * RadiusDelta;
        var theta = N() * AngleDelta;
        theta = theta * Mathf.PI / 180;
        var x = r * Mathf.Cos(theta);
        var z = r * Mathf.Sin(theta);
        return new Vector3(x, y, z);
    }

}
