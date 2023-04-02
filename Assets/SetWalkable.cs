using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Helper script for building levels that automatically sets IsWalkable 
public class SetWalkable : MonoBehaviour
{
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        // Find area this object covers, and set iswalkable to false
        var center = transform.position;
        var size = transform.lossyScale;
        int stx = Mathf.CeilToInt(center.x - size.x / 2);
        int sty = Mathf.CeilToInt(center.z - size.z / 2);
        int edx = Mathf.FloorToInt(center.x + size.x / 2);
        int edy = Mathf.FloorToInt(center.z + size.z / 2);

        for (int x = stx; x <= edx; x++)
            for (int y = sty; y <= edy; y++)
            {
                gameManager.SetWalkable(new(x, y), false);
            }
    }
}