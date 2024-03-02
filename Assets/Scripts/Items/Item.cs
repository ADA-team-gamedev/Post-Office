using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Item : MonoBehaviour
{
	[field: Header("Item")]

	[field: SerializeField] public bool CanBePicked { get; set; } = true;

	#region Icon Properties

	[SerializeField] private GameObject _itemIcon;

	[SerializeField] private Transform _iconTargetToLook;

	[SerializeField] private bool _changeIconStateAutomatically = false;

	public bool IsIconEnabled => _itemIcon.activeSelf;

	#endregion

	public Action<Item> OnPickUpItem { get; set; }

	public Action<Item> OnDropItem { get; set; }

	private void Start()
	{		
		Initialize();
	}

	private void Update()
	{
		RotateIconToObject(_iconTargetToLook.position);
	}

	protected virtual void Initialize()
	{
		if (_changeIconStateAutomatically)
		{
			ActivateAutoIconStateChanging();

			ShowIcon();
		}
		else
		{
			HideIcon();
		}

		_itemIcon.transform.position = new(transform.position.x, _itemIcon.transform.position.y, transform.position.z);
	}

	#region Icon Logic

	protected void RotateIconToObject(Vector3 targetPosition)
	{
		if (!IsIconEnabled)
			return;

		targetPosition.y = _itemIcon.transform.position.y; //constrain y axis

		_itemIcon.transform.LookAt(targetPosition);
	}

	public void ActivateAutoIconStateChanging()
	{
		OnPickUpItem += HideIcon;

		OnDropItem += ShowIcon;
	}

	public void DeactivateAutoIconStateChanging()
	{
		OnPickUpItem -= HideIcon;

		OnDropItem -= ShowIcon;
	}

	public void HideIcon()
	{
		if (!IsIconEnabled)
			return;

		_itemIcon.SetActive(false);
	}

	public void ShowIcon()
	{
		if (IsIconEnabled)
			return;

		_itemIcon.SetActive(true);
		
		_itemIcon.transform.position = new(transform.position.x, _itemIcon.transform.position.y, transform.position.z);
	}

	private void HideIcon(Item item)
	{
		HideIcon();
	}

	private void ShowIcon(Item item)
	{
		ShowIcon();
	}

	#endregion
}