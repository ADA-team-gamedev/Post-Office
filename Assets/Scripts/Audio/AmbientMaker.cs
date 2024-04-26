using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AmbientMaker : MonoBehaviour
    {
		[Header("Clip names")]
		[SerializeField] private List<string> _backgroundMusicClipsName = new();

		[SerializeField] private List<string> _ambientSoundsName = new();

		[Header("Values")]
		[SerializeField, Delayed] private float _minDelayBeforeAmbientPlaying = 10;

		[SerializeField, Delayed] private float _maxDelayBeforeAmbientPlaying = 20;

		private Dictionary<string, float> _musicClipsParameters;

		private Dictionary<string, float> _ambientSounds;

		private float _musicClipPauseDelay = 1f;

		private float _ambientClipPauseDelay = 1f;

		private void Start()
		{
			FillBackgroundMusicClips();

			FillAmbientSoundClips();

			_ambientClipPauseDelay = Random.Range(_minDelayBeforeAmbientPlaying, _maxDelayBeforeAmbientPlaying);		

			StartCoroutine(PlayAmbient());

			StartCoroutine(PlayMusic());
		}

		private void FillBackgroundMusicClips()
		{
			_musicClipsParameters = new();

			foreach (var clipName in _backgroundMusicClipsName)
			{
				if (AudioManager.Instance.TryGetSound(clipName, out AudioClip clip))
					_musicClipsParameters.Add(clipName, clip.length);
			}
		}

		private void FillAmbientSoundClips()
		{
			_ambientSounds = new(_ambientSoundsName.Count);

			foreach (var clipName in _ambientSoundsName)
			{
				if (AudioManager.Instance.TryGetSound(clipName, out AudioClip clip))
					_ambientSounds.Add(clipName, clip.length);			
			}
		}

		private IEnumerator PlayMusic()
		{
			if (_musicClipsParameters.Count <= 0)
			{
				Debug.LogWarning("No music clips!");

				yield break;
			}

			yield return new WaitForSeconds(_musicClipPauseDelay);

			int ambientIndex = Random.Range(0, _backgroundMusicClipsName.Count);
			
			string clipName = _backgroundMusicClipsName[ambientIndex];

			if (_musicClipsParameters.ContainsKey(clipName))
			{
				float clipLength = _musicClipsParameters[clipName];

				AudioManager.Instance.PlaySound(clipName, transform.position);

				_musicClipPauseDelay = clipLength;
			}

			StartCoroutine(PlayMusic());
		}

		private IEnumerator PlayAmbient()
		{
			if (_ambientSounds.Count <= 0)
			{
				Debug.LogWarning("No ambient clips!");

				yield break;
			}	

			yield return new WaitForSeconds(_ambientClipPauseDelay);

			int ambientIndex = Random.Range(0, _ambientSoundsName.Count);

			string clipName = _ambientSoundsName[ambientIndex];

			if (_ambientSounds.ContainsKey(clipName))
			{
				float clipLength = _ambientSounds[clipName];

				AudioManager.Instance.PlaySound(clipName, transform.position);

				float newClipPauseDelay = Random.Range(_minDelayBeforeAmbientPlaying, _maxDelayBeforeAmbientPlaying);

				_ambientClipPauseDelay = Mathf.Clamp(newClipPauseDelay, clipLength, _maxDelayBeforeAmbientPlaying);
			}	

			StartCoroutine(PlayAmbient());
		}

		private void OnValidate()
		{
			if (_minDelayBeforeAmbientPlaying < 0)
				_minDelayBeforeAmbientPlaying = 0;

			if (_maxDelayBeforeAmbientPlaying < _minDelayBeforeAmbientPlaying)
				_maxDelayBeforeAmbientPlaying = _minDelayBeforeAmbientPlaying;
		}
	}
}