using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingController : MonoBehaviour
{
    [SerializeField] Volume volume;
    [SerializeField] VolumeProfile normal;
    [SerializeField] VolumeProfile zombie;

    public void ChangeVolumeProfile(bool isZombie)
    {
        volume.profile = isZombie ? zombie : normal;
    }
}
