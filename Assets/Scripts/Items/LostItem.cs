using UnityEngine;

namespace Items
{
	public class LostItem : Item
	{
		[field: SerializeField] public Texture StikckerTexture { get; private set; }

		protected override void Start()
		{
			base.Start();
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

		protected override void OnDestroy()
		{
			base.OnDestroy();

			OnPickUpItem -= OnPlayerFindItem;
		}
	}
}