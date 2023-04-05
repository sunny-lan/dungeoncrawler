using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTelegraphController : MonoBehaviour
{
    public enum TelgraphType { MOVE, ATTACK };
    private TelgraphType type;
    private Vector2Int direction;

    [Header("telegraph")]
    [SerializeField] private Transform indicatorPivot;
    [SerializeField] private new MeshRenderer renderer;
    [SerializeField] private Material humanMat;
    [SerializeField] private Material zombieMat;

    [Header("emotion")]
    [SerializeField] private Image emotionImage;
    [SerializeField] private Sprite hostileSprite;
    [SerializeField] private Sprite fleeSprite;

    public void UpdateTelegraph(TelgraphType type, Vector2Int direction, bool isZombie, int length = 1)
    {
        this.type = type;
        this.direction = direction;

        indicatorPivot.gameObject.SetActive(true);
        indicatorPivot.localScale = new Vector3(1, 1, length);
        transform.forward = new Vector3(direction.x, 0, direction.y);

        renderer.material = isZombie ? zombieMat : humanMat;
    }

    public void ClearTelegraph()
    {
        indicatorPivot.gameObject.SetActive(false);
    }

    public void SetEmotion(bool hostile)
    {
        emotionImage.sprite = hostile ? hostileSprite : fleeSprite;
        emotionImage.enabled = true;
    }

    public void ClearEmotion()
    {
        emotionImage.sprite = null;
        emotionImage.enabled = false;
    }

    public TelgraphType GetTelegraphType()
    {
        return type;
    }

    public Vector2Int GetDirection()
    {
        return direction;
    }

}
