using Items;
using System;

namespace Player.Inventory
{
    public interface IReadOnlyInventory
    {
		public event Action<Item> OnItemPicked;

		public event Action<Item> OnItemDroped;

		public event Action OnItemChanged;

		public bool IsContainsItem<T>(T item) where T : Item;

		public bool TryGetItem<T>(out T item) where T : Item;
			
		public bool TryGetCurrentItem<T>(out T item) where T : Item;

		public bool TryRemoveItem<T>(T item) where T : Item;
	}
}