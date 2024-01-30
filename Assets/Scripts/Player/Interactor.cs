using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
	[field: SerializeField] public Camera PlayerCamera { get; private set; }

	public float InteractionDistance { get; private set; } = 3f;

	private PlayerInput _playerInput;

	private IInteractable _interactableObject;

	private void Awake()
	{
		_playerInput = new();

		_playerInput.Player.Interact.performed += OnStartInteract;
		_playerInput.Player.Interact.canceled += OnStopInteract;
	}

	private void OnStartInteract(InputAction.CallbackContext context)
	{
		if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out RaycastHit hit, InteractionDistance))
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

	private void OnEnable()
	{
		_playerInput.Enable();
	}

	private void OnDisable()
	{
		_playerInput.Disable();
	}
}
