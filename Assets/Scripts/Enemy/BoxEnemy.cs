using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Box))]
[RequireComponent(typeof(FieldOfView))]
[RequireComponent(typeof(NavMeshAgent))]
public class BoxEnemy : MonoBehaviour
{
	[Header("Player Sanity")]
	[SerializeField] private PlayerSanity _playerSanity;

	#region Values

	[Header("Values")]
	[SerializeField] private float _patrolPointsRestDelay = 10f;

	[SerializeField] private float _patrolingSpeed = 4f;
	[SerializeField] private float _attackingSpeed = 4f;
	[SerializeField] private float _runningSpeed = 6f;

	[Header("Phases starts")]

	[SerializeField] private float _patrolPhaseStart = 40f;
	[SerializeField] private float _attackPhaseStart = 10f;

	[Space(10)]
	[SerializeField] private float _tranfromToEnemyDelay = 5f;

	#endregion

	#region Animation
	[Header("Animation Settings")]	
	[SerializeField] private Animator _animator;

	[SerializeField] private string BecomeInsectTrigger = "BecomeInsect";
	[SerializeField] private string BecomeBoxTrigger = "BecomeBox";

	[SerializeField] private string IsMoving = "IsMoving";

	[SerializeField] private string KillAnimationTrigger = "Jumpscare";

	private float _defaultAnimationSpeed;

	#endregion

	[Header("Patrol points")]
	[SerializeField] private List<Transform> _patrolPoints;

	[Header("Hidden spots")]
	[SerializeField] private List<Transform> _hiddenPoints;

	private Vector3 _currentPointToMove;
	private int _currentPatrolPointIndex = 0;

	private EnemyState _enemyState = EnemyState.Patroling;

	private Transform _attackingTarget;

	private NavMeshAgent _agent;
	private Rigidbody _rigidbody;
	private FieldOfView _fieldOfView;

	private Box _box;

	//flags
	private bool _isFleeing = false;
	private bool _isPatroling = false;
	private bool _isWaiting = false;

	private bool _isPicked = false;

	private bool _isTranformedIntoInsect = false;

	private bool _isEnemyPhasesStarts = false; //only for first enemy start moment in update

	private void Awake()
	{
		_currentPointToMove = transform.position;
		
		_agent.speed = _patrolingSpeed;

		DisableAI();

		_box.OnPickUpItem += PickUpItem;

		_box.OnDropItem += DropItem;

		_defaultAnimationSpeed = _animator.speed;
	}

	private void Update()
	{
		if (_isPicked)
			return;

		if (!_isEnemyPhasesStarts && IsCanStartEnemyLogic()) //AI first start check
		{
			_isEnemyPhasesStarts = true;

			EnableAI();
		}

		if (_isTranformedIntoInsect)
		{
			CheckVision();

			HandleStateLauncher();
		}				
	}

	private bool IsCanStartEnemyLogic()
		=> !_isTranformedIntoInsect && _playerSanity.Sanity <= _patrolPhaseStart;

	private void CheckVision()
	{
		_fieldOfView.VisionCheck();

		if (_fieldOfView.InstantDetectTarget && _enemyState == EnemyState.Attacking)
			KillPlayer();
		else if (_fieldOfView.CanSeePLayer || _fieldOfView.InstantDetectTarget)
		{
			if (!_isFleeing)
			{
				_enemyState = _playerSanity.Sanity <= _attackPhaseStart ? EnemyState.Attacking : EnemyState.Fleeing;

				if (_enemyState == EnemyState.Attacking && !_attackingTarget)
					_attackingTarget = _fieldOfView.TargetTransform;
			}				
		}
		else //don't see player
		{
			if (_enemyState == EnemyState.Attacking && _attackingTarget)
			{
				_attackingTarget = null;

				if (_agent.remainingDistance <= _agent.stoppingDistance)
					_enemyState = EnemyState.Idle;
			}
		}				
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

					_animator.SetBool(IsMoving, false);

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

		_animator.SetBool(IsMoving, true);

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
		{
			if (_agent.remainingDistance <= _agent.stoppingDistance)
			{
				_animator.SetBool(IsMoving, false);
			}

			return;
		}

		StopAllCoroutines();

		_isPatroling = false;
		_isFleeing = true;

		_agent.speed = _runningSpeed;

		_animator.speed *= 1.5f;

		_animator.SetBool(IsMoving, true);

		if (_hiddenPoints.Count > 0)
		{
			_currentPointToMove = _hiddenPoints[Random.Range(0, _hiddenPoints.Count)].position;

			_agent.SetDestination(_currentPointToMove);
		}
		else
		{
			Debug.Log($"Can't start fleeing because the {gameObject} doesn't have points to hide");
		}
	}

	#endregion

	#region Attacking

	private void Attacking()
	{
		if (!_attackingTarget)
		{
			if (_agent.remainingDistance <= _agent.stoppingDistance)
			{
				_isPatroling = true; //for cases when we lost player. (watch patroling method)

				Patroling();
			}

			return;
		}

		StopAllCoroutines();

		_agent.speed = _attackingSpeed;

		_isWaiting = false;
		_isPatroling = false; 

		_agent.SetDestination(_attackingTarget.position);
	}

	private void KillPlayer()
	{
		_animator.SetTrigger(KillAnimationTrigger);	

		DisableAI();

		gameObject.SetActive(false);
	}

	#endregion

	#endregion

	#region Inventory interaction

	#region PickUp

	public void PickUpItem()
	{
		_isPicked = true;

		_attackingTarget = null;

		DisableAI();
	}

	private void DisableAI()
	{
		StopAllCoroutines();

		if (_isTranformedIntoInsect)
			_animator.SetTrigger(BecomeBoxTrigger);

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

		if (IsCanStartEnemyLogic())
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

		_rigidbody.isKinematic = true;

		_animator.speed = _defaultAnimationSpeed;

		_animator.SetTrigger(BecomeInsectTrigger);
	}

	#endregion

	#endregion

	private void OnValidate()
	{
		_agent ??= GetComponent<NavMeshAgent>();
		_agent.speed = _patrolingSpeed;

		_fieldOfView ??= GetComponent<FieldOfView>();

		_box ??= GetComponent<Box>();

		_rigidbody ??= GetComponent<Rigidbody>();

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
