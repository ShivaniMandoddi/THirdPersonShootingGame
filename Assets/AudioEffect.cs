using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEffect : MonoBehaviour
{
    // Start is called before the first frame update
    public List<AudioClip> clip;
    AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
    }
    public void Walk()
    {
        audioSource.clip = clip[0];
        audioSource.Play();
    }
    public void Shoot()
    {
        audioSource.clip = clip[1];
        audioSource.Play();
    }
}
