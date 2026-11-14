public class DepthMap
{
    private readonly int [,] _distance;

    public int MaxDistance { get; }

    public DepthMap(int[,] distance, int maxDistance)
    {
        _distance = distance;
        MaxDistance = maxDistance;
    }

    public int GetDistance(int x, int y)
    {
        if(x < 0 || y < 0 || x >= _distance.GetLength(0) || y >= _distance.GetLength(1)) return -1;
        return _distance[x, y];
    }

    public float GetDepthOne(int x, int y)
    {
        int d = GetDistance(x, y);
        if(d < 0) return -1f;
        return d / (float)Math.Max(1, MaxDistance);
    }
}