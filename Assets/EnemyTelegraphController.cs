using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTelegraphController : MonoBehaviour
{
    public enum TelgraphType { MOVE, ATTACK };
    private TelgraphType type;
    private Vector2Int direction;

    [SerializeField] private Transform indicatorPivot;

    public void UpdateTelegraph(TelgraphType type, Vector2Int direction)
    {
        this.type = type;
        this.direction = direction;

        indicatorPivot.gameObject.SetActive(true);
        transform.forward = new Vector3(direction.x, 0, direction.y);
    }

    public void ClearTelegraph()
    {
        indicatorPivot.gameObject.SetActive(false);
    }

    public TelgraphType GetTelegraphType()
    {
        return type;
    }

    public Vector2Int GetDirection()
    {
        return direction;
    }
}
