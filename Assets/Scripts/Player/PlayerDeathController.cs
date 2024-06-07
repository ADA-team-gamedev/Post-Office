using Level;
using System;
using System.Collections;
using UnityEngine;
using UnityModification;

namespace Player
{
    public class PlayerDeathController : DestructiveBehaviour<PlayerDeathController>
    {
        [SerializeField][Range(1f, 10f)] private float _afterDeathDelay = 5f;

        [SerializeField] private DayFinisher _dayFinisher;

        public bool IsAlive { get; private set; } = true;

        public event Action OnDied;

		public void Die()
        {
            if (!IsAlive)
                return;
#if UNITY_EDITOR
            Debug.Log("The player has died!");
#endif
            IsAlive = false;

            OnDied?.Invoke();

            StartCoroutine(FinishDay());
		}

        private IEnumerator FinishDay()
        {
            yield return new WaitForSeconds(_afterDeathDelay);

            _dayFinisher.FinishDayWork();
		}
	}
}
