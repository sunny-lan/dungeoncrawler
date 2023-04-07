using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public Transform player;

    public bool fixYRotation = true;

    void Awake()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookpos = player.position;

        if (fixYRotation)
            lookpos.y = transform.position.y;

        transform.LookAt(lookpos);
    }
}
