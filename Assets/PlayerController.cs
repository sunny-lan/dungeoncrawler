using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : GridEntity
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    int moveCnt = 0;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        bool didMove = false;
        Vector2Int newPosition = pos;

        // Check inputs
        if (Input.GetKeyDown(KeyCode.W)) { newPosition += Vector2Int.up; }
        if (Input.GetKeyDown(KeyCode.A)) { newPosition += Vector2Int.left;}
        if (Input.GetKeyDown(KeyCode.S)) { newPosition += Vector2Int.down;}
        if (Input.GetKeyDown(KeyCode.D)) { newPosition += Vector2Int.right;}
        //if (Input.GetKeyDown(KeyCode.Q)) { transform.Rotate(0, -90, 0);}
        //if (Input.GetKeyDown(KeyCode.E)) { transform.Rotate(0, 90, 0);}

        // Check valid
        if (newPosition != pos)
        {
            if (GameManager.Instance.IsWalkable(newPosition))
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
                GameManager.Instance.DoEnemyTurn();
            }
        }
    }

}
