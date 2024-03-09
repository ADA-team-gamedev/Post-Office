using UnityEngine;

[System.Serializable]
public class Icon
{
	[SerializeField] private bool _isIconNeeded = true;

	[field: SerializeField] protected GameObject ItemIcon { get; private set; }

	[field: SerializeField] protected Transform IconTargetToLook { get; private set; }

	public bool IsIconEnabled => ItemIcon.activeSelf;

	public void RotateIconToObject()
	{
		if (!_isIconNeeded || !IsIconEnabled)
			return;

		Vector3 targetPosition = IconTargetToLook.position;

		targetPosition.y = ItemIcon.transform.position.y; //constrain y axis
		
		ItemIcon.transform.LookAt(targetPosition);
	}

	public void HideIcon()
	{
		if (!_isIconNeeded || !IsIconEnabled)
			return;
		
		ItemIcon.SetActive(false);
	}

	public void ShowIcon()
	{
		if (!_isIconNeeded || !IsIconEnabled)
			return;

		ItemIcon.SetActive(true);
	}
}
