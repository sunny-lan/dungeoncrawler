using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PulseImageColor : MonoBehaviour
{
    private RawImage image;
    public float maxAlpha = 0.5f;
    public float onSpeed = 5;
    public float offSpeed = 3;
    public Color color = Color.red;

    private void Awake()
    {
        image = GetComponent<RawImage>();
    }

    IEnumerator Pulse(float strength)
    {
        strength = Mathf.Clamp01(strength);
        for(float alpha = 0; alpha <= strength*maxAlpha; alpha += onSpeed * Time.deltaTime)
        {
            var c = color;
            c.a = alpha;
            image.color = c;
            yield return null;
        }

        for (float alpha = strength* maxAlpha; alpha>=0; alpha -= offSpeed * Time.deltaTime)
        {
            var c = color;
            c.a = alpha;
            image.color = c;
            yield return null;
        }
    }

    public void Trigger(float strength=1)
    {
        StopAllCoroutines();
        StartCoroutine(Pulse(strength));
    }
}
