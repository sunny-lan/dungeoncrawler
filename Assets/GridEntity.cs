using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridEntity : MonoBehaviour
{
    public UnityEvent<bool> onChangeZombieStatus;
    public bool isZombieInitial = false;
    bool isInit = true;
    public bool isZombie
    {
        get => _isZombie; set
        {
            if (value != _isZombie || isInit)
            {
                _isZombie = value;
                if (_isZombie)
                    needToBite = bitesToHuman;
                onChangeZombieStatus?.Invoke(_isZombie);
            }
        }
    }

    public Vector2Int pos;
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
            isZombie = _health <= 0;
            onChangeHealth?.Invoke(_health);
        }
    }
    public UnityEvent<float> onChangeHealth;

    private HPBar healthbar;


    public float biteDamage = 190;
    public float biteSelfHeal = 70;

    private bool _isZombie;

    protected GameManager gameManager { get; private set; }

    protected virtual void Awake() {
        healthbar = GetComponentInChildren<HPBar>();
        gameManager = FindObjectOfType<GameManager>();
    }

    protected virtual void Start()
    {
        gameManager.RegisterEntity(this);
        pos.x = Mathf.FloorToInt(transform.position.x);
        pos.y = Mathf.FloorToInt(transform.position.z);
        transform.position = new Vector3(pos.x, 0, pos.y); // snap to grid
        health = initialHealth;
        isZombie = isZombieInitial;
        needToBite = bitesToHuman;

        isInit = false;
    }
    protected virtual void Update()
    {
    }

    public float bittenMaxHeatlh = -10;
    public int bitesToHuman = 3;

    // If zombie, counts the # of bites needed before becoming a human
    public int needToBite { get; private set; }

    public virtual void GetBitten(GridEntity by)
    {
        Debug.Log($"{name} got bitten by {by.name}");
        Debug.Assert(!isZombie, $"{name} was a zombie but got bitten");
        Debug.Assert(by.isZombie, $"{by.name} was a not a zombie");

        health = Mathf.Min(health, bittenMaxHeatlh);
        needToBite = bitesToHuman;
        isZombie = true;
    }


    public virtual void Bite(GridEntity victim)
    {
        victim.GetBitten(this);
        victim.health -= biteDamage;
        health += biteSelfHeal;
        needToBite--;
        if (needToBite <= 0 || health>0)
        {
            ReviveFromZombie();
        }
    }

    public float reviveMinHealth = 50;

    public virtual void ReviveFromZombie()
    {
        isZombie = false;
        health = Mathf.Max(reviveMinHealth, health);
    }
}
