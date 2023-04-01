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

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Vector2Int facing = GetFacingDir();

        bool didMove = false;
        Vector2Int newPosition = pos;

        // Check inputs
        if (Input.GetKeyDown(KeyCode.W)) { newPosition += facing; }
        if (Input.GetKeyDown(KeyCode.A)) { newPosition += facing.Left();}
        if (Input.GetKeyDown(KeyCode.S)) { newPosition += facing.Back();}
        if (Input.GetKeyDown(KeyCode.D)) { newPosition += facing.Right();}
        //if (Input.GetKeyDown(KeyCode.Q)) { transform.Rotate(0, -90, 0);}
        //if (Input.GetKeyDown(KeyCode.E)) { transform.Rotate(0, 90, 0);}

        // Check valid
        if (newPosition != pos)
        {
            if (gameManager.IsWalkable(newPosition))
            {
                pos = newPosition;

                transform.position = new(pos.x, 0, pos.y);
                didMove = true;
            }
        }

        // Call DoEnemyTurn
        if (didMove)
        {
            moveCnt++;
            if (moveCnt % 2 == 0)
            {
                gameManager.DoEnemyTurn();
            }
        }
    }

}
