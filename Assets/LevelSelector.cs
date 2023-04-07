using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    [Serializable]
    public struct Level
    {
        public string name;
        public string scene;
    }

    public List<Level> levels=new();

    [SerializeField] LevelBtnController levelBtnPrefab;
    [SerializeField] FadeInOut fadeOut;

    void Start()
    {
        // Add levels as children
        foreach (Level level in levels)
        {
            LevelBtnController levelBtnController = Instantiate(levelBtnPrefab, transform);
            levelBtnController.level = level;
            levelBtnController.fadeOut = fadeOut;
        }
    }
}
