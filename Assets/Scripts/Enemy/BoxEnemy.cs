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

	[Header("AI check")]
	[SerializeField] private LayerMask _groundLayer;
	[SerializeField] private float _groundCheckDistance = 0.1f;

	public bool IsAIActivated { get; private set; } = false;

	[Header("Values")]
	[SerializeField] private float _patrolPointsRestDelay = 10f;

	[SerializeField] private float _patrolingSpeed = 4f;
	[SerializeField] private float _attackingSpeed = 4f;
	[SerializeField] private float _runningSpeed = 6f;

	[Header("Phases starts")]

	[SerializeField][Range(0.01f, 1f)] private float _patrolPhaseStartSanityPercent = 0.4f;
	[SerializeField][Range(0.01f, 1f)] private float _attackPhaseStartSanityPercent = 0.1f;

	[field: SerializeField, Space(10)] public float TranfromToEnemyDelay { get; private set; } = 5f;

	#endregion

	#region Animation
	[Header("Animation Settings")]	
	[SerializeField] private Animator _animator;

	[SerializeField] private string BecomeInsectTrigger = "BecomeInsect";
	[SerializeField] private string BecomeBoxTrigger = "BecomeBox";

	[SerializeField] private string IsMoving = "IsMoving";

	[Space(10), SerializeField] private GameObject _killerBoxJumpScare;

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

	private bool _isEnemyPhasesStarts = false; //only for first enemy start moment in update

	private void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
		_agent.speed = _patrolingSpeed;

		_fieldOfView = GetComponent<FieldOfView>();
		
		_box = GetComponent<Box>();

		_rigidbody = GetComponent<Rigidbody>();

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

			StartCoroutine(TransformFromBoxToInsect(TranfromToEnemyDelay));
		}

		if (IsAIActivated)
		{
			CheckVision();

			HandleStateLauncher();
		}
	}

	#region Checker

	public bool IsCanActivateEnemy()
		=> !_isPicked && IsOnGround();

	private bool IsCanStartEnemyLogic() 
		=> !_isPicked && (_playerSanity.SanityPercent <= _patrolPhaseStartSanityPercent) && IsOnGround();		

	private bool IsOnGround()
	{
		return Physics.Raycast(transform.position, Vector3.down, out RaycastHit _, _groundCheckDistance, _groundLayer);
	}

	private void CheckVision()
	{
		_fieldOfView.VisionCheck();

		if (_fieldOfView.InstantDetectTarget && _enemyState == EnemyState.Attacking)
		{
			KillPlayer();
		}
		else if (_fieldOfView.CanSeeTarget)
		{
			if (!_isFleeing)
			{
				_enemyState = _playerSanity.SanityPercent <= _attackPhaseStartSanityPercent ? EnemyState.Attacking : EnemyState.Fleeing;

				if (_enemyState == EnemyState.Attacking && !_attackingTarget)
					_attackingTarget = _fieldOfView.Target;
			}				
		}
		else //don't see player
		{
			if (_enemyState == EnemyState.Attacking)
			{
				_attackingTarget = null;

				if (_agent.remainingDistance <= _agent.stoppingDistance)
					_enemyState = EnemyState.Idle;
			}	
		}				
	}

	#endregion

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

	private void MoveTo(Vector3 point)
	{
		_currentPointToMove = point;

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

	public void OrderToAttack(Vector3 point)
	{
		if (!IsAIActivated)
			return;

		StopAllCoroutines();

		_enemyState = EnemyState.Attacking;

		_isWaiting = false;
		_isPatroling = false;
		_isFleeing = false;

		_agent.speed = _attackingSpeed;

		MoveTo(point);
	}

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

		_animator.SetBool(IsMoving, true);
	}

	private void KillPlayer()
	{
		DisableAI();

		_fieldOfView.Target.GetComponent<PlayerDeathController>().Die();

		_killerBoxJumpScare.SetActive(true);

		gameObject.SetActive(false);	
	}

	#endregion

	#endregion

	#region Inventory interaction

	#region PickUp

	private void PickUpItem(Item item)
	{
		_isPicked = true;

		_attackingTarget = null;

		DisableAI();
	}

	private void DisableAI()
	{
		StopAllCoroutines();

		if (IsAIActivated)
		{
			//_animator.speed = _runningSpeed;

			_animator.SetTrigger(BecomeBoxTrigger);
		}

		IsAIActivated = false;

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

	private void DropItem(Item item)
	{
		_isPicked = false;

		if (IsCanStartEnemyLogic())
			StartCoroutine(TransformFromBoxToInsect(TranfromToEnemyDelay));			
	}

	private IEnumerator TransformFromBoxToInsect(float delay)
	{
		_animator.speed = _defaultAnimationSpeed;

		_animator.SetTrigger(BecomeInsectTrigger);

		yield return new WaitForSeconds(delay);

		EnableAI();
	}

	private void EnableAI()
	{
		_agent.enabled = true;

		_agent.isStopped = false;

		_enemyState = EnemyState.Patroling;

		_rigidbody.isKinematic = true;

		IsAIActivated = true;
	}

	#endregion

	#endregion

	public void ActivateEnemyBox()
	{
		if (!gameObject.activeSelf || !IsCanActivateEnemy())
			return;

		_isPicked = false;

		StartCoroutine(TransformFromBoxToInsect(TranfromToEnemyDelay));
	}

	private void OnValidate()
	{
		if (_patrolingSpeed >= _runningSpeed)
			_runningSpeed = _patrolingSpeed + 1;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;

		Gizmos.DrawRay(transform.position, Vector3.down * _groundCheckDistance);

		foreach (var point in _patrolPoints)
			Gizmos.DrawSphere(point.position, 0.3f);

		Gizmos.color = Color.green;

		foreach (var point in _hiddenPoints)
			Gizmos.DrawSphere(point.position, 0.3f);
	}
}
