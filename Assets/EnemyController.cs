using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyController : GridEntity
{
    Vector2Int lastMoveDir = Vector2Int.up;
    [SerializeField] [Range(0, 1)] float randomMoveChance = 0.5f;
    [SerializeField] EnemyTelegraphController telegraphController;

    [SerializeField] GameObject zombieModel;
    [SerializeField] GameObject guardModel;

    protected override void Awake()
    {
        base.Awake();

        // TODO outline not working for zombie
        onChangeZombieStatus.AddListener(isZombie =>
        {
            zombieModel.SetActive(isZombie);
            guardModel.SetActive(!isZombie);
        });
    }

    public void TelegraphTurn()
    {
        if (isZombie)
        {
            if (!TelegraphAttack())
            {
                // move toward humans
                TelegraphMove(targetIsZombie: false, moveTowardTarget: true);
            }
        }
        else
        {
            // UNDONE guard ranged attack
            // move away from zombies
            TelegraphMove(targetIsZombie: true, moveTowardTarget: false);
        }
    }

    public void DoTurn()
    {
        var entity = gameManager.GetEntityAt(pos + telegraphController.GetDirection());
        if (telegraphController.GetTelegraphType() == EnemyTelegraphController.TelgraphType.MOVE)
        {
            if (entity == null)
            {
                DoMove(telegraphController.GetDirection());  
            }
            else if(isZombie && !entity.isZombie) // UNDONE guard ranged attack
            {
                entity.GetBitten(this);
            }
        }
        else
        { 
            if (entity && isZombie && !entity.isZombie) // UNDONE guard ranged attack
            {
                entity.GetBitten(this);
            }
        }

        telegraphController.ClearTelegraph();
    }

    private void DoMove(Vector2Int delta)
    {
        pos = pos + delta;
        transform.position = new Vector3(pos.x, 0, pos.y);
        lastMoveDir = delta;
    }

    private bool TelegraphAttack()
    {
        // attack enemy in random direction, returns true if I can attack anyone, false otherwise
        Vector2Int delta = lastMoveDir;

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
            var entity = gameManager.GetEntityAt(pos + delta);
            if (entity == null)
            {
                dirs.Remove(delta);
            }
            else if (isZombie && !entity.isZombie) // UNDONE guard ranged attack
            {
               telegraphController.UpdateTelegraph(EnemyTelegraphController.TelgraphType.ATTACK, delta);   
               return true;
            }
        }

        return false;
    }

    private bool TelegraphMove(bool targetIsZombie, bool moveTowardTarget)
    {
        var visible = gameManager.GetAllEntitiesVisibleBy(this);
        bool seesTarget = false;
        Vector2Int nearestTargetPos = pos;
        float distToNearestTarget = Mathf.Infinity;

        foreach (var entity in visible)
        {
            if (entity.isZombie == targetIsZombie) // I see a target
            {
                Debug.DrawLine(raycastCenter.position, entity.raycastCenter.position, Color.red, 0.5f);
                seesTarget = true;
                var distToTarget = Vector2Int.Distance(pos, entity.pos);
                if (distToTarget < distToNearestTarget)
                {
                    nearestTargetPos = entity.pos;
                    distToNearestTarget = distToTarget;
                }
            }
        }

        if (seesTarget)
        {
            var dirs = new List<Vector2Int>()
            {
                Vector2Int.up,
                Vector2Int.left,
                Vector2Int.down,
                Vector2Int.right
            };


            if (moveTowardTarget)
            {
                dirs = dirs.OrderBy(x => Vector2Int.Distance(x + pos, nearestTargetPos)).ToList();
            }
            else
            {
                dirs = dirs.OrderByDescending(x => Vector2Int.Distance(x + pos, nearestTargetPos)).ToList();
            }

            foreach (var dir in dirs)
            {
                if (gameManager.IsWalkable(pos + dir))
                {
                    telegraphController.UpdateTelegraph(EnemyTelegraphController.TelgraphType.MOVE, dir);
                    return true;
                }
            }
        }

        return TelegraphRandomMove();
    }


    private bool TelegraphRandomMove()
    {
        // more likely to continue moving in the same direction it was going
        Vector2Int delta = lastMoveDir;

        if (!gameManager.IsWalkable(pos + delta) || Random.value < randomMoveChance)
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
                if (gameManager.IsWalkable(pos + delta))
                {
                    break;
                }
                else
                {
                    dirs.Remove(delta);
                }
            }
        }

        telegraphController.UpdateTelegraph(EnemyTelegraphController.TelgraphType.MOVE, delta);
        return true;
    }

    public override void GetBitten(GridEntity by)
    {
        base.GetBitten(by);
        TelegraphTurn(); // TODO Stun when zombified
    }
}
