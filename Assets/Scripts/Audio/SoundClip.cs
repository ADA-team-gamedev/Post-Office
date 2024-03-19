using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [Serializable]
    public class SoundClip
    {
		[field: SerializeField] public AudioClip Clip { get; private set; }

		[field: SerializeField] public AudioMixerGroup MixerGroup { get; private set; }
	}
}