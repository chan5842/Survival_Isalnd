using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public float Volume= 1f; 
    public bool isSoundMute = false;
    public static SoundManager soundManager;

    void Awake()
    {
        if (soundManager == null)
            soundManager = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(Vector3 pos, AudioClip audioClip)
    {
        if (isSoundMute) return;

        GameObject soundObj = new GameObject("Sfx");
        soundObj.transform.position = pos;

        AudioSource source = soundObj.AddComponent<AudioSource>();
        source.clip = audioClip;
        source.minDistance = 10f;
        source.maxDistance = 30f;
        source.volume = Volume;
        source.Play();
        Destroy(soundObj, audioClip.length +10f);
    }
}
