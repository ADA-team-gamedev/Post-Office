using UnityEngine;

namespace Items.Icon
{
	[System.Serializable]
	public class Icon
	{
		[field: SerializeField] protected bool IsIconNeeded { get; private set; } = true;

		[field: SerializeField] protected GameObject ItemIcon { get; private set; }

		[field: SerializeField] protected Transform IconTargetToLook { get; private set; }

		public bool IsIconEnabled => ItemIcon.activeSelf;

		public void RotateIconToObject()
		{
			if (!IsIconNeeded || !IsIconEnabled)
				return;

			Vector3 targetPosition = IconTargetToLook.position;

			targetPosition.y = ItemIcon.transform.position.y; //constrain y axis

			ItemIcon.transform.LookAt(targetPosition);
		}

		public virtual void HideIcon()
		{
			if (!IsIconNeeded || !IsIconEnabled)
				return;

			ItemIcon.SetActive(false);
		}

		public virtual void ShowIcon()
		{
			if (!IsIconNeeded || IsIconEnabled)
				return;

			ItemIcon.SetActive(true);
		}
	}
}
