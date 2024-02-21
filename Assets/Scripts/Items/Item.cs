using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Item : MonoBehaviour
{
	public Action OnPickUpItem { get; set; }

	public Action OnDropItem { get; set; }

	[Header("Icon")]
	[SerializeField] private GameObject _itemIcon;

	[SerializeField] private Transform _player;

	public bool IsIconEnabled => _itemIcon.activeSelf;

	private void Start()
	{		
		Init();
	}

	private void Update()
	{
		RotateIconToObject(_player.position);
	}

	protected virtual void Init()
	{
		HideIcon();

		OnPickUpItem += HideIcon;

		//OnDropItem += ShowIcon;

		_itemIcon.transform.position = new(transform.position.x, _itemIcon.transform.position.y, transform.position.z);
	}

	protected void RotateIconToObject(Vector3 targetPosition)
	{
		if (!IsIconEnabled)
			return;

		targetPosition.y = _itemIcon.transform.position.y; //constrain y axis

		_itemIcon.transform.LookAt(targetPosition);
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
}
