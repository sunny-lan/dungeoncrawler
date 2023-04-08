using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopRandomAudio : MonoBehaviour
{
    [SerializeField] AudioClip[] clips;
    [SerializeField] AudioSource source;
    [SerializeField] float delayMin;
    [SerializeField] float delayMax;

    private void Start()
    {
        if (delayMin > delayMax)
        {
            float temp = delayMax;
            delayMax = delayMin;
            delayMin = temp;
        }
        if (clips.Length > 0)
            StartCoroutine(loop());
    }

    IEnumerator loop()
    {
        // yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
        while (true)
        {
            source.clip = clips[Random.Range(0, clips.Length)];
            source.Play();
            yield return new WaitForSeconds(source.clip.length + Random.Range(delayMin, delayMax));
        }
    }
}
