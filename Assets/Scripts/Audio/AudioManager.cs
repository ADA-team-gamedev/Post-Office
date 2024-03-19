using UnityEngine;
using UnityEngine.Audio;
using AYellowpaper.SerializedCollections;
using System;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

		[SerializedDictionary(nameof(String), nameof(SoundClipData))]
		public SerializedDictionary<string, SoundClipData> SoundClips;

		[SerializedDictionary(nameof(AudioMixerGroup), nameof(AudioSource))]
        public SerializedDictionary<AudioMixerGroup, AudioSource> AudioSources;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;		
			else
				Debug.LogError($"{nameof(AudioManager)} Instance already exists!");		
		}

		private void Start()
		{
			FillAudioSources();
		}

		private void FillAudioSources()
		{
			//AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

			//foreach (var source in audioSources)
			//{
			//	AudioSources.Add(source.outputAudioMixerGroup, source);
			//}
		}

		public void PlaySound(string clipName, Vector3 spawnPosition, float volume = 1f, float spatialBlend = 0)
		{
			if (!SoundClips.TryGetValue(clipName, out SoundClipData soundClipData))
			{
				Debug.LogWarning($"No clip with name {clipName}");

				return;
			}

			SoundClip soundClip = soundClipData.SoundClip;

			if (!AudioSources.TryGetValue(soundClip.MixerGroup, out AudioSource basedAudioSource))
			{
				Debug.LogWarning($"No audio source with {soundClip.MixerGroup}");

				return;
			}

			AudioSource audioSource = Instantiate(basedAudioSource, spawnPosition, Quaternion.identity);
			
			audioSource.clip = soundClip.Clip;
			
			audioSource.volume = Mathf.Clamp01(volume);

			audioSource.outputAudioMixerGroup = soundClip.MixerGroup;

			audioSource.spatialBlend = Mathf.Clamp01(spatialBlend);

			audioSource.Play();

			float clipLength = audioSource.clip.length;

			Destroy(audioSource.gameObject, clipLength);
		}
	}
}