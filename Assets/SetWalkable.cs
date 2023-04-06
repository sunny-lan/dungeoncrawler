using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Helper script for building levels that automatically sets IsWalkable 
public class SetWalkable : MonoBehaviour
{
    GameManager gameManager;

    public bool walkable = false;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        // Find area this object covers, and set iswalkable to false
        var center = transform.position;
        var size = transform.localScale;

        if(transform.rotation.eulerAngles.y is (>85 and <95) or (>265 and <275))
        {
            size = new(size.z, size.y, size.x);
        }

        int stx = Mathf.CeilToInt(center.x - size.x / 2);
        int sty = Mathf.CeilToInt(center.z - size.z / 2);
        int edx = Mathf.FloorToInt(center.x + size.x / 2);
        int edy = Mathf.FloorToInt(center.z + size.z / 2);

        for (int x = stx; x <= edx; x++)
            for (int y = sty; y <= edy; y++)
            {
                gameManager.SetWalkable(new(x, y), walkable);
            }
    }
}
