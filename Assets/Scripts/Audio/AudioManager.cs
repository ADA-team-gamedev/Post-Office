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
				Debug.LogWarning($"No clip with name - ({clipName})");

				return;
			}

			if (!AudioSources.TryGetValue(soundClip.MixerGroup, out AudioSource basedAudioSource))
			{

				Debug.LogWarning($"No audio source with {soundClip.MixerGroup}");

				return;
			}

			AudioSource audioSource = Instantiate(basedAudioSource, spawnPosition, Quaternion.identity);

			SetValuesInAudioSource(audioSource, new(soundClip.Clip, volume, soundClip.MixerGroup, spatialBlend));

			audioSource.Play();

			Destroy(audioSource.gameObject, audioSource.clip.length);
		}

		public void PlaySound(string clipName, Vector3 spawnPosition, float soundDelay, float volume = 1, float spatialBlend = 0)
		{
			if (!_soundclips.TryGetValue(clipName, out SoundClip soundClip))
			{
				Debug.LogWarning($"No clip with name - ({clipName})");

				return;
			}

			if (!AudioSources.TryGetValue(soundClip.MixerGroup, out AudioSource basedAudioSource))
			{
				Debug.LogWarning($"No audio source with {soundClip.MixerGroup}");

				return;
			}
			
			AudioSource audioSource = Instantiate(basedAudioSource, spawnPosition, Quaternion.identity);

			SetValuesInAudioSource(audioSource, new(soundClip.Clip, volume, soundClip.MixerGroup, spatialBlend));

			audioSource.Play();

			Destroy(audioSource.gameObject, soundDelay);
		}

		private void SetValuesInAudioSource(AudioSource audioSource, AudioSourceParameters audioSourceParameters)
		{
			audioSource.clip = audioSourceParameters.AudioClip;

			audioSource.volume = audioSourceParameters.Volume;

			audioSource.outputAudioMixerGroup = audioSourceParameters.MixerGroup;

			audioSource.spatialBlend = audioSourceParameters.SpatialBlend;
		}
	}

	public struct AudioSourceParameters
	{
		public AudioClip AudioClip { get; private set; }

		public float Volume { get; private set; }

		public AudioMixerGroup MixerGroup { get; private set; }

		public float SpatialBlend { get; private set; }

		public AudioSourceParameters(AudioClip audioClip, float volume, AudioMixerGroup mixerGroup, float spatialBlend)
		{
			AudioClip = audioClip;

			Volume = Mathf.Clamp01(volume);

			MixerGroup = mixerGroup;

			SpatialBlend = Mathf.Clamp01(spatialBlend);
		}
	}
}