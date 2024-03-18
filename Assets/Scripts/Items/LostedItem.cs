using UnityEngine;
using TaskSystem.NoteBook;

namespace Items
{
	public class LostedItem : Item
	{
		[SerializeField] private GameObject _lostedItemPhoto;

		[SerializeField] private SerializedTime _lostedItemAddTime;

		[SerializeField] private TimeClock _timeClock;

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

			_lostedItemPhoto.gameObject.SetActive(false);

			_timeClock.IncreaseTimeLimit(new(_lostedItemAddTime.Hours, _lostedItemAddTime.Minutes, _lostedItemAddTime.Seconds));
		}
	}
}