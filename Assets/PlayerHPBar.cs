using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface HPBar
{
    public void SetHP(float hp);
}

public class PlayerHPBar : MonoBehaviour, HPBar
{
    // Value between -1 and 1
    public float HP;
    private float _hpLerp;

    [SerializeField] RectTransform negativeAnimated, positiveAnimated;
    [SerializeField] RectTransform negative, positive;

    public float lerpSpeed = 1;
    public float minWidth = 0.02f;
    public bool flip = false;

    void Start()
    {
        _hpLerp = HP;
    }

    private void Update()
    {
        _hpLerp = Mathf.Lerp(_hpLerp, HP, Time.deltaTime * lerpSpeed);

        setTransforms(negative, positive, HP);
        setTransforms(negativeAnimated, positiveAnimated, _hpLerp);
    }


    void setTransforms(RectTransform negative, RectTransform positive, float hp)
    {
        if (flip)
        {


            var scale = this.positive.anchorMin;
            scale.x = 1 - Mathf.Max(minWidth, -hp);
            negative.anchorMin = scale;

            scale = negative.anchorMax;
            scale.x = Mathf.Max(minWidth, hp);
            positive.anchorMax = scale;
        }
        else
        {

            var scale = this.positive.anchorMin;
            scale.x = 1 - Mathf.Max(minWidth, hp);
            positive.anchorMin = scale;

            scale = negative.anchorMax;
            scale.x = Mathf.Max(minWidth, -hp);
            negative.anchorMax = scale;
        }
    }

    public void SetHP(float hp)
    {
        HP = hp;
    }
}
