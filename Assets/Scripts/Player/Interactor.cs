using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Interactor : MonoBehaviour
{
	[field: SerializeField] public Camera PlayerCamera { get; private set; }

	public float InteractionDistance { get; private set; } = 3f;

	[Header("Crosshair")]
	[SerializeField] private Image _crosshairImage;

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

		_crosshairImage.color = _defaultCrosshairColor;

		_playerDeathController = GetComponent<PlayerDeathController>();

		_playerDeathController.OnDeath += DisableInteractor;
	}

	private void Update()
	{
		PaintCrossHair();
	}

	private void PaintCrossHair()
	{
		bool isHitted = Physics.Raycast(_interactionRay, out RaycastHit hit, InteractionDistance);

		bool isInteractable = false;

		bool isHaveInteractableParent = false;

		if (isHitted)
		{
			isInteractable = hit.collider.TryGetComponent(out IInteractable _) || hit.collider.TryGetComponent(out Item _);

			if (hit.transform) //hit object with col or parent object(door or something, what doesn't have collider on himself)
				isHaveInteractableParent = hit.transform.TryGetComponent(out IInteractable _) || hit.transform.TryGetComponent(out Item _);
		}

		_isHitInteractableObject = isHitted && (isInteractable || isHaveInteractableParent);

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
