using System;
using UnityEngine;

public class ObstacleAppearance : MonoBehaviour
{
	#region Onstacle

	[Header("Obstacle")]
    [SerializeField][Range(0.01f, 1)] private float _sanityPercentToStart = 0.5f;

	[SerializeField] private float _obstacleTimeExistenceInSeconds = 10f;
	private float _obstacleTimeRemaining = 0;

	[SerializeField] private GameObject _obstacle;

	[SerializeField] private GameObject[] _interferingObstacles;

	#endregion

	[Header("Sanity")]
	[SerializeField] private PlayerSanity _playerSanity;

	[Header("Player")]
	[SerializeField] private Camera _playerCamera;

	public event Action OnObstacleDisappeared;
	public event Action OnObstacleAppeared;

	private void Start()
	{
		_obstacle.SetActive(false);
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

	#region Checkers

	private bool IsCanDiactivateObject()
		=> !IsPlayerLookingOnObstacle() && _obstacleTimeRemaining <= 0;

	private bool IsCanSpawnObstacle()
		=> !IsObstacleInterfering() && !IsPlayerLookingOnObstacle();

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
