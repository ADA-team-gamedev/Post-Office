using System;
using System.Collections.Generic;
using UnityEngine;

public enum InputType
{
	Interact,
	Drop,
	Drag,
}
public class PlayerHandler : MonoBehaviour
{
	public static PlayerHandler Instance {  get; private set; }

	public Dictionary<InputType, KeyBind> KeyBinds { get; private set; } = new()
	{
		{ InputType.Drop, new KeyBind(KeyCode.G) },
		{ InputType.Interact, new KeyBind(KeyCode.E) },
		{ InputType.Drag, new KeyBind(KeyCode.Mouse0) },
	};

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Debug.LogWarning("PlayerHandler Instance already exists!");
	}

	private void Update()
	{
		UpdateKeys();
	}

	private void UpdateKeys()
	{
		foreach (var bind in KeyBinds.Values)
		{
			bind.UpdateKeyBind();
		}
	}
}

public class KeyBind
{
	public KeyCode KeyCode;

	public Action OnKeyDown;
	public Action OnKeyHold;
	public Action OnKeyUp;

	public KeyBind(KeyCode keycode)
	{
		KeyCode = keycode;
	}

	public void UpdateKeyBind()
	{
		if (Input.GetKeyDown(KeyCode))
			OnKeyDown?.Invoke();
		if (Input.GetKey(KeyCode))
			OnKeyHold?.Invoke();
		if (Input.GetKeyUp(KeyCode))
			OnKeyUp?.Invoke();
	}
}
