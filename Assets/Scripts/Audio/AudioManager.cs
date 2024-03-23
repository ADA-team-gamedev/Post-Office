using UnityEngine;
using UnityEngine.Audio;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

		[SerializeField] private List<SoundClipData> SoundClipDatas;

		private Dictionary<string, SoundClip> _soundclips = new();

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
			FillSoundClips();
			
			FillAudioSources();
		}

		private void FillSoundClips()
		{
			foreach (var clipData in SoundClipDatas)
			{
				SoundClip clip = clipData.SoundClip;

				if (_soundclips.ContainsValue(clip))
				{
					Debug.LogError($"{clip.Name} already exists in sound collection!");

					continue;
				}
				
				_soundclips.Add(clip.Name, clip);
			}
		}

		private void FillAudioSources()
		{
			if (AudioSources.Count > 0)
				return;

			AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

			foreach (var source in audioSources)
			{
				AudioSources.Add(source.outputAudioMixerGroup, source);
			}
		}

		public void PlaySound(string clipName, Vector3 spawnPosition, float volume = 1f, float spatialBlend = 0)
		{
			if (!_soundclips.TryGetValue(clipName, out SoundClip soundClip))
			{
				Debug.LogWarning($"No clip with name {clipName}");

				return;
			}

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