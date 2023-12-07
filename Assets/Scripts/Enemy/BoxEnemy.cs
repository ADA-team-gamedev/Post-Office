using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(FieldOfView))]
[RequireComponent(typeof(SphereCollider))]
public class BoxEnemy : MonoBehaviour
{
	[Header("Layer")]
	[SerializeField] private LayerMask _playerLayer;

	#region Values

	[Header("Values")]
	[SerializeField] private float _patrolPointsRestDelay = 10f;

	[SerializeField] private float _walkingSpeed = 3f;
	[SerializeField] private float _runningSpeed = 6f;

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
	private SphereCollider _sphereCollider;

	private bool _isFleeing = false;
	private bool _isPatroling = false;
	private bool _isWaiting = false;

	private void Awake()
	{
		_currentPointToMove = transform.position;
		
		_agent.isStopped = false;
		_agent.speed = _walkingSpeed;

		_sphereCollider.isTrigger = true;
		_sphereCollider.radius = _fieldOfView.InstantDetectionRadius;
	}

	private void Update()
	{
		CheckVision();
		
		HandleStateLauncher();
	}

	private void CheckVision()
	{
		_fieldOfView.VisionCheck();

		if (_fieldOfView.CanSeePLayer)	
			_enemyState = EnemyState.Fleeing;		
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
			default:
				Debug.Log($"{gameObject} doesn't have state - {_enemyState}");
				break;
		}
	}

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
			if (!_isWaiting && _agent.remainingDistance <= _agent.stoppingDistance)
			{
				_isWaiting = true;

				StartCoroutine(TakeRestOnTime(_patrolPointsRestDelay));
			}

			return;
		}

		_isPatroling = true;

		MoveTo();
	}

	private IEnumerator TakeRestOnTime(float delay)
	{
		yield return new WaitForSeconds(delay);

		_isPatroling = false;

		_isWaiting = false;
	}

	#endregion

	#region Fleeing

	private void Fleeing()
	{
		if (_isFleeing)
			return;

		_isFleeing = true;

		_agent.speed = _runningSpeed;

		_agent.SetDestination(_hiddenPoints[Random.Range(0, _hiddenPoints.Count)].position);	
	}

	#endregion

	private void OnTriggerEnter(Collider other)
	{
		Vector3 directionToTarget = (other.transform.position - transform.position).normalized;

		float distanceToTarget = Vector3.Distance(transform.position, other.transform.position);

		if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit hit, distanceToTarget) && _playerLayer.value == 1 << hit.transform.gameObject.layer)
		{
			_enemyState = EnemyState.Fleeing;

			StopAllCoroutines();
		}
	}
	
	private void OnValidate()
	{
		_agent ??= GetComponent<NavMeshAgent>();
		_agent.speed = _walkingSpeed;

		_fieldOfView ??= GetComponent<FieldOfView>();
		
		_sphereCollider ??= GetComponent<SphereCollider>();

		_sphereCollider.isTrigger = true;
		_sphereCollider.radius = _fieldOfView.InstantDetectionRadius;
		
		if (_walkingSpeed >= _runningSpeed)
			_runningSpeed = _walkingSpeed + 1;
	}

}
