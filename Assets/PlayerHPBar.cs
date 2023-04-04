using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HPBar
{
    public void SetHP(float hp);
}

public class PlayerHPBar : MonoBehaviour, HPBar
{
    // Value between -1 and 1
    public float HP;
    private float _hpLerp;

    [SerializeField] RectTransform negative, positive;

    public float lerpSpeed = 1;
    public float minWidth = 0.02f;

    void Start()
    {
        _hpLerp = HP;
    }

    private void Update()
    {
        _hpLerp = Mathf.Lerp(_hpLerp, HP, Time.deltaTime * lerpSpeed);

        var scale = positive.anchorMin;
        scale.x = 1 - Mathf.Max(minWidth, _hpLerp );
        positive.anchorMin = scale;

        scale = negative.anchorMax;
        scale.x = Mathf.Max(minWidth, -_hpLerp );
        negative.anchorMax = scale;
    }

    public void SetHP(float hp)
    {
        HP = hp;
    }
}
