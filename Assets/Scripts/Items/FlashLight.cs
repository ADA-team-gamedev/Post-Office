using Audio;
using Player;
using UnityEngine;

namespace Items
{
    public class FlashLight : Item, IUsable
    {
        [Header("Objects")]
        [SerializeField] private Light _flashlight;

        [SerializeField] private bool _disableOnStart = true;

        public bool IsWorking { get; private set; } = false;

		protected override void Start()
        {
			base.Start();
		}

		protected override void InitializeItem()
		{
			base.InitializeItem();

			IsWorking = !_disableOnStart;

			_flashlight.enabled = IsWorking;

            OnDropItem += EnableOnDrop;
		}

		public void Use(Interactor interactor)
        {
            if (!IsWorking)
                TurnOn();
            else
                TurnOff();

			AudioManager.Instance.PlaySound("Flashlight On", transform.position);
		}

        private void TurnOff()
        {
            _flashlight.enabled = false;

            IsWorking = false;
        }

        private void TurnOn()
        {
            _flashlight.enabled = true;

            IsWorking = true;
        }

        private void EnableOnDrop(Item item)
        {
            TurnOn();
		}

		protected override void OnDestroy()
		{
            base.OnDestroy();

			OnDropItem -= EnableOnDrop;
		}
	}
}
