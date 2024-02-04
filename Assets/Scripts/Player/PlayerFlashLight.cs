using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlashLight : MonoBehaviour
{
	[SerializeField][Range(1f, 10f)] private float _rotationSpeed = 1f;

	[SerializeField] private PlayerInventory _playerInventory;
	[SerializeField] private Camera _playerCamera;

    private Light _light;

	private bool _isFlashLightPickedUp = false;

	private PlayerInput _playerInput;

	private void Awake()
	{
		_playerInput = new();

		_playerInput.Player.FlahsLight.performed += UseFlashLight;
	}

	private void Start()
	{
		_light = GetComponent<Light>();

		_light.enabled = false;

		_playerInventory.OnItemPicked += OnItemPicked;
		_playerInventory.OnItemDroped += OnItemDroped;

		_playerInventory.OnItemChanged += OnItemChanged;

		transform.rotation = _playerCamera.transform.rotation;
	}

	private void Update()
	{
		transform.position = _playerCamera.transform.position;

		RotateLight();
	}

	private void RotateLight()
	{
		if (!_isFlashLightPickedUp || !_light.enabled)
		{
			transform.rotation = _playerCamera.transform.rotation;

			return;
		}

		transform.rotation = Quaternion.Lerp(transform.rotation, _playerCamera.transform.rotation, _rotationSpeed * Time.deltaTime);
	}

	#region Inventory Actions

	private void OnItemPicked(Item item)
	{
		if (_isFlashLightPickedUp)
			return;

		_isFlashLightPickedUp = item.TryGetComponent(out FlashLight flashlight);
	}

	private void OnItemDroped(Item item)
	{
		if (!_isFlashLightPickedUp)
			return;

		if (item.TryGetComponent(out FlashLight flashLight))
		{
			_light.enabled = false;

			_isFlashLightPickedUp = false;
		}
	}

	private void OnItemChanged()
	{
		if (!_isFlashLightPickedUp)
			return;

		if (_playerInventory.TryGetCurrentItem(out FlashLight flashLight))
		{
			_light.enabled = false;
		}
		else
		{
			if (_playerInventory.TryGetItem(out flashLight))
				_light.enabled = flashLight.IsWorking;		
		}
	}

	private void UseFlashLight(InputAction.CallbackContext context)
	{
		if (!_isFlashLightPickedUp)
			return;

		if (!_playerInventory.TryGetCurrentItem(out FlashLight flashlight))
			_light.enabled = !_light.enabled;
	}

	#endregion

	private void OnEnable()
	{
		_playerInput.Enable();
	}

	private void OnDisable()
	{
		_playerInput.Disable();
	}
}
