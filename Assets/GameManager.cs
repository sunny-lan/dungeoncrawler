using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {

            Instance = this;
        }
        else
        {
            Debug.LogError("Singleton Violation");
            Destroy(this);
        }
    }

    Dictionary<Vector2Int, bool> walkable = new ();
    private List<GridEntity> entities;
    public PlayerController player { get; private set; }

    public void RegisterEntity(GridEntity entity)
    {
        if (entities == null)
            entities = new List<GridEntity>();

        if (entity is PlayerController)
            player = entity as PlayerController;

        entities.Add(entity);
    }

    [ContextMenu("Do Enemy Telegraph")]
    public void DoEnemyTelegraph()
    {
        foreach (var entity in entities)
        {
            if (entity is EnemyController)
                (entity as EnemyController).TelegraphTurn();
        }
    }

    [ContextMenu("Do Enemy Turn")]
    public void DoEnemyTurn()
    {
        foreach (var entity in entities)
        {
            if (entity is EnemyController)
                (entity as EnemyController).DoTurn();
        }
    }

    public GridEntity GetEntityAt(Vector2Int pos)
    {
        foreach (var entity in entities)
        {
            if (entity.pos == pos)
                return entity;
        }
        return null;
    }

    public List<GridEntity> GetAllEntitiesVisibleAnd90Degrees(GridEntity origin)
    {
        GridEntity north = null;
        GridEntity south = null;
        GridEntity east = null;
        GridEntity west = null;

        foreach (var entity in entities)
        {
            if (entity == origin)
                continue;

            var delta = entity.pos - origin.pos;

            if (delta.x != 0 && delta.y != 0)
                continue;

            if (delta.x == 0)
            {
                // north or south
                if (delta.y > 0 && entity.pos.y < north.pos.y)
                {
                    north = entity;
                }
                else if (delta.y < 0 && entity.pos.y > south.pos.y)
                {
                    south = entity;
                }
            } 
            else
            {
                // east or west
                if (delta.x > 0 && entity.pos.x < east.pos.x)
                {
                    east = entity;
                }
                else if (delta.x < 0 && entity.pos.x > west.pos.x)
                {
                    west = entity;
                }
            }
        }

        var ret = new List<GridEntity>();
        if (north) ret.Add(north);
        if (south) ret.Add(south);
        if (east) ret.Add(east);
        if (west) ret.Add(west);
        return ret;
    }

    public List<GridEntity> GetAllEntitiesVisibleBy(GridEntity origin)
    {
        Physics.SyncTransforms();
        var originRaycastCenter = origin.raycastCenter.position;
        var visible = new List<GridEntity>();

        foreach (var entity in entities)
        {
            if (entity == origin || visible.Contains(entity))
                continue;
            
            if (CanEntitySee(origin, entity))
                visible.Add(entity);
        }
        return visible;
    }

    public bool CanEntitySee90Degrees(GridEntity from, GridEntity to)
    {

        if (from.pos.x != to.pos.x && from.pos.y != to.pos.y)
            return false;

        // normalize
        var delta = to.pos - from.pos;
        var dir = delta;
        dir.x = dir.x.Sign();
        dir.y = dir.y.Sign();

        Debug.Assert(dir * (int) delta.magnitude == delta);

        for (int i = 1; i < delta.magnitude; i++)
        {
            var checkPos = from.pos + dir * i;

            if (!IsWalkable(checkPos))
                return false;

            var between = GetEntityAt(from.pos + dir * i);

            if (between && between != from && between != to)
                return false;
        }

        return true;
    }
    public bool CanEntitySee(GridEntity from, GridEntity to)
    {
        var fromRaycastCenter = from.raycastCenter.position;
        var toRaycastCenter = to.raycastCenter.position;
        var hits = Physics.RaycastAll(fromRaycastCenter, toRaycastCenter - fromRaycastCenter, 1000);

        System.Array.Sort(hits, (a, b) => (a.distance.CompareTo(b.distance)));

        foreach (var hit in hits)
        {
            var hitEntity = hit.transform.GetComponent<GridEntity>();

            if (hitEntity == from)
                continue;

            if (hitEntity != null)
                return true;

            return false; // either hit a wall or another entity.
        }
        return false;
    }

    public bool IsWalkable(Vector2Int pos)
    {
        foreach (var entity in entities)
        {
            if (entity.pos == pos)
            {
                return false;
            }
        }
        return walkable.GetValueOrDefault(pos, true);
    }

    public void SetWalkable(Vector2Int pos, bool _walkable)
    {
        walkable[pos] = _walkable;
    }

    HashSet<KeyController> unfoundKeys = new();
    HashSet<KeyController> allKeys = new();
    public IReadOnlyCollection<KeyController> UnfoundKeys => unfoundKeys;
    public IReadOnlyCollection<KeyController> AllKeys => allKeys;

    public void RandomGenerateKeys()
    {
        //TODO
    }

    // Called by keys to register themselves
    public void RegisterKey(KeyController key)
    {
        unfoundKeys.Add(key);
        allKeys.Add(key);
    }

    internal void OnPlayerCollectedKey(KeyController key)
    {
        unfoundKeys.Remove(key);

        if (unfoundKeys.Count == 0)
        {
            Debug.LogWarning("TODO unlock door");
        }
    }
}
