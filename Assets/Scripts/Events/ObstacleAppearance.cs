using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ObstacleAppearance : MonoBehaviour
{
	#region Obstacle

	[Header("Obstacle")]
    [SerializeField][Range(0.01f, 1)] private float _sanityPercentToStart = 0.5f;

	[SerializeField] private float _obstacleTimeExistenceInSeconds = 10f;
	private float _obstacleTimeRemaining = 0;

	[SerializeField] private GameObject _obstacle;

	[SerializeField] private GameObject[] _interferingObstacles;

	public event Action OnObstacleDisappeared;
	public event Action OnObstacleAppeared;

	#endregion

	[Header("Player Settings")]
	[SerializeField] private string _playerTag = "Player";

	[SerializeField] private Camera _playerCamera;
	[SerializeField] private PlayerSanity _playerSanity;

	private BoxCollider _boxCollider;
	private bool _isPlayerInterfering = false;

	private void Start()
	{
		_obstacle.SetActive(false);

		_boxCollider ??= GetComponent<BoxCollider>();
		_boxCollider.isTrigger = true;
	}

	private void Update()
	{
		if (_playerSanity.SanityPercent > _sanityPercentToStart)
			return;

		if (_obstacle.activeSelf)
		{
			if (IsCanDiactivateObject())
				DiactivateObject();
		}
		else
		{
			if (_obstacleTimeRemaining >= _obstacleTimeExistenceInSeconds)
			{
				if (IsCanSpawnObstacle())
					SpawnObstacle();
			}
		}

		_obstacleTimeRemaining += _obstacle.activeSelf ? -Time.deltaTime : Time.deltaTime;

		_obstacleTimeRemaining = Mathf.Clamp(_obstacleTimeRemaining, 0, _obstacleTimeExistenceInSeconds);
	}

	#region Triggers

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(_playerTag))
			_isPlayerInterfering = true;	
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag(_playerTag))
			_isPlayerInterfering = false;
	}

	#endregion

	#region Obstacle methods

	public void SpawnObstacle(bool checkIsPlayerLookedOnIt)
	{
		if (_obstacle.activeSelf)
			return;

		if (checkIsPlayerLookedOnIt)
		{
			if (!IsCanSpawnObstacle())
			{
				Debug.Log($"Player looking on obstacle which we want to spawn. We can't do that!");

				return;
			}
		}

		SpawnObstacle();
	}

	public void DiactivateObstacle(bool checkIsPlayerLookedOnIt)
	{
		if (!_obstacle.activeSelf)
			return;

		if (checkIsPlayerLookedOnIt)
		{
			if (!IsCanDiactivateObject())
			{
				Debug.Log($"Player looking on obstacle which we want to diactivate. We can't do that!");

				return;
			}
		}

		DiactivateObject();
	}

	private void SpawnObstacle()
	{
		_obstacle.SetActive(true);

		OnObstacleAppeared?.Invoke();

		_obstacleTimeRemaining = _obstacleTimeExistenceInSeconds;
	}

	private void DiactivateObject()
	{
		_obstacle.SetActive(false);

		OnObstacleDisappeared?.Invoke();

		_obstacleTimeRemaining = 0;
	}

	#endregion 

	#region Checkers

	private bool IsCanDiactivateObject()
		=> !IsPlayerLookingOnObstacle() && _obstacleTimeRemaining <= 0;

	private bool IsCanSpawnObstacle()
		=> !IsObstacleInterfering() && !IsPlayerLookingOnObstacle() && !_isPlayerInterfering;

	private bool IsObstacleInterfering()
	{
		foreach (var obstacle in _interferingObstacles)
		{
			if (obstacle.activeSelf)
				return true;	
		}
		
		return false;
	}

	private bool IsPlayerLookingOnObstacle()
	{
		Vector3 forwardVectorTowardsCamera = (_playerCamera.transform.position - transform.position).normalized;

		float dotProduct = Vector3.Dot(_playerCamera.transform.forward, forwardVectorTowardsCamera);

		return dotProduct < -0.5f;
	}

	#endregion
}
