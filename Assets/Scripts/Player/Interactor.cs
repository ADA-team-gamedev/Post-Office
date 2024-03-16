using Items;
using Level.Doors;
using Level.Lights.Lamp;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Player
{
	[RequireComponent(typeof(PlayerDeathController))]
	public class Interactor : MonoBehaviour
	{
		[field: SerializeField] public Camera PlayerCamera { get; private set; }

		public float InteractionDistance { get; private set; } = 3f;

		[Header("Crosshair")]
		[SerializeField] private Image _crosshairImage;

		[Header("Crosshair sprites")]
		[SerializeField] private Sprite _defaultCrosshair;
		[SerializeField] private Sprite _crosshairLock;

		[SerializeField] private Color _defaultCrosshairColor = new(108, 108, 108, 255);
		[SerializeField] private Color _interactableCrosshairColor = new(152, 152, 152, 255);

		private PlayerInput _playerInput;

		private PlayerDeathController _playerDeathController;

		private IInteractable _interactableObject;

		private Ray _interactionRay => new(PlayerCamera.transform.position, PlayerCamera.transform.forward);

		private bool _isHitInteractableObject = false;

		private void Awake()
		{
			_playerInput = new();

			_playerInput.Player.Interact.performed += OnStartInteract;
			_playerInput.Player.Interact.canceled += OnStopInteract;

			_crosshairImage.sprite = _defaultCrosshair;

			_crosshairImage.color = _defaultCrosshairColor;

			_playerDeathController = GetComponent<PlayerDeathController>();

			_playerDeathController.OnDeath += DisableInteractor;
		}

		private void Update()
		{
			ChangeCrossHair();
		}

		private void ChangeCrossHair()
		{
			bool isHitted = Physics.Raycast(_interactionRay, out RaycastHit hit, InteractionDistance);

			bool isHaveInteractableParent = false;

			if (isHitted)
			{
				bool isPickableItem = hit.collider.TryGetComponent(out Item item) && item.CanBePicked;

				bool isInteractable = hit.collider.TryGetComponent(out IInteractable _) || hit.collider.TryGetComponent(out Lamp _);

				if (hit.transform) //hit object with col or parent object(door or something, what doesn't have collider on himself)
				{
					isHaveInteractableParent = hit.transform.TryGetComponent(out IInteractable _);

					if (hit.transform.TryGetComponent(out Door door) && door.IsClosed)
						_crosshairImage.sprite = _crosshairLock;
					else
						_crosshairImage.sprite = _defaultCrosshair;
				} 

				_isHitInteractableObject = isInteractable || isHaveInteractableParent || isPickableItem;
			}
			else
			{
				_isHitInteractableObject = false;

				_crosshairImage.sprite = _defaultCrosshair;
			}

			if (_isHitInteractableObject)
				_crosshairImage.color = _interactableCrosshairColor;
			else
				_crosshairImage.color = _defaultCrosshairColor;
		}

		#region Input Actions

		private void OnStartInteract(InputAction.CallbackContext context)
		{
			if (Physics.Raycast(_interactionRay, out RaycastHit hit, InteractionDistance))
			{
				if (hit.collider.TryGetComponent(out _interactableObject))
				{
					_interactableObject.StartInteract();
				}
				else if (hit.transform) //hit object with col or parent object(door or something, what doesn't have collider on himself)
				{
					if (hit.transform.TryGetComponent(out _interactableObject))
						_interactableObject.StartInteract();
				}
			}
		}

		private void OnStopInteract(InputAction.CallbackContext context)
		{
			if (_interactableObject != null)
			{
				_interactableObject.StopInteract();

				_interactableObject = null;
			}
		}

		#endregion

		private void DisableInteractor()
		{
			Destroy(this);
		}

		private void OnEnable()
		{
			_playerInput.Enable();
		}

		private void OnDisable()
		{
			_playerInput.Disable();
		}

		private void OnDrawGizmosSelected()
		{
			if (_isHitInteractableObject)
				Gizmos.color = Color.green;
			else
				Gizmos.color = Color.red;

			Gizmos.DrawRay(_interactionRay.origin, _interactionRay.direction * InteractionDistance);
		}
	}
}
