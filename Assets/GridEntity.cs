using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEntity : MonoBehaviour
{
    protected bool isZombie;
    public Vector2Int pos;
    public Transform raycastCenter;

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
        GameManager.Instance.RegisterEntity(this);
        pos.x = Mathf.FloorToInt(transform.position.x);
        pos.y = Mathf.FloorToInt(transform.position.z);
    }
    protected virtual void Update()
    {
    }
}
