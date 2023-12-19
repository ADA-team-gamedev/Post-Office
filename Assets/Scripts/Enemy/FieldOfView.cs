using UnityEngine;

public class FieldOfView : MonoBehaviour
{
	[Header("Layer")]
	[SerializeField] private LayerMask _playerLayer;

	[SerializeField] private LayerMask _obstacleLayer;

	[Header("Values")]
	public float ViewedRadius = 6f;

	public float InstantDetectionRadius = 2f;

	[Range(0, 360)] 
	public float ViewedAngle = 90;

	public Transform TargetTransform { get; private set; }

	public bool CanSeePLayer { get; private set; } = false;
	public bool InstantDetectTarget { get; private set; } = false;

	public void VisionCheck()
	{
		Collider[] rangeChecks = Physics.OverlapSphere(transform.position, ViewedRadius, _playerLayer);

		if (rangeChecks.Length != 0)
		{
			Transform target = rangeChecks[0].transform;

			TargetTransform = target;

			Vector3 directionToTarget = (target.position - transform.position).normalized;

			float distanceToTarget = Vector3.Distance(transform.position, target.position);

			if (distanceToTarget <= InstantDetectionRadius) //instant radius checker
			{
				if (!Physics.Raycast(transform.position, directionToTarget, _obstacleLayer))
					InstantDetectTarget = true;
				else
					InstantDetectTarget = false;
			}

			if (Vector3.Angle(transform.forward, directionToTarget) < ViewedAngle / 2) //FOV checker
			{
				if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleLayer))
					CanSeePLayer = true;
				else
					CanSeePLayer = false;
			}
			else
				CanSeePLayer = false;
		}
		else if (CanSeePLayer)
			CanSeePLayer = false;
		else if (InstantDetectTarget)
			InstantDetectTarget = false;
	}

	private void OnValidate()
	{
		if (InstantDetectionRadius > ViewedRadius)
			ViewedRadius = InstantDetectionRadius + 1;
	}
}
