using Effects;
using System.Collections;
using TaskSystem.NoteBook;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level
{
	public class DayFinisher : MonoBehaviour
	{
		[SerializeField] private string _menuSceneName = "Menu";
		[SerializeField] private TimeClock _timeClock;

		[Header("Effect")]
		[SerializeField, Range(0, 10)] private float _dissolveEffectDelay = 1f;

		[SerializeField] private DissolveEffect _effect;

		private float dissolveEffectStrength = 1;

		private void Start()
		{
			_effect.gameObject.SetActive(false);

			_timeClock.OnGameCompleted += FinishDayWork;
		}

		[ContextMenu("Finish Day")]
		public void FinishDayWork()
		{
			_timeClock.OnGameCompleted -= FinishDayWork;

			_effect.gameObject.SetActive(true);

			StartCoroutine(PlayDissolveEffect());
		}

		private IEnumerator PlayDissolveEffect()
		{
			while (dissolveEffectStrength > 0)
			{
				_effect.ApplyDissolveEffectChanges(dissolveEffectStrength);

				dissolveEffectStrength -= Time.deltaTime;

				yield return null;
			}

			SceneManager.LoadScene(_menuSceneName);
		}
	}
}