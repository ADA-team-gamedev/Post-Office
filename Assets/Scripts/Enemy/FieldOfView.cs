using UnityEngine;

namespace Enemy
{
	public class FieldOfView : MonoBehaviour
	{
		[Header("Layer")]
		[SerializeField] private LayerMask _targetLayer;

		[SerializeField] private LayerMask _obstacleLayer;

		[Header("Values")]
		public float ViewedRadius = 6f;

		public float InstantDetectionRadius = 2f;

		[Range(0, 360)]
		public float ViewedAngle = 90;

		public Transform Target { get; private set; }

		public bool SeesInFOV { get; private set; } = false;
		public bool InstantDetectTarget { get; private set; } = false;
		public bool CanSeeTarget => SeesInFOV || InstantDetectTarget;

		public void VisionCheck()
		{
			Collider[] rangeChecks = Physics.OverlapSphere(transform.position, ViewedRadius, _targetLayer);

			if (rangeChecks.Length != 0)
			{
				Transform target = rangeChecks[0].transform;

				Target = target;

				Vector3 directionToTarget = (target.position - transform.position).normalized;

				float distanceToTarget = Vector3.Distance(transform.position, target.position);

				bool isHitObstacle = Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleLayer);

				if (distanceToTarget < InstantDetectionRadius && !isHitObstacle) //instant radius checker
					InstantDetectTarget = true;
				else
					InstantDetectTarget = false;

				if (Vector3.Angle(transform.forward, directionToTarget) < ViewedAngle / 2) //FOV checker
				{
					if (!isHitObstacle)
						SeesInFOV = true;
					else
						SeesInFOV = false;
				}
				else
				{
					SeesInFOV = false;
				}
			}
			else
			{
				SeesInFOV = false;

				InstantDetectTarget = false;
			}
		}

		private void OnValidate()
		{
			if (InstantDetectionRadius > ViewedRadius)
				ViewedRadius = InstantDetectionRadius + 1;

			if (InstantDetectionRadius < 0)
				InstantDetectionRadius = 0;

			if (ViewedRadius < 0)
				ViewedRadius = 0;
		}
	}
}
