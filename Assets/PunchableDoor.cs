using System.Collections;
using UnityEngine;


public class PunchableDoor : MonoBehaviour, IPunchable
{
    private ShakeController shaker;
    private Rigidbody rb;
    private GameManager gameManager;
    public float maxShake = 0.3f, defaultShake = 0.1f;

    public float maxHP = 30;
    float HP=1;
    public bool setWalkable = true;

    void Awake()
    {
        shaker = GetComponentInChildren<ShakeController>();
        rb = GetComponentInChildren<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        rb.isKinematic = true;
        initialpos = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)
        );
    }


    public float hpFadeoutTime = 1.5f;
    float lastPunchTime;
    public bool broken { get; private set; } = false;
    private Vector2Int initialpos;
    public float punchImpulse = 1;

    public void GetPunched(GridEntity by, float strength, Vector3 direction)
    {
        shaker._distance = Mathf.Min(maxShake, defaultShake * strength);
        shaker.Begin();

        lastPunchTime = Time.timeSinceLevelLoad;

        HP -= strength/maxHP;

        if (HP <= 0)
        {
            Break(direction * strength * punchImpulse);
        }
    }

    public void Break(Vector3 direction)
    {
        rb.isKinematic = false;
        rb.AddForceAtPosition(direction, rb.centerOfMass, ForceMode.Impulse);
        broken = true;
        if (setWalkable)
        {
            gameManager.SetWalkable(initialpos, true);
        }
        enabled = false;
    }
}
