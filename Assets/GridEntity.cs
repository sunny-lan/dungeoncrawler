using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridEntity : MonoBehaviour
{
    public UnityEvent<bool> onChangeZombieStatus;
    public bool isZombieInitial = false;
    public bool isZombie
    {
        get => _isZombie; set
        {
            _isZombie = value;
            if (renderer)
                renderer.material.color = _isZombie ? Color.green : Color.red;
            onChangeZombieStatus?.Invoke(_isZombie);
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
            onChangeHealth?.Invoke(_health);
        }
    }
    public UnityEvent<float> onChangeHealth;

    public float biteDamage = 190;
    public float biteSelfHeal = 70;

    [SerializeField] private MeshRenderer renderer;
    private bool _isZombie;

    protected virtual void Awake() { }

    public virtual bool GetIsZombie()
    {
        return isZombie;
    }

    protected virtual void Start()
    {
        GameManager.Instance.RegisterEntity(this);
        pos.x = Mathf.FloorToInt(transform.position.x);
        pos.y = Mathf.FloorToInt(transform.position.z);
        transform.position = new Vector3(pos.x, 0, pos.y); // snap to grid
        isZombie = isZombieInitial;
        health = initialHealth;
    }
    protected virtual void Update()
    {
    }

    public virtual void GetBitten(GridEntity by)
    {
        Debug.Assert(by.GetIsZombie(), $"{by.name} was not a zombie");
        Debug.Log($"{name} got bitten by {by.name}");
        isZombie = true;
    }

    public virtual void Bite(GridEntity victim)
    {
        victim.GetBitten(this);
        victim.health = Mathf.Clamp(victim.health - biteDamage, -maxHealth, -1);
        health += biteSelfHeal;
    }
}
