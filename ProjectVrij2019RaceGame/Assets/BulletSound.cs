using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSound : MonoBehaviour
{
    public AudioSource[] audioSources;
    

    int lastSourceUsed;

    // Start is called before the first frame update
    void Start()
    {
        audioSources = transform.GetComponentsInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayAudioClip() {

        if(lastSourceUsed < audioSources.Length) {

            lastSourceUsed++;

        } 
        else {

            lastSourceUsed = 0;
        }

        audioSources[lastSourceUsed].pitch = Random.Range(0.9f, 1.05f);
        audioSources[lastSourceUsed].Play();
 
    }
}
