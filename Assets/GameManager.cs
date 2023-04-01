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

    [SerializeField] private List<EnemyController> enemies;
    bool[,] walls;

    public void RegisterEnemy(EnemyController enemy)
    {
        if (enemies == null)
            enemies = new List<EnemyController>();

        enemies.Add(enemy);
    }

    public void DoEnemyTurn()
    {
        foreach (var enemy in enemies)
        {
            enemy.DoTurn();
        }
    }

    public GridEntity GetObjectAt(Vector2Int pos)
    {
        // return null if unoccupied
        // return the gameobject if it's there
        return null;
    }

    public bool IsWalkable(Vector2Int pos)
    {
        return true;
    }
}
