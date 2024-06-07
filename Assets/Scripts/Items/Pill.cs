using Audio;
using Player;
using System;
using System.Collections;
using UnityEngine;
using UnityModification;

namespace Items
{
    public class Pill : Item, IUsable
    {
        [Header("Pill settings")]
        [SerializeField][Range(1, 5)] private int _countOfUses = 1;

        [Header("Values")]
        [SerializeField] private float _sanityAddingNumber;
        [SerializeField] private float _sanityAddingDelay;

        private bool _isUsing = false;

        public void Use(Interactor interactor)
        {
            if (IsHaveCharge())
            {
                if (!_isUsing)
                {
                    _countOfUses--;

					EditorDebug.Log($"{gameObject.name}s are used");

                    AudioManager.Instance.PlaySound("Use Pills", transform.position);

					StartCoroutine(RestoreeSanity(interactor.Sanity));
                }
            }
            else
            {
				EditorDebug.Log($"These {gameObject.name} pills are empty");

                AudioManager.Instance.PlaySound("Pill Empty", transform.position);
			}
        }

        private bool IsHaveCharge()
            => _countOfUses > 0;

        private IEnumerator RestoreeSanity(PlayerSanity sanity)
        {
            _isUsing = true;

            float timer = _sanityAddingDelay;

			while (timer > 0)
            {
                timer -= Time.deltaTime;

				sanity.Sanity += _sanityAddingNumber / _sanityAddingDelay * Time.deltaTime;

                yield return new WaitForSeconds(Time.deltaTime);
            }

            _isUsing = false;
        }
    }
}
