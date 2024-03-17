using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

		[SerializeField] private List<SoundClip> _soundClips = new();
		[SerializeField] private List<AudioSource> _audioSources = new();

		private void Awake()
		{
			if (Instance == null)
				Instance = this;		
			else
				Debug.LogError("AudioManager Instance already exists!");		
		}

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
			
			foreach (var item in _audioSources)
			{
				DontDestroyOnLoad(item.gameObject);
			}
		}

		public void Play(string clipName)
		{
			if (!TryGetClipByName(clipName, out SoundClip soundClip))
			{
				Debug.LogWarning($"No clip with {clipName} name!");
			}

			Play(soundClip);
		}

		public void Play(int clipID)
		{
			if (!TryGetClipByID(clipID, out SoundClip soundClip))
			{
				Debug.LogWarning($"No clip with {clipID} ID!");

				return;
			}
			
			Play(soundClip);	
		}

		private void Play(SoundClip soundClip)
		{
			if (!TryGetAudioSourceByMixer(soundClip.MixerGroup, out AudioSource selectedSource))
			{
				Debug.LogWarning($"We don't have audio source with {soundClip.MixerGroup} mixer in our mixers collection!");

				return;
			}

			selectedSource.clip = soundClip.Clip;

			selectedSource.volume = soundClip.Volume;

			selectedSource.pitch = soundClip.Pitch;

			selectedSource.PlayOneShot(soundClip.Clip);
		}

		private bool TryGetClipByName(string clipName, out SoundClip soundClip)
		{
			foreach (var clip in _soundClips)
			{
				if (clip.ClipName == clipName)
				{
					soundClip = clip;

					return true;
				}	
			}

			soundClip = null;

			return false;
		}

		private bool TryGetClipByID(int clipID, out SoundClip soundClip)
		{
			foreach (var clip in _soundClips)
			{
				if (clip.ClipID == clipID)
				{
					soundClip = clip;

					return true;
				}
			}

			soundClip = null;

			return false;
		}

		private bool TryGetAudioSourceByMixer(AudioMixerGroup neededMixerGroup, out AudioSource audioSource)
		{
			foreach (var item in _audioSources)
			{
				if (item.outputAudioMixerGroup == neededMixerGroup)
				{
					audioSource = item;

					return true;
				}
			}

			audioSource = null;

			return false;
		}
	}
}