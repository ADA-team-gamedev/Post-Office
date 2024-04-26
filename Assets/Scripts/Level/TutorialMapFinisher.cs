using UnityEngine;
using UnityEngine.UI;

namespace Level
{
	public class TutorialMapFinisher : MonoBehaviour
	{
		[Header("Target Points")]
		[SerializeField] private Transform _endPoint;

		[SerializeField] private Transform _startedPoint;

		[SerializeField] private Transform _player;

		[Header("Values")]
		[SerializeField] private float _distanceThresshold = 5f;

		[Header("UI")]
		[SerializeField] private Image _fadeImage;

		[SerializeField] private DayFinisher _dayFinisher;

		private float _maxDistance;

		private bool _isFinished = false;

		private void Start()
		{
			Vector3 startPointPosition = new(_startedPoint.position.x, _endPoint.position.y, _startedPoint.position.z);

			_maxDistance = Vector3.Distance(_endPoint.position, startPointPosition);
		}

		private void Update()
		{
			if (IsPlayerNearEndPoint())
			{
				FinishTutorial();
			}
			else
			{
				float playerDistanceToEndPoint = GetPlayerDistanceToPoint();
				
				float normalizedDistance = Mathf.Clamp01((playerDistanceToEndPoint - _distanceThresshold) / (_maxDistance - _distanceThresshold));
				
				float alpha = 1f - normalizedDistance; 

				if (playerDistanceToEndPoint <= _distanceThresshold)
					alpha = 0f; 
				
				Color _fadeImageColor = _fadeImage.color;

				_fadeImageColor.a = alpha;
				_fadeImage.color = _fadeImageColor;
			}

		}

		private bool IsPlayerNearEndPoint()
		{
			return GetPlayerDistanceToPoint() <= _distanceThresshold;
		}

		private float GetPlayerDistanceToPoint()
		{
			Vector3 playerPosition = new(_player.position.x, _endPoint.position.y, _player.position.z);

			return Vector3.Distance(_endPoint.position, playerPosition);
		} 

		private void FinishTutorial()
		{
			if (_isFinished)
				return;

			_isFinished = true;
			
			_dayFinisher.FinishDayWork();
		}

		private void OnDrawGizmosSelected()
		{
			if (!_endPoint || !_player || !_startedPoint)
				return;

			Gizmos.color = Color.yellow;

			Vector3 playerPosition = new(_player.position.x, _endPoint.position.y, _player.position.z);

			Gizmos.DrawWireSphere(_startedPoint.position, 1f);

			Gizmos.DrawLine(_endPoint.position, playerPosition);

			Color color = IsPlayerNearEndPoint() ? Color.green : Color.red;

			Gizmos.color = color;

			Gizmos.DrawWireSphere(_endPoint.position, _distanceThresshold);
		}
	}
}