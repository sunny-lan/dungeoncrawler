using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachToHead : MonoBehaviour
{
    public Renderer head;

    void Update()
    {
        if (head != null)
            transform.position = head.bounds.center;
    }
}
