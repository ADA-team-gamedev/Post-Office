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

		[SerializeField] private DissolveEffect _dissolveEffect;

		private void Start()
		{
			_dissolveEffect.gameObject.SetActive(false);
			
			_timeClock.OnGameCompleted += FinishDayWork;
		}

		[ContextMenu("Finish Day")]
		public void FinishDayWork()
		{
			_timeClock.OnGameCompleted -= FinishDayWork;

			_dissolveEffect.gameObject.SetActive(true);

			StartCoroutine(PlayDissolveEffect());
		}

		private IEnumerator PlayDissolveEffect()
		{
			float dissolveEffectStrength = DissolveEffect.MinDissolveEffectStrength;

			while (dissolveEffectStrength < DissolveEffect.MaxDissolveEffectStrength)
			{
				_dissolveEffect.ChangeEffectStrength(dissolveEffectStrength);

				dissolveEffectStrength += Time.deltaTime;

				yield return null;
			}

			SceneManager.LoadScene(_menuSceneName);
		}
	}
}