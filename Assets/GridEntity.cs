using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridEntity : MonoBehaviour
{
    public UnityEvent<bool> onChangeZombieStatus;
    public bool isZombie => health <= 0;

    public UnityEvent<Vector2Int> onChangePos;
    public Vector2Int pos
    {
        get => _pos; set
        {
            _pos = value;
            onChangePos?.Invoke(_pos);
        }
    }
    public Transform raycastCenter;

    public float maxHealth = 100;
    public float initialHealth = 100;
    private float _health;
    public float health
    {
        get => _health; set
        {
            _health = Mathf.Clamp(value, -maxHealth, maxHealth);
            healthbar?.SetHP(health / maxHealth);
            onChangeHealth?.Invoke(_health);
            onChangeZombieStatus?.Invoke(isZombie);
        }
    }
    public UnityEvent<float> onChangeHealth;

    private HPBar healthbar;


    public float biteDamage = 190;
    public float biteSelfHeal = 70;


    protected GameManager gameManager { get; private set; }

    protected virtual void Awake()
    {
        healthbar = GetComponentInChildren<HPBar>();
        gameManager = FindObjectOfType<GameManager>();
        gameManager.RegisterEntity(this);
    }

    protected virtual void Start()
    {
        pos = new(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));
        transform.position = new Vector3(pos.x, 0, pos.y); // snap to grid
        health = initialHealth;
    }
    protected virtual void Update()
    {
    }

    public float bittenMaxHeatlh = -10;

    // If zombie, counts the # of bites needed before becoming a human
    public int needToBite => Mathf.CeilToInt(Mathf.Max(-health, 0) / biteSelfHeal);

    public virtual void GetBitten(GridEntity by)
    {
        Debug.Log($"{name} got bitten by {by.name}");
        Debug.Assert(!isZombie, $"{name} was a zombie but got bitten");
        Debug.Assert(by.isZombie, $"{by.name} was a not a zombie");

        // Ensure health < 0
        health = Mathf.Min(bittenMaxHeatlh, health);
    }


    public virtual void Bite(GridEntity victim)
    {
        victim.GetBitten(this);
        victim.health -= biteDamage;
        health += biteSelfHeal;
        if (needToBite <= 0)
        {
            ReviveFromZombie();
        }
    }

    public float reviveMinHealth = 50;
    private Vector2Int _pos;

    public virtual void ReviveFromZombie()
    {
        // On revive from zombie, give player some minimum health to avoid instant dying
        health = Mathf.Max(reviveMinHealth, health);
    }
}
