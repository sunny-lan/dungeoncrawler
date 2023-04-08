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
    public bool fadeInOnStart = false;

    private void Awake()
    {
        opacityController = GetComponent<CanvasGroup>();
    }

    public void Start()
    {
        opacityController.blocksRaycasts = opacityController.interactable = !initiallyFadedOut;
        opacityController.alpha = initiallyFadedOut ? 0 : 1;
        if (fadeInOnStart)
        {
            StartCoroutine(initiallyFadedOut ? FadeIn() : FadeOut());
        }
    }

    public IEnumerator FadeOut()
    {
        float time = 0;
        while (opacityController.alpha > 0)
        {
            opacityController.alpha = 1 - time / fadeInTime;
            if(opacityController.alpha < 0.5)
            {
                opacityController.blocksRaycasts = opacityController.interactable = false;
            }
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
            if(opacityController.alpha > 0.5)
            {
                opacityController.blocksRaycasts = opacityController.interactable = true;
            }
            time += Time.deltaTime;
            yield return null;
        }
    }
}
