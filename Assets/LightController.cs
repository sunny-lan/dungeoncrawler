using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] MeshRenderer tube;
    [SerializeField] Light lightBulb;

    Color maxTubeIntensity;
    float maxBulbIntensity;
    private void Start()
    {
        maxBulbIntensity = lightBulb.intensity;
        maxTubeIntensity = tube.material.GetColor("_EmissionColor");
    }

    // from 0 to 1
    float intensity = 1;
    public float Intensity
    {
        get => intensity; set
        {
            intensity = value;
            lightBulb.intensity = value * maxBulbIntensity;
            tube.material.SetColor("_EmissionColor", value * maxTubeIntensity);
        }
    }
}
