using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class FadeInOut : MonoBehaviour
{
    CanvasGroup opacityController;
    public float fadeInTime;
    public bool initiallyFadedOut = true;

    private void Awake()
    {
        opacityController = GetComponent<CanvasGroup>();
    }

    public void Start()
    {
        opacityController.alpha = initiallyFadedOut ? 0 : 1;
    }

    public IEnumerator FadeOut()
    {
        float time = 0;
        while (opacityController.alpha > 0)
        {
            opacityController.alpha = 1 - time / fadeInTime;
            time += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        float time = 0;
        while (opacityController.alpha < 1)
        {
            opacityController.alpha = time / fadeInTime;
            time += Time.deltaTime;
            yield return null;
        }
    }
}
