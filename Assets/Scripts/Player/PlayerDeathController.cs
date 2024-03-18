using Level;
using System;
using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerDeathController : MonoBehaviour
    {
        [SerializeField][Range(1f, 10f)] private float _afterDeathDelay = 5f;

        [SerializeField] private DayFinisher _dayFinisher;

        public bool IsAlive { get; private set; } = true;

        public event Action OnDeath;

        public void Die()
        {
            if (!IsAlive)
                return;

            Debug.Log("The player has died");

            IsAlive = false;

            OnDeath?.Invoke();

            StartCoroutine(FinishDay());
		}

        private IEnumerator FinishDay()
        {
            yield return new WaitForSeconds(_afterDeathDelay);

            _dayFinisher.FinishDayWork();
		}
    }
}
