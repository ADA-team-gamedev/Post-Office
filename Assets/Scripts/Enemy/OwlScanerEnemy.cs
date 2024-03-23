using Audio;
using Player;
using System.Collections;
using UnityEngine;

namespace Enemy
{
	[RequireComponent(typeof(FieldOfView))]
	public class OwlScanerEnemy : MonoBehaviour
	{
		private FieldOfView _fieldOfView;

		[Header("Player")]
		[SerializeField] private PlayerSanity _playerSanity;

		[Header("Values")]
		[SerializeField][Range(0.01f, 1f)] private float _sanityPercentToStartScanning = 0.6f;
		[SerializeField][Range(10, 300)] private float _breakDelayInSeconds = 60f;

		[Header("Sounds")]
		[SerializeField] private string _targetDetectedSound = "Owl Detect Target";

		[SerializeField] private BoxEnemy[] _boxEnemies;

		private bool _isEnemyCalledOut = false;

		private void Start()
		{
			_fieldOfView ??= GetComponent<FieldOfView>();
		}

		private void Update()
		{
			if (_playerSanity.SanityPercent > _sanityPercentToStartScanning || _isEnemyCalledOut)
				return;

			_fieldOfView.VisionCheck();

			if (_fieldOfView.SeesInFOV)
				TryCallEnemy();
		}

		private void TryCallEnemy()
		{
			if (_boxEnemies.Length < 0 || _isEnemyCalledOut)
				return;

			AudioManager.Instance.PlaySound(_targetDetectedSound, transform.position);

			if (TryTakeBoxForOrder(out BoxEnemy boxEnemy))
			{
				_isEnemyCalledOut = true;

				if (!boxEnemy.IsAIActivated)
					boxEnemy.ActivateEnemyBox();

				StartCoroutine(OrderEnemy(boxEnemy));
			}
		}

		private IEnumerator OrderEnemy(BoxEnemy boxEnemy)
		{
			yield return new WaitForSeconds(boxEnemy.TranfromToEnemyDelay + 1);

			boxEnemy.OrderToAttack(_fieldOfView.Target.position);

			StartCoroutine(TakeBreak());
		}

		private IEnumerator TakeBreak()
		{
			yield return new WaitForSeconds(_breakDelayInSeconds);

			_isEnemyCalledOut = false;
		}

		private bool TryTakeBoxForOrder(out BoxEnemy boxToOrder)
		{
			foreach (var boxEnemy in _boxEnemies)
			{
				if (boxEnemy.IsAIActivated)
				{
					boxToOrder = boxEnemy;

					return true;
				}

				if (boxEnemy.IsCanActivateEnemy())
				{
					boxToOrder = boxEnemy;

					return true;
				}
			}

			boxToOrder = null;

			return false;
		}
	}
}
