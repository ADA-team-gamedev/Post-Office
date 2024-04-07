using UnityEngine;

namespace Items.Icon
{
	[System.Serializable]
	public class ItemIcon : Icon
	{
		[field: SerializeField] public bool EnableOnStart { get; private set; } = false;
		[field: SerializeField] public bool ChangeIconStateAutomatically { get; private set; } = false;

		public void ShowIcon(Item item)
		{
			if (!IsIconNeeded)
				return;

			ShowIcon();

			ItemIcon.transform.position = new(item.transform.position.x, ItemIcon.transform.position.y, item.transform.position.z);
		}

		public void HideIcon(Item item)
		{
			HideIcon();
		}
	}
}
