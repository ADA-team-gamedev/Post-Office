using Audio;
using UnityEngine;

namespace Level.Doors
{
	public enum GarageDoorPhase
	{
		None,
		Opening,
		Closing,
	}

	public class GarageDoor : MonoBehaviour
	{
		[Header("Values")]

		[SerializeField] private float _doorMaxHeight;

		[SerializeField][Range(0.5f, 5)] private float _doorRaisingSpeed = 1f;

		[Header("Objects")]

		[SerializeField] private Transform _doorModel;

		private float _defaultDoorYPosition;

		private GarageDoorPhase _garageDoorPhase = GarageDoorPhase.None;
		private bool _isOpenedAutomatically = false;

		private void Start()
		{
			_defaultDoorYPosition = _doorModel.position.y;
		}

		private void Update()
		{
			if (_garageDoorPhase == GarageDoorPhase.Opening)
				OpenDoorAutomatically();
			else if (_garageDoorPhase == GarageDoorPhase.Closing)
				CloseDoorAutomatically();
		}

		#region Door raising

		public void InteractRemotely()
		{
			if (_isOpenedAutomatically)
			{
				_isOpenedAutomatically = false;

				_garageDoorPhase = GarageDoorPhase.Closing;
			}
			else
			{
				_isOpenedAutomatically = true;

				_garageDoorPhase = GarageDoorPhase.Opening;
			}

			AudioManager.Instance.PlaySound("Garage Door Open", transform.position, spatialBlend: 0.8f);
		}

		private void OpenDoorAutomatically()
		{
			if (_garageDoorPhase != GarageDoorPhase.Opening)
			{
				return;
			}

			Vector3 raisedDoorPosiiton = new(_doorModel.position.x, _doorMaxHeight, _doorModel.position.z);

			if (_doorModel.position.y == raisedDoorPosiiton.y)
				_garageDoorPhase = GarageDoorPhase.None;

			_doorModel.position = Vector3.Lerp(_doorModel.position, raisedDoorPosiiton, Time.deltaTime * _doorRaisingSpeed);
		}

		private void CloseDoorAutomatically()
		{
			if (_garageDoorPhase != GarageDoorPhase.Closing)
				return;

			Vector3 defaultDoorPosition = new(_doorModel.position.x, _defaultDoorYPosition, _doorModel.position.z);

			if (_doorModel.position.y == defaultDoorPosition.y)
				_garageDoorPhase = GarageDoorPhase.None;

			_doorModel.position = Vector3.Lerp(_doorModel.position, defaultDoorPosition, Time.deltaTime * _doorRaisingSpeed);
		}

		#endregion

		private void OnValidate()
		{
			if (_doorMaxHeight < transform.position.y)
				_doorMaxHeight++;
		}

		private void OnDrawGizmosSelected()
		{
			//door possible max position
			Gizmos.color = Color.cyan;

			Gizmos.DrawWireCube(new(_doorModel.position.x, _doorMaxHeight, _doorModel.position.z), _doorModel.localScale);

			//door possible min position
			Gizmos.color = Color.green;

			Gizmos.DrawWireCube(new(_doorModel.position.x, _defaultDoorYPosition, _doorModel.position.z), _doorModel.localScale);
		}
	}
}
