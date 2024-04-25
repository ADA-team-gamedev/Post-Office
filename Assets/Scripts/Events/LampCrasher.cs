using Level.Lights.Lamp;
using UnityEngine;

namespace Events
{
    [RequireComponent(typeof(BoxCollider))]
    public class LampCrasher : MonoBehaviour
    {
		[SerializeField] private string _playerTag = "Player";

		[SerializeField] private BreakableLamp[] _breakableLamps;

		private bool _areLampsBroken = false;

		private void OnTriggerEnter(Collider other)
		{
			if (_areLampsBroken)
				return;

			if (other.CompareTag(_playerTag))
				BreakLamps();
		}

		private void BreakLamps()
		{
			if (_areLampsBroken)
				return;

			foreach (var lamp in _breakableLamps)
			{
				lamp.BreakLamp();
			}

			_areLampsBroken = true;
		}
	}
}