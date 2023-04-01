using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static Vector2Int Left(this Vector2Int v)
    {
        return new Vector2Int(-v.y, v.x);
    }
    public static Vector2Int Right(this Vector2Int v)
    {
        return new Vector2Int(v.y, -v.x);
    }
    public static Vector2Int Back(this Vector2Int v)
    {
        return new Vector2Int(-v.x, -v.y);
    }
}
