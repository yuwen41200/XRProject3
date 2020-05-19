public class GameCoordinate {

    private readonly byte m;
    private readonly byte n;

    public GameCoordinate(int newM, int newN) {
        if (newM > 11) m = 11;
        if (newM < 0) m = 0;
        m = (byte) newM;
        newN %= 12;
        if (newN < 0) newN += 12;
        n = (byte) newN;
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

    public int M() {
        return m;
    }

    public int N() {
        return n;
    }

}
