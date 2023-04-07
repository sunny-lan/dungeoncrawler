using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    private GameManager gameManager;
    public bool collectibleInZombieMode = false;

    [SerializeField] MeshRenderer keyRenderer;

    public Color activeColor, inactiveColor;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.RegisterKey(this);
    }

    private void Start()
    {
        gameManager.player.onChangePos.AddListener(OnPlayerChangePos);
        gameManager.player.onChangeZombieStatus.AddListener(OnPlayerChangeZombie);
        OnPlayerChangeZombie(gameManager.player.isZombie);
    }

    private void OnPlayerChangeZombie(bool isZombie)
    {
        keyRenderer.material.color = (collectibleInZombieMode || !isZombie) switch
        {
            false => inactiveColor,
            true => activeColor,
        };
    }

    private void OnPlayerChangePos(Vector2Int keyPos)
    {
        Vector2Int myPos = new(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));
        if (myPos == keyPos && (collectibleInZombieMode || !gameManager.player.isZombie))
        {
            gameManager.OnPlayerCollectedKey(this);
            gameManager.player.OnCollectedKey(this);
            Destroy(gameObject);
        }
    }
}
