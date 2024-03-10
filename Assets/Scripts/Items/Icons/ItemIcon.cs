using UnityEngine;

[System.Serializable]
public class ItemIcon : Icon
{
	[field: SerializeField] public bool ChangeIconStateAutomatically { get; private set; } = false;

	public void ShowIcon(Item item)
	{
		ShowIcon();

		ItemIcon.transform.position = new(item.transform.position.x, ItemIcon.transform.position.y, item.transform.position.z);
	}

	public void HideIcon(Item item)
	{
		HideIcon();
	}
}
