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

    public static void SetEnabled(this MonoBehaviour c, bool enable)
    {
        c.enabled = enable;
    }

    public static int Manhattan(this Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x-b.x)+Mathf.Abs(a.y-b.y);
    }

    public static int Sign(this int i)
    {
        if (i == 0)
        {
            return 0;
        }
        else if (i > 0)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
}
