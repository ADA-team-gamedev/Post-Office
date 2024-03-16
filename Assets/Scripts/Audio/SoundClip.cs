using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [CreateAssetMenu]
    public class SoundClip : ScriptableObject
    {
        [field: SerializeField] public string ClipName { get; private set; }

        [field: SerializeField] public int ClipID { get; private set; }

        [field: SerializeField] public AudioClip Clip { get; private set; }

        [field: SerializeField] public AudioMixerGroup MixerGroup { get; private set; }

        [field: SerializeField, Range(0f, 1f)] public float Volume { get; private set; } = 0.5f;

        [field: SerializeField, Range(-3f, 3f)] public float Pitch { get; private set; } = 1f;
    }
}