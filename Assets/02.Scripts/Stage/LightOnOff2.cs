using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOnOff2 : MonoBehaviour
{
    public Light light;
   // public AudioSource audioSource;
    public AudioClip OnSound;
    public AudioClip OffSound;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            light.enabled = true;
            SoundManager.soundManager.PlaySound(transform.position, OnSound);
            //audioSource.PlayOneShot(OnSound, 0.9f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            light.enabled = false;
            SoundManager.soundManager.PlaySound(transform.position, OffSound);
            //audioSource.PlayOneShot(OffSound, 0.9f);
        }
    }

}
