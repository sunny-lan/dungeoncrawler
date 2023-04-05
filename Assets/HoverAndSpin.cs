using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAndSpin : MonoBehaviour
{
    public float hoverSpeed = 1, hoverAmt = 0.1f;
    public float rotSpeed = 0.3f;

    // Update is called once per frame
    float height = 0;
    float rotation = 0;
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, rotation, 0);
        transform.localPosition = Vector3.up * Mathf.Sin(height) * hoverAmt;
        height = (height + Time.deltaTime * hoverSpeed) % (Mathf.PI*2);
        rotation = (rotation + Time.deltaTime * rotSpeed) % 360;
    }
}
