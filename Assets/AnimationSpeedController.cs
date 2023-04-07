using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationSpeedController : MonoBehaviour
{
    [SerializeField] AnimationCurve speed;
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        anim.speed = 0;
    }

    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine(play());
    }

    IEnumerator play()
    {
        var end = speed.keys.Last().time;
        var t = 0f;
        while (t <= end)
        {
            anim.speed = speed.Evaluate(t);
            yield return null;
            t += Time.deltaTime;
        }
    }
}
