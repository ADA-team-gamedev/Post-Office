using Items;
using Level.Doors;
using Level.Lights.Lamps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Player
{
	[RequireComponent(typeof(PlayerDeathController))]
	public class Interactor : MonoBehaviour
	{
		[field: SerializeField] public Camera PlayerCamera { get; private set; }

		[field: SerializeField, Range(0.5f, 10f)] public float InteractionDistance { get; private set; } = 3f;

		[Header("Crosshair")]
		[SerializeField] private Image _crosshairImage;

		[Header("Crosshair sprites")]
		[SerializeField] private Sprite _defaultCrosshair;
		[SerializeField] private Sprite _crosshairLock;

		[SerializeField] private Color _defaultCrosshairColor = new(108, 108, 108, 255);
		[SerializeField] private Color _interactableCrosshairColor = new(152, 152, 152, 255);

		private PlayerDeathController _playerDeathController;

		private IInteractable _interactableObject;

		private Ray _interactionRay => new(PlayerCamera.transform.position, PlayerCamera.transform.forward);

		private bool _isHitInteractableObject = false;

		private PlayerInput _playerInput;

		[Inject]
		private void Construct(PlayerInput playerInput)
		{
			_playerInput = playerInput;

			_playerInput.Player.Interact.performed += OnStartInteract;
			_playerInput.Player.Interact.canceled += OnStopInteract;
		}

		private void Start()
		{
			_crosshairImage.sprite = _defaultCrosshair;

			_crosshairImage.color = _defaultCrosshairColor;

			_playerDeathController = GetComponent<PlayerDeathController>();

			_playerDeathController.OnDied += DisableInteractor;
		}

		private void Update()
		{
			OnUpdateInteract();
		}

		private void FixedUpdate()
		{
			ChangeCrosshair();
		}

		private void OnUpdateInteract()
		{
			if (!_playerInput.Player.Interact.IsPressed())
				return;

			_interactableObject?.UpdateInteract();
		}

		private void ChangeCrosshair()
		{
			bool isHitted = Physics.Raycast(_interactionRay, out RaycastHit hit, InteractionDistance);

			bool isHighlightable = isHitted && hit.transform.TryGetComponent(out IHighlightable highlightableObject) && highlightableObject.IsHighlightable;
			
			if (isHitted)
			{
				if (isHighlightable)
					_crosshairImage.color = _interactableCrosshairColor;
				else
					_crosshairImage.color = _defaultCrosshairColor;

				bool isClosedDoor = hit.transform.TryGetComponent(out Door door) && door.IsClosed;

				if (isClosedDoor)
					_crosshairImage.sprite = _crosshairLock;
				else
					_crosshairImage.sprite = _defaultCrosshair;
			}
			else
			{
				_crosshairImage.color = _defaultCrosshairColor;

				_crosshairImage.sprite = _defaultCrosshair;
			}	
		}

		#region Input Actions

		private void OnStartInteract(InputAction.CallbackContext context)
		{
			if (Physics.Raycast(_interactionRay, out RaycastHit hit, InteractionDistance))
			{
				if (hit.collider.TryGetComponent(out _interactableObject))				
					_interactableObject.StartInteract();			
			}
		}	

		private void OnStopInteract(InputAction.CallbackContext context)
		{
			_interactableObject?.StopInteract();

			_interactableObject = null;	
		}

		#endregion

		private void DisableInteractor()
		{
			Destroy(this);

			_playerDeathController.OnDied -= DisableInteractor;
		}

		private void OnDrawGizmosSelected()
		{
			if (_isHitInteractableObject)
				Gizmos.color = Color.green;
			else
				Gizmos.color = Color.red;

			Gizmos.DrawRay(_interactionRay.origin, _interactionRay.direction * InteractionDistance);
		}

		private void OnDestroy()
		{
			if (_playerInput != null)
			{
				_playerInput.Player.Interact.performed -= OnStartInteract;
				_playerInput.Player.Interact.canceled -= OnStopInteract;
			}
		}
	}
}
