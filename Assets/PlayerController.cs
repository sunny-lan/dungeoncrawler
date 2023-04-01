using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : GridEntity
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Debug.Log("Start Parent");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) transform.position += Vector3.forward;
        if (Input.GetKeyDown(KeyCode.A)) transform.position += Vector3.left;
        if (Input.GetKeyDown(KeyCode.S)) transform.position += Vector3.back;
        if (Input.GetKeyDown(KeyCode.D)) transform.position += Vector3.right;
        if (Input.GetKeyDown(KeyCode.Q)) transform.Rotate(0, -90, 0);
        if (Input.GetKeyDown(KeyCode.E)) transform.Rotate(0, 90, 0);

        // Call DoEnemyTurn
    }

}
