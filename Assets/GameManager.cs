using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    Dictionary<Vector2Int, bool> walkable = new ();
    private List<GridEntity> entities;
    public PlayerController player { get; private set; }

    [Header("spawning")]
    [SerializeField] BoundsInt[] mapBounds; // ignores y
    [SerializeField] int numKeys;
    [SerializeField] int numEnemies;
    [SerializeField] float keySpawnMinDistanceFromPlayer;
    [SerializeField] float enemySpawnMinDistanceFromPlayer;
    [SerializeField] GameObject keyPrefab;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] ExitController exitController;

    public float walkableVisualizerHeight;

    private WinLoseScreenController winLoseScreenController;
    private void Awake()
    {
        winLoseScreenController = FindObjectOfType<WinLoseScreenController>();
    }

    public void HandlePlayerWinLose(bool win)
    {
        winLoseScreenController.Show(win);
        player.enabled = false;
    }

    public void RegisterEntity(GridEntity entity)
    {
        if (entities == null)
            entities = new List<GridEntity>();

        if (entity is PlayerController)
        {
            player = entity as PlayerController;
        }

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

        Debug.Assert(dir * (int)delta.magnitude == delta);

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
        var hits = Physics.RaycastAll(fromRaycastCenter, toRaycastCenter - fromRaycastCenter, 100);

        var hitsSorted = hits.OrderBy(x => x.distance).ToList();

        foreach (var hit in hitsSorted)
        {
            var hitEntity = hit.transform.GetComponent<GridEntity>();

            if (hitEntity == to)
            {
                return true;
            }

            if (hitEntity == null || hitEntity != from)
            {
                return false;
            }
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

    HashSet<KeyController> unfoundKeys = new ();
    HashSet<KeyController> allKeys = new ();
    public IReadOnlyCollection<KeyController> UnfoundKeys => unfoundKeys;
    public IReadOnlyCollection<KeyController> AllKeys => allKeys;

    private void Start()
    {
        if (mapBounds.Length == 0)
        {
            Debug.LogError("mapBounds is empty. Add bounding boxes for the possible spawn zones of the enemies/keys. Each bounding box has the same probability of being chosen regardless of size. y is ignored.");
            return;
        }
        RandomGenerateKeys();
        RandomGenerateEnemies();
    }

    public void RandomGenerateKeys()
    {
        var occupied = new HashSet<Vector2Int>();
        for (int i = 0; i < numKeys; i++)   
        {
            Vector2Int pos;
            int failsafe = 100;
            do
            {
                failsafe--;
                if (failsafe == 0)
                {
                    Debug.LogError("Failed to spawn key! Aborting...");
                    return;
                }
                var bound = mapBounds[Random.Range(0, mapBounds.Length)];
                pos = new Vector2Int(Random.Range(bound.xMin, bound.xMax + 1),
                                     Random.Range(bound.zMin, bound.zMax + 1));
            } while (!walkable.GetValueOrDefault(pos, true) ||
                     occupied.Contains(pos) ||
                     Vector2Int.Distance(pos, player.pos) < keySpawnMinDistanceFromPlayer);

            Instantiate(keyPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
            occupied.Add(pos);
        }
    }

    public void RandomGenerateEnemies()
    {
        // can't realy on enemy registering themselves because after I instantiate them I need to wait a frame for Awake() to trigger.
        // I'm too lazy to fix it so I'm just going to keep track of my own occupied hashset :p
        var occupied = new HashSet<Vector2Int>();
        for (int i = 0; i < numEnemies; i++)
        {
            Vector2Int pos;
            int failsafe = 100;
            do
            {
                failsafe--;
                if (failsafe == 0)
                {
                    Debug.LogError("Failed to spawn enemy! Aborting...");
                    return;
                }
                var bound = mapBounds[Random.Range(0, mapBounds.Length)];
                pos = new Vector2Int(Random.Range(bound.xMin, bound.xMax + 1),
                                     Random.Range(bound.zMin, bound.zMax + 1));
            } while (!walkable.GetValueOrDefault(pos, true) ||
                     occupied.Contains(pos) ||
                     Vector2Int.Distance(pos, player.pos) < enemySpawnMinDistanceFromPlayer);

            Instantiate(enemyPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
            occupied.Add(pos);
        }
    }

    // Called by keys to register themselves
    public void RegisterKey(KeyController key)
    {
        unfoundKeys.Add(key);
        allKeys.Add(key);
        exitController.UpdateKeyCount(0, AllKeys.Count);
    }

    internal void OnPlayerCollectedKey(KeyController key)
    {
        unfoundKeys.Remove(key);
        exitController.UpdateKeyCount(AllKeys.Count - unfoundKeys.Count, AllKeys.Count);

        if (unfoundKeys.Count == 0)
        {
            exitController.Unlock();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        foreach (var bound in mapBounds)
        {
            Gizmos.DrawWireCube(bound.center, bound.size);
        }

        if (player)
        {
            Gizmos.DrawWireSphere(new Vector3(player.pos.x, 0, player.pos.y), keySpawnMinDistanceFromPlayer);
            Gizmos.DrawWireSphere(new Vector3(player.pos.x, 0, player.pos.y), enemySpawnMinDistanceFromPlayer);
        }

        if (walkable == null)
            return;

        Gizmos.color = Color.red;
        foreach (var w in walkable)
        {
            if (!w.Value)
            {
                Gizmos.DrawCube(new Vector3(w.Key.x, walkableVisualizerHeight, w.Key.y), new Vector3(1f, 0.1f, 1f));
            }
        }
    }
}
