using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Item : MonoBehaviour
{
	public Action OnPickUpItem { get; set; }

	public Action OnDropItem { get; set; }

	[SerializeField] private GameObject _itemIcon;

	public bool IsIconEnabled => _itemIcon.activeSelf;

	[SerializeField] private bool _subscribeIconOnItemMethods = true;

	[SerializeField] private float _iconRotationSpeed = 1f;

	private void Start()
	{		
		Init();
	}

	protected virtual void Init()
	{
		//HideIcon();

		if (_itemIcon && _subscribeIconOnItemMethods)
		{
			OnPickUpItem += HideIcon;

			OnDropItem += ShowIcon;
		}
	}

	protected void RotateIconToObject(Transform target)
	{
		if (!IsIconEnabled)
			return;

		Vector3 directionToTarget = target.position - _itemIcon.transform.position;

	}

	public void HideIcon()
	{
		_itemIcon.SetActive(false);
	}

	public void ShowIcon()
	{
		_itemIcon.SetActive(true);

		_itemIcon.transform.position = new(transform.position.x, _itemIcon.transform.position.y, transform.position.z);
	}
}
