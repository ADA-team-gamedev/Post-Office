using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(FieldOfView))]
public class BoxEnemy : MonoBehaviour, IPickable
{
	[Header("Player Sanity")]
	[SerializeField] private PlayerSanity _playerSanity;

	#region Values

	[Header("Values")]
	[SerializeField] private float _patrolPointsRestDelay = 10f;

	[SerializeField] private float _patrolingSpeed = 3f;
	[SerializeField] private float _runningSpeed = 6f;

	[Header("Phases starts")]

	[SerializeField] private float _patrolPhaseStart = 40f;
	[SerializeField] private float _attackPhaseStart = 10f;

	[Space(10)]
	[SerializeField] private float _tranfromToEnemyDelay = 5f;

	#endregion

	[Header("Patrol points")]
	[SerializeField] private List<Transform> _patrolPoints;

	[Header("Hidden spots")]
	[SerializeField] private List<Transform> _hiddenPoints;

	private Vector3 _currentPointToMove;
	private int _currentPatrolPointIndex = 0;

	private EnemyState _enemyState = EnemyState.Patroling;

	private NavMeshAgent _agent;
	private FieldOfView _fieldOfView;

	//flags
	private bool _isFleeing = false;
	private bool _isPatroling = false;
	private bool _isWaiting = false;
	private bool _canAttackPlayer = false;

	private bool _isEnemyPhasesStart = false;

	private bool _isPicked = false;
	private bool _isTranformedIntoInsect = false;

	private void Awake()
	{
		_currentPointToMove = transform.position;
		
		_agent.speed = _patrolingSpeed;

		DisableAI();
	}

	private void Update()
	{
		if (_isPicked)
			return;

		if (!_isEnemyPhasesStart && _playerSanity.Sanity <= _patrolPhaseStart)
		{
			_isEnemyPhasesStart = true;

			EnableAI();
		}

		if (_isEnemyPhasesStart && _isTranformedIntoInsect)
		{
			CheckVision();

			HandleStateLauncher();
		}				
	}

	private void CheckVision()
	{
		_fieldOfView.VisionCheck();

		if (_fieldOfView.CanSeePLayer)	
			_enemyState = _playerSanity.Sanity <= _attackPhaseStart ? EnemyState.Attacking : EnemyState.Fleeing;		
	}

	private void HandleStateLauncher()
	{
		switch (_enemyState)
		{
			case EnemyState.Patroling:
				Patroling();
				break;
			case EnemyState.Fleeing:
				Fleeing();
				break;
			case EnemyState.Idle:
				if (!_isWaiting)
				{
					_isWaiting = true;

					StartCoroutine(TakeRestOnTime(_patrolPointsRestDelay));
				}
				break;
			case EnemyState.Attacking:
				Attacking();
				break;
			default:
				Debug.Log($"{gameObject} doesn't have state - {_enemyState}");
				break;
		}
	}

	#region Phases

	#region Patrol

	private void MoveTo()
	{
		if (_patrolPoints.Count <= 0)
			return;

		_currentPatrolPointIndex = Random.Range(0, _patrolPoints.Count); //possible simillar points

		_currentPointToMove = _patrolPoints[_currentPatrolPointIndex].position;

		_agent.SetDestination(_currentPointToMove);
	}

	private void Patroling()
	{
		if (_isPatroling)
		{
			if (_agent.remainingDistance <= _agent.stoppingDistance)
			{
				_isPatroling = false;

				_enemyState = EnemyState.Idle;
			}

			return;
		}

		_isPatroling = true;

		_agent.speed = _patrolingSpeed;

		MoveTo();
	}

	private IEnumerator TakeRestOnTime(float delay)
	{
		yield return new WaitForSeconds(delay);

		_isWaiting = false;

		_enemyState = EnemyState.Patroling;
	}

	#endregion

	#region Fleeing

	private void Fleeing()
	{
		if (_isFleeing)
			return;

		StopAllCoroutines();

		_isPatroling = false;
		_isFleeing = true;

		_agent.speed = _runningSpeed;

		_agent.SetDestination(_hiddenPoints[Random.Range(0, _hiddenPoints.Count)].position);	
	}

	#endregion

	#region Attacking(WIP)

	private void Attacking()
	{
		if (_canAttackPlayer)
			return;

		StopAllCoroutines();

		_canAttackPlayer = true;

		Debug.LogWarning("Enemy attack phase still in progress");
	}

	#endregion

	#endregion

	#region Inventory interaction

	#region PickUp

	public void PickUpItem()
	{
		StopAllCoroutines();

		_isPicked = true;

		DisableAI();
	}

	private void DisableAI()
	{
		_isTranformedIntoInsect = false;

		_agent.enabled = true;

		_agent.isStopped = true;

		_agent.enabled = false;

		_isPatroling = false;
		_isFleeing = false;
		_isWaiting = false;

		_enemyState = EnemyState.None;
	}

	#endregion

	#region Drop

	public void DropItem()
	{
		_isPicked = false;

		if (_isEnemyPhasesStart)
			StartCoroutine(TransformFromBoxToInsect(_tranfromToEnemyDelay));		
	}

	private IEnumerator TransformFromBoxToInsect(float delay)
	{
		yield return new WaitForSeconds(delay);	

		EnableAI();
	}

	private void EnableAI()
	{
		_isTranformedIntoInsect = true;

		_agent.enabled = true;

		_agent.isStopped = false;

		_enemyState = EnemyState.Patroling;
	}

	#endregion

	#endregion

	private void OnValidate()
	{
		_agent ??= GetComponent<NavMeshAgent>();
		_agent.speed = _patrolingSpeed;

		_fieldOfView ??= GetComponent<FieldOfView>();
		
		if (_patrolingSpeed >= _runningSpeed)
			_runningSpeed = _patrolingSpeed + 1;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;

		foreach (var point in _patrolPoints)
			Gizmos.DrawSphere(point.position, 0.3f);

		Gizmos.color = Color.green;

		foreach (var point in _hiddenPoints)
			Gizmos.DrawSphere(point.position, 0.3f);
	}
}
