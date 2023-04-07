using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButtons : MonoBehaviour
{
    [SerializeField] FadeInOut blackOut;

    public void OnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnLevelSelect()
    {
        IEnumerator tmp()
        {
            yield return blackOut.FadeIn();

            SceneManager.LoadScene("Level Select");
        }
        StartCoroutine(tmp());
    }
}
