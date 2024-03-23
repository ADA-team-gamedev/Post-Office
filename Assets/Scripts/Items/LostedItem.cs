using UnityEngine;

namespace Items
{
	public class LostedItem : Item
	{
		[SerializeField] private GameObject _lostedItemPhoto;

		private void Start()
		{
			InitializeItem();
		}

		protected override void InitializeItem()
		{
			base.InitializeItem();

			OnPickUpItem += OnPlayerFindItem;

			_lostedItemPhoto.SetActive(true);
		}

		private void OnPlayerFindItem(Item item)
		{
			OnPickUpItem -= OnPlayerFindItem;

			_lostedItemPhoto.SetActive(false);
		}
	}
}