using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelBtnController : MonoBehaviour
{
    public LevelSelector.Level level;

    [SerializeField] TMP_Text label;
    internal FadeInOut fadeOut;

    private void Start()
    {
        label.text = level.name;
    }

    public void OnClick()
    {
        IEnumerator tmp()
        {
            yield return fadeOut.FadeOut();
            SceneManager.LoadScene(level.scene);
        }
        StartCoroutine(tmp());
    }
}
