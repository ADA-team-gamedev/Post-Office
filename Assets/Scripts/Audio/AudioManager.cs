using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

		[SerializeField] private List<SoundClipData> SoundClipDatas;

		private Dictionary<string, SoundClip> _soundclips = new();

        private Dictionary<AudioMixerGroup, AudioSource> _audioSources = new();

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
			{
#if UNITY_EDITOR
				Debug.LogError($"{nameof(AudioManager)} Instance already exists!");
#endif
			}

			FillAudioSource();

			FillSoundClips();
		}

		private void FillAudioSource()
		{
			AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
			
			foreach (var source in audioSources)
			{
				_audioSources.TryAdd(source.outputAudioMixerGroup, source);
			}
		}

		private void FillSoundClips()
		{
			foreach (var clipData in SoundClipDatas)
			{
				SoundClip clip = clipData.SoundClip;

				if (_soundclips.ContainsValue(clip))
				{
#if UNITY_EDITOR
					Debug.LogError($"{clip.Name} already exists in sound collection!");
#endif
					continue;
				}
				
				_soundclips.Add(clip.Name, clip);
			}
		}

		#region Play Sound

		public void PlaySound(string clipName)
		{
			PlaySound(clipName, transform.position);
		}

		public void PlaySound(string clipName, Vector3 spawnPosition, float volume = 1f, float spatialBlend = 0)
		{
			if (!TryGetClipFromCollection(clipName, out SoundClip soundClip) || !TryGetAudioSourceFromCollection(soundClip.MixerGroup, out AudioSource basedAudioSource))
				return;

			AudioSource audioSource = Instantiate(basedAudioSource, spawnPosition, Quaternion.identity);

			SetValuesInAudioSource(audioSource, new(soundClip.Clip, volume, soundClip.MixerGroup, spatialBlend));

			audioSource.Play();

			Destroy(audioSource.gameObject, audioSource.clip.length);
		}

		public void PlaySound(string clipName, Vector3 spawnPosition, float soundDelay, float volume = 1f, float spatialBlend = 0)
		{
			if (!TryGetClipFromCollection(clipName, out SoundClip soundClip) || !TryGetAudioSourceFromCollection(soundClip.MixerGroup, out AudioSource basedAudioSource))
				return;
			
			AudioSource audioSource = Instantiate(basedAudioSource, spawnPosition, Quaternion.identity);

			SetValuesInAudioSource(audioSource, new(soundClip.Clip, volume, soundClip.MixerGroup, spatialBlend));

			audioSource.Play();

			Destroy(audioSource.gameObject, soundDelay);
		}

		public void PlayLoopedSound(string clipName, Vector3 spawnPosition, float volume = 1f, float spatialBlend = 0)
		{
			if (!TryGetClipFromCollection(clipName, out SoundClip soundClip) || !TryGetAudioSourceFromCollection(soundClip.MixerGroup, out AudioSource basedAudioSource))
				return;

			AudioSource audioSource = Instantiate(basedAudioSource, spawnPosition, Quaternion.identity);

			SetValuesInAudioSource(audioSource, new(soundClip.Clip, volume, soundClip.MixerGroup, spatialBlend, true));

			audioSource.Play();
		}

		public bool TryGetSound(string clipName, out AudioClip clip)
		{
			if (TryGetClipFromCollection(clipName, out SoundClip soundClip))
			{
				clip = soundClip.Clip;

				return true;	
			}

			clip = null;

			return false;
		}

		#endregion

		private void SetValuesInAudioSource(AudioSource audioSource, AudioSourceParameters audioSourceParameters)
		{
			audioSource.clip = audioSourceParameters.AudioClip;

			audioSource.volume = audioSourceParameters.Volume;

			audioSource.outputAudioMixerGroup = audioSourceParameters.MixerGroup;

			audioSource.spatialBlend = audioSourceParameters.SpatialBlend;

			audioSource.loop = audioSourceParameters.IsLooped;
		}

		private bool TryGetClipFromCollection(string clipName, out SoundClip soundClip)
		{
			if (_soundclips.TryGetValue(clipName, out soundClip))
				return true;
#if UNITY_EDITOR
			Debug.LogWarning($"No clip with name - ({clipName})");
#endif
			return false;
		}

		private bool TryGetAudioSourceFromCollection(AudioMixerGroup mixerGroup, out AudioSource basedAudioSource)
		{
			if (_audioSources.TryGetValue(mixerGroup, out basedAudioSource))
				return true;
#if UNITY_EDITOR
			Debug.LogWarning($"No audio source with {mixerGroup}");
#endif
			return false;
		}
	}

	public struct AudioSourceParameters
	{
		public AudioClip AudioClip { get; private set; }

		public float Volume { get; private set; }

		public AudioMixerGroup MixerGroup { get; private set; }

		public float SpatialBlend { get; private set; }

		public bool IsLooped { get; private set; }

		public AudioSourceParameters(AudioClip audioClip, float volume, AudioMixerGroup mixerGroup, float spatialBlend, bool isLooped = false)
		{
			AudioClip = audioClip;

			Volume = Mathf.Clamp01(volume);

			IsLooped = isLooped;

			MixerGroup = mixerGroup;

			SpatialBlend = Mathf.Clamp01(spatialBlend);
		}
	}
}