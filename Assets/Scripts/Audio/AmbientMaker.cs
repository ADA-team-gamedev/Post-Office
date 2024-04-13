using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AmbientMaker : MonoBehaviour
    {
		[Header("Clip names")]
		[SerializeField] private string _mainNoiseMusicName = "Room Ambient";

		[SerializeField] private List<string> _ambientSoundsName = new();

		[Header("Values")]
		[SerializeField, Delayed] private float _minDelayBeforeAmbientPlaying;

		[SerializeField, Delayed] private float _maxDelayBeforeAmbientPlaying;

		private Dictionary<string, float> _ambientSounds;

		private float _clipPauseDelay;

		private void Start()
		{
			FillSoundClips();

			_clipPauseDelay = Random.Range(_minDelayBeforeAmbientPlaying, _maxDelayBeforeAmbientPlaying);

			AudioManager.Instance.PlayLoopedSound(_mainNoiseMusicName, transform.position);

			StartCoroutine(PlayMusic());			
		}

		private void FillSoundClips()
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
			if (_ambientSounds.Count <= 0)
			{
				Debug.LogWarning("No music clips!");

				yield break;
			}	

			yield return new WaitForSeconds(_clipPauseDelay);

			int ambientIndex = Random.Range(0, _ambientSoundsName.Count);

			string clipName = _ambientSoundsName[ambientIndex];

			if (_ambientSounds.ContainsKey(clipName))
			{
				float clipLength = _ambientSounds[clipName];

				AudioManager.Instance.PlaySound(clipName, transform.position);

				float newClipPauseDelay = Random.Range(_minDelayBeforeAmbientPlaying, _maxDelayBeforeAmbientPlaying);

				_clipPauseDelay = Mathf.Clamp(newClipPauseDelay, clipLength, _maxDelayBeforeAmbientPlaying);
			}	

			StartCoroutine(PlayMusic());
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