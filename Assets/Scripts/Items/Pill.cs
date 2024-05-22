using Audio;
using Player;
using System;
using System.Collections;
using UnityEngine;

namespace Items
{
    public class Pill : Item, IUsable
    {
        [Header("Pill settings")]
        [SerializeField][Range(1, 5)] private int _countOfUses = 1;

        [Header("Values")]
        [SerializeField] private float _sanityAddingNumber;
        [SerializeField] private float _sanityAddingDelay;

        [SerializeField] private PlayerSanity _sanity;

        private bool _isUsing = false;

        public void Use(Interactor interactor)
        {
            if (IsHaveCharge())
            {
                if (!_isUsing)
                {
                    _countOfUses--;
#if UNITY_EDITOR
					Debug.Log($"{gameObject.name}s are used");
#endif
                    AudioManager.Instance.PlaySound("Use Pills", transform.position);

					StartCoroutine(RestoreeSanity());
                }
            }
            else
            {
#if UNITY_EDITOR
				Debug.Log($"These {gameObject.name} pills are empty");
#endif
                AudioManager.Instance.PlaySound("Pill Empty", transform.position);
			}
        }

        private bool IsHaveCharge()
            => _countOfUses > 0;

        private IEnumerator RestoreeSanity()
        {
            _isUsing = true;

            float timer = _sanityAddingDelay;

			while (timer > 0)
            {
                timer -= Time.deltaTime;

                _sanity.Sanity += _sanityAddingNumber / _sanityAddingDelay * Time.deltaTime;

                yield return new WaitForSeconds(Time.deltaTime);
            }

            _isUsing = false;
        }
    }
}
