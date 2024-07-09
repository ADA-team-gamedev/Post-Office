using Items;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityModification;

namespace Enemy
{
	[RequireComponent(typeof(Box))]
	[RequireComponent(typeof(FieldOfView))]
	[RequireComponent(typeof(NavMeshAgent))]
	public class BoxEnemy : MonoBehaviour
	{
		[SerializeField] private bool _isAliveBox = true;

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
		[field: SerializeField, Space(10)] public float TranfromToBoxDelay { get; private set; } = 1.5f;

		#endregion

		#region Animation
		[Header("Animation Settings")]
		[SerializeField] private Animator _animator;

		[SerializeField] private string BecomeInsectTrigger = "BecomeInsect";
		[SerializeField] private string BecomeBoxTrigger = "BecomeBox";

		[SerializeField] private string IsMoving = "IsMoving";

		[Space(10), SerializeField] private GameObject _killerBoxJumpScare;

		[SerializeField] private float _patrolingAnimationSpeed = 1;
		[SerializeField] private float _fleeingAnimationSpeed = 1;

		private float _defaultAnimationSpeed;

		#endregion

		[Header("Patrol points")]
		[SerializeField] private List<Transform> _patrolPoints;

		[Header("Hidden spots")]
		[SerializeField] private List<Transform> _hiddenPoints;

		private Vector3 _currentPointToMove;

		private EnemyState _enemyState = EnemyState.Patroling;

		private Transform _attackingTarget;

		private NavMeshAgent _agent;
		private Rigidbody _rigidbody;
		private FieldOfView _fieldOfView;

		private Box _boxItem;

		//flags
		private bool _isFleeing = false;
		private bool _isReachedHidenPoint = false;

		private bool _isPatroling = false;
		private bool _isWaiting = false;

		public bool IsPicked { get; private set; } = false;

		private bool _isEnemyPhasesStarts = false; //only for first enemy start moment in update

		private void Awake()
		{
			_agent = GetComponent<NavMeshAgent>();
			_agent.speed = _patrolingSpeed;
			
			_fieldOfView = GetComponent<FieldOfView>();

			_boxItem = GetComponent<Box>();

			_rigidbody = GetComponent<Rigidbody>();

			_currentPointToMove = transform.position;

			_agent.speed = _patrolingSpeed;

			DisableAI();

			_boxItem.OnObjectDestroyed += OnItemDestroyed;

			_boxItem.OnPickUpItem += PickUpItem;

			_boxItem.OnDropItem += DropItem;

			_boxItem.OnItemPickingPropertyChanged += OnItemPickingPropertyChanged;

			_defaultAnimationSpeed = _animator.speed;
		}

		private void Update()
		{
			if (IsPicked)
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

			if (_boxItem.ItemIcon.IsIconEnabled()) //For task. If task enable item icons we update icon position
				_boxItem.ItemIcon.ShowIcon(_boxItem);		
		}

		#region Checker

		public bool IsCanActivateEnemy()
			=> !IsPicked && IsOnGround();

		private bool IsCanStartEnemyLogic()
			=> !IsPicked && (_playerSanity.SanityPercent <= _patrolPhaseStartSanityPercent) && IsOnGround() && _isAliveBox;

		private bool IsOnGround()
			=> Physics.Raycast(transform.position, Vector3.down, out RaycastHit _, _groundCheckDistance, _groundLayer);		

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
					EditorDebug.Log($"{gameObject} doesn't have state - {_enemyState}");
					break;
			}
		}

		#region Phases

		#region Patrol

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
			
			if (TryMoveTo(_patrolPoints, _patrolingSpeed, _patrolingAnimationSpeed))
				_isPatroling = true;		
		}

		private IEnumerator TakeRestOnTime(float delay)
		{
			yield return new WaitForSeconds(delay);

			_isWaiting = false;

			_enemyState = EnemyState.Patroling;
		}

		private bool TryMoveTo(Vector3 point, float agentSpeed, float animationSpeed)
		{
			if (IsReachablePoint(point))
			{
				_currentPointToMove = point;

				_agent.speed = agentSpeed;

				_animator.speed = animationSpeed;

				_agent.SetDestination(_currentPointToMove);

				_animator.SetBool(IsMoving, true);

				return true;
			}

			return false;
		}

		private bool TryMoveTo(List<Transform> points, float agentSpeed, float animationSpeed)
		{
			if (TryGetPoint(points, out Vector3 pointToMove))
			{
				_currentPointToMove = pointToMove;

				_agent.speed = agentSpeed;

				_animator.speed = animationSpeed;

				_agent.SetDestination(_currentPointToMove);

				_animator.SetBool(IsMoving, true);

				return true;
			}

			return false;
		}

		private bool TryGetPoint(List<Transform> points, out Vector3 pointToMove)
		{
			if (points.Count <= 0)
			{
				pointToMove = Vector3.zero;

				return false;
			}

			List<Transform> possiblePointsToMove = new(points.Count);

			possiblePointsToMove.AddRange(points);

			int randomPointIndex;

			Transform randomPoint;

			while (possiblePointsToMove.Count > 0)
			{
				randomPointIndex = Random.Range(0, possiblePointsToMove.Count);

				randomPoint = possiblePointsToMove[randomPointIndex];

				if (IsReachablePoint(randomPoint.position))
				{
					pointToMove = randomPoint.position;

					return true;
				}

				possiblePointsToMove.Remove(randomPoint);
			}

			EditorDebug.LogError($"No reachable points in {points} collection!");

			pointToMove = Vector3.zero;

			return false;
		}

		private bool IsReachablePoint(Vector3 point)
		{
			NavMeshPath path = new();

			return _agent.CalculatePath(point, path) && path.status == NavMeshPathStatus.PathComplete;
		}

		#endregion

		#region Fleeing

		private void Fleeing()
		{
			if (_isFleeing)
			{
				if (!_isReachedHidenPoint && _agent.remainingDistance <= _agent.stoppingDistance) 
				{
					_animator.SetBool(IsMoving, false);

					_agent.speed = _patrolingSpeed;

					_animator.speed = _defaultAnimationSpeed;

					_isReachedHidenPoint = true;
				}

				return;
			}

			_isPatroling = false;

			_isReachedHidenPoint = false;

			_animator.speed *= 1.5f;

			if (TryMoveTo(_hiddenPoints, _runningSpeed, _fleeingAnimationSpeed))
			{
				_isFleeing = true;		
			}
			else
			{
				EditorDebug.LogError($"Can't start fleeing because the {gameObject.name} doesn't have points to hide");
			}
		}

		#endregion

		#region Attacking

		public void OrderToAttack(Vector3 point)
		{
			if (!IsAIActivated)
				return;

			StopAllCoroutines();

			if (TryMoveTo(point, _attackingSpeed, _patrolingAnimationSpeed))
			{
				_enemyState = _playerSanity.SanityPercent <= _attackPhaseStartSanityPercent ? EnemyState.Attacking : EnemyState.Patroling;

				_isPatroling = true;

				_isWaiting = false;
				_isFleeing = false;
			}
			else
			{
				EditorDebug.Log($"Can't go to that point({point}), because i can't reach it!");
			}
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

			if (!_fieldOfView.Target.TryGetComponent(out PlayerDeathController playerDeath))
			{
				EditorDebug.LogError($"No {nameof(PlayerDeathController)} in the target({_fieldOfView.Target})");

				return;
			}

			playerDeath.Die();

			_killerBoxJumpScare.SetActive(true);

			gameObject.SetActive(false);
		}

		#endregion

		#endregion

		#region Inventory interaction

		#region PickUp

		private void PickUpItem(Item item)
		{
			if (IsAIActivated)
				_animator.SetTrigger(BecomeBoxTrigger);

			DisableAI();	
			
			StartCoroutine(TransformFromInsectToBox(TranfromToBoxDelay));	
		}

		private IEnumerator TransformFromInsectToBox(float delay)
		{
			yield return new WaitForSeconds(delay);

			IsPicked = true;
		}

		private void DisableAI()
		{
			_attackingTarget = null;

			StopAllCoroutines();

			IsAIActivated = false;

			_agent.enabled = true;

			_agent.isStopped = true;

			_agent.enabled = false;

			_isPatroling = false;
			_isFleeing = false;
			_isWaiting = false;

			_isReachedHidenPoint = false;

			_enemyState = EnemyState.None;
		}	

		#endregion

		#region Drop

		private void DropItem(Item item)
		{
			IsPicked = false;
			
			if (IsCanStartEnemyLogic())
				StartCoroutine(TransformFromBoxToInsect(TranfromToEnemyDelay));
		}

		private IEnumerator TransformFromBoxToInsect(float delay)
		{
			if (!IsAIActivated)
			{
				_animator.speed = _defaultAnimationSpeed;

				yield return new WaitForSeconds(delay / 2);

				_animator.SetTrigger(BecomeInsectTrigger);

				yield return new WaitForSeconds(delay / 2);

				EnableAI();
			}		
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

		private void OnItemPickingPropertyChanged()
		{
			StopAllCoroutines();

			_isAliveBox = _boxItem.CanBePicked;
		}

		#endregion

		public void ActivateEnemyBox()
		{
			if (!gameObject.activeInHierarchy || !IsCanActivateEnemy())
				return;
			
			IsPicked = false;

			StartCoroutine(TransformFromBoxToInsect(TranfromToEnemyDelay));
		}

		private void OnItemDestroyed(Item item)
		{
			_boxItem.OnObjectDestroyed -= OnItemDestroyed;

			_boxItem.OnPickUpItem -= PickUpItem;

			_boxItem.OnDropItem -= DropItem;

			_boxItem.OnItemPickingPropertyChanged -= OnItemPickingPropertyChanged;
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
			{
				if (point)
					Gizmos.DrawSphere(point.position, 0.3f);
			}

			Gizmos.color = Color.green;

			foreach (var point in _hiddenPoints)
			{
				if (point)
					Gizmos.DrawSphere(point.position, 0.3f);
			}
		}
	}
}
