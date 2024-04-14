using UnityEngine;

namespace Items
{
	public class LostItem : Item
	{
		[field: SerializeField] public Texture StikckerTexture { get; private set; }

		private void Start()
		{
			InitializeItem();
		}

		protected override void InitializeItem()
		{
			base.InitializeItem();

			OnPickUpItem += OnPlayerFindItem;
		}

		private void OnPlayerFindItem(Item item)
		{
			OnPickUpItem -= OnPlayerFindItem;
		}
	}
}