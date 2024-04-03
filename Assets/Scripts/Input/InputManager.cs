using UnityEngine;

namespace InputSystem
{
	public class InputManager : MonoBehaviour
	{
		public static InputManager Instance;

		public PlayerInput PlayerInput { get; private set; }

		private void Awake()
		{
			if (!Instance)
				Instance = this;
			else
				Debug.LogWarning($"{this} Instance already exists!");

			PlayerInput = new();
			
			PlayerInput.Enable();
		}

		private void OnEnable()
		{
			PlayerInput.Enable();
		}

		private void OnDisable()
		{
			PlayerInput.Disable();
		}
	}
}