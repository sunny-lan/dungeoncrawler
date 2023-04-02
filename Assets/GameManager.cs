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
    [SerializeField] private List<GridEntity> entities;
    [SerializeField] private PlayerController player;

    public void RegisterEntity(GridEntity entity)
    {
        if (entities == null)
            entities = new List<GridEntity>();

        entities.Add(entity);
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

    public List<GridEntity> GetAllEntitiesVisibleBy(GridEntity origin)
    {
        Physics.SyncTransforms();
        var originRaycastCenter = origin.raycastCenter.position;
        var found = new List<GridEntity>();

        foreach (var entity in entities)
        {
            if (entity == origin || found.Contains(entity))
                continue;

            var targetRaycastCenter = entity.raycastCenter.position;

            var hits = Physics.RaycastAll(originRaycastCenter, targetRaycastCenter - originRaycastCenter, 1000);

            System.Array.Sort(hits, (a, b) => (a.distance.CompareTo(b.distance)));

            foreach (var hit in hits)
            {
                var hitEntity = hit.transform.GetComponent<GridEntity>();

                if (hitEntity == origin)
                    continue;

                if (hitEntity != null)
                    found.Add(hitEntity);

                break; // either hit a wall or an entity.
            }
        }
        return found;
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
}
