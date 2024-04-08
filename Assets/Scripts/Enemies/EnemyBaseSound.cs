using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseSound : MonoBehaviour
{
    [SerializeField] private AudioSource aud;
    [SerializeField] private AudioClip die;
    [SerializeField] private AudioClip hit;

    public void PlayDie()
    {
        aud.PlayOneShot(die);
    }
    public void PlayHurt()
    {
        aud.PlayOneShot(hit);
    }
}
