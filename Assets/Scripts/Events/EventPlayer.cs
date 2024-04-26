using UnityEngine;
using AYellowpaper;

namespace Events
{
    [RequireComponent(typeof(BoxCollider))]
    public class EventPlayer : MonoBehaviour
    {
		[SerializeField] private string _playerTag = "Player";

		[Header("Event Settings")]
		[SerializeField] private bool _isRandomEventPlayed = false;
		[SerializeField] private bool _isEventMustPlayedOnce = true;

		[SerializeField] private InterfaceReference<IEvent, MonoBehaviour>[] _eventObjects;

		[SerializeReference] private IEvent _event;

		private bool _isEventPlayed = false;

		private void OnTriggerEnter(Collider other)
		{
			if (_isEventPlayed)
				return;

			if (other.CompareTag(_playerTag))
				PlayEvents();
		}

		private void PlayEvents()
		{
			if (_isEventPlayed)
				return;

			if (_isRandomEventPlayed)
			{
				InterfaceReference<IEvent, MonoBehaviour> eventItem = _eventObjects[Random.Range(0, _eventObjects.Length)];
				
				eventItem.Value.PlayEvent();
			}
			else
			{
				foreach (var eventItem in _eventObjects)
				{
					eventItem.Value.PlayEvent();
				}
			}

			if (_isEventMustPlayedOnce)
				_isEventPlayed = true;
		}
	}
}