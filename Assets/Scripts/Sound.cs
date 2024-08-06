using System;
using UnityEngine;
[Serializable]
public class Sound
{
    public string name;
    [Range(0f, 1f)]
    public float volume;
    [Range(0f, 1f)]
    public float pitch;
    public AudioClip clip;
    [HideInInspector]
    public AudioSource audioSource;
}