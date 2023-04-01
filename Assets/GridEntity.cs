using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEntity : MonoBehaviour
{
    protected bool isZombie;
    public Vector2Int pos;

    public virtual bool GetIsZombie()
    {
        return isZombie;
    }

    public virtual void SetIsZombie()
    {
        isZombie = true;
    }

    protected virtual void Start()
    {
    }
    protected virtual void Update()
    {
    }
}
