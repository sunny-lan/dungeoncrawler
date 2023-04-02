using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : GridEntity
{
    private GameManager gameManager;
    

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        gameManager = FindObjectOfType<GameManager>();
    }

    int moveCnt = 0;

    Vector2Int[] dirs =
    {
        Vector2Int.up,
        Vector2Int.left,
        Vector2Int.down,
        Vector2Int.right
    };

    Vector2Int GetFacingDir()
    {
        Vector2Int best = default;
        Vector2 forward2d = new(transform.forward.x, transform.forward.z);
        float max = float.NegativeInfinity;
        foreach (var dir in dirs)
        {
            var dot = Vector2.Dot(dir, forward2d);
            if (dot > max)
            {
                max = dot;
                best = dir;
            }
        }

        return best;
    }

    void TryMove(Vector2Int newPosition)
    {
        if (gameManager.IsWalkable(newPosition))
        {
            pos = newPosition;

            transform.position = new(pos.x, 0, pos.y);
            OnDidMove();
        }
    }

    void OnDidMove()
    {
        moveCnt++;
        if (moveCnt % 2 == 0)
        {
            gameManager.DoEnemyTurn();
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();



        Vector2Int facing = GetFacingDir();

        // Check inputs
        if (Input.GetKeyDown(KeyCode.W))  TryMove( pos + facing); 
        else if (Input.GetKeyDown(KeyCode.A))  TryMove( pos + facing.Left());
        else if (Input.GetKeyDown(KeyCode.S))  TryMove( pos + facing.Back());
        else if (Input.GetKeyDown(KeyCode.D))  TryMove( pos + facing.Right());


    }

    public int bitesToHuman = 3;

    int needToBite;

    public void GetBitten()
    {
        needToBite = 3;

    }

}
