using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyController : GridEntity
{
    Vector2Int lastMoveDir = Vector2Int.up;
    [SerializeField] [Range(0, 1)] float randomMoveChance = 0.5f;
    public void DoTurn()
    {
        Move();
    }

    private void Move()
    {
        var visible = GameManager.Instance.GetAllEntitiesVisibleBy(this);
        bool seesZombie = false;
        Vector2Int nearestZombiePos = pos;
        float distToNearestZombie = Mathf.Infinity;

        foreach (var entity in visible)
        {
            if (entity.isZombie)
            {
                seesZombie = true;
                if (Vector2Int.Distance(pos, entity.pos) < distToNearestZombie)
                {
                    nearestZombiePos = entity.pos;
                    distToNearestZombie = Vector2Int.Distance(pos, entity.pos);
                }
            }
        }

        if (seesZombie)
        {
            var dirs = new List<Vector2Int>()
            {
                Vector2Int.up,
                Vector2Int.left,
                Vector2Int.down,
                Vector2Int.right
            };


            dirs = dirs.OrderByDescending(x => Vector2Int.Distance(x + pos, nearestZombiePos)).ToList();

            foreach (var dir in dirs)
            {
                if (GameManager.Instance.IsWalkable(pos + dir))
                {
                    pos = pos + dir;
                    transform.position = new Vector3(pos.x, 0, pos.y);
                    lastMoveDir = dir;
                    return;
                }
            }
        }
        else
        {
            MoveRandomly();
        }
    }

    private void MoveRandomly()
    {
        // more likely to continue moving in the same direction it was going
        Vector2Int delta = lastMoveDir;

        if (!GameManager.Instance.IsWalkable(pos + delta) || Random.value < randomMoveChance)
        {
            var dirs = new List<Vector2Int>()
            {
                Vector2Int.up,
                Vector2Int.left,
                Vector2Int.down,
                Vector2Int.right
            };

            // pick random direction that we haven't tried yet
            for (int i = 0; i < 4; i++)
            {
                delta = dirs[Random.Range(0, dirs.Count)];
                if (GameManager.Instance.IsWalkable(pos + delta))
                {
                    break;
                }
                else
                {
                    dirs.Remove(delta);
                }
            }
        }


        pos = pos + delta;
        transform.position = new Vector3(pos.x, 0, pos.y);
        lastMoveDir = delta;
    }
}
