using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioSource aud;
    [SerializeField] private AudioClip shoot;
    [SerializeField] private AudioClip hit;
    public void PlayShoot()
    {
        aud.PlayOneShot(shoot);
    }
    public void PlayHurt()
    {
        aud.PlayOneShot(hit);
    }
}
