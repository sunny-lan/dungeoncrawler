using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridEntity : MonoBehaviour
{
    public UnityEvent<bool> onChangeZombieStatus;
    public bool isZombie;
    public Vector2Int pos;
    public Transform raycastCenter;
    public float health;
    [SerializeField] private MeshRenderer renderer;

    protected virtual void Awake() { }

    public virtual bool GetIsZombie()
    {
        return isZombie;
    }

    [ContextMenu("SetIsZombie")]
    public virtual void SetIsZombie(bool isZomb = true)
    {
        isZombie = isZomb;
        onChangeZombieStatus?.Invoke(isZombie);
        if (renderer)
            renderer.material.color = isZomb ? Color.green : Color.red;
    }

    protected virtual void Start()
    {
        GameManager.Instance.RegisterEntity(this);
        pos.x = Mathf.FloorToInt(transform.position.x);
        pos.y = Mathf.FloorToInt(transform.position.z);
        transform.position = new Vector3(pos.x, 0, pos.y); // snap to grid
        SetIsZombie(isZombie); //sus but whatever
    }
    protected virtual void Update()
    {
    }

    public virtual void GetBitten(GridEntity by)
    {
        Debug.Log($"{name} got bitten by {by.name}");
        SetIsZombie();
    }

    public virtual void GetDamaged(GridEntity by, float dmg)
    {
        health -= dmg;
    }
}
