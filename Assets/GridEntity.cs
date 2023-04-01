using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEntity : MonoBehaviour
{
    bool isZombie;
    Vector2Int pos;

    protected virtual void Start()
    {
        Debug.Log("Start Base");
    }
}
