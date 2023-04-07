using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButtons : MonoBehaviour
{
    [SerializeField] FadeInOut blackOut;

    public void OnRestart()
    {
        StartCoroutine(fadeAndLoad(SceneManager.GetActiveScene().name));
    }

    IEnumerator fadeAndLoad(string scene)
    {
        yield return blackOut.FadeIn();

        SceneManager.LoadScene(scene);
    }

    public void OnLevelSelect()
    {
        StartCoroutine(fadeAndLoad("Level Select"));
    }
}
