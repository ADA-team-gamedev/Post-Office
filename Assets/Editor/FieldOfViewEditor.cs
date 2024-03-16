using Enemy;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
	private void OnSceneGUI()
	{
		FieldOfView fieldOfView = (FieldOfView)target;

		//instant detection area
		Handles.color = Color.red;

		Handles.DrawWireArc(fieldOfView.transform.position, Vector3.up, Vector3.forward, 360, fieldOfView.InstantDetectionRadius);

		//viewed area

		Handles.color = Color.yellow;

		Handles.DrawWireArc(fieldOfView.transform.position, Vector3.up, Vector3.forward, 360, fieldOfView.ViewedRadius);

		Vector3 viewLeftAngleSide = DirectionFromAngle(fieldOfView.transform.eulerAngles.y, -fieldOfView.ViewedAngle / 2);

		Vector3 viewRightAngleSide = DirectionFromAngle(fieldOfView.transform.eulerAngles.y, fieldOfView.ViewedAngle / 2);

		Color color = Color.grey;

		Handles.color = new Color(color.r, color.g, color.b, 0.4f);

		Handles.DrawSolidArc(fieldOfView.transform.position, Vector3.up, viewLeftAngleSide, fieldOfView.ViewedAngle, fieldOfView.ViewedRadius);

		Handles.color = Color.black;

		Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + viewLeftAngleSide * fieldOfView.ViewedRadius);

		Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + viewRightAngleSide * fieldOfView.ViewedRadius);

		if (fieldOfView.SeesInFOV)
		{
			Handles.color = Color.green;
			
			Handles.DrawLine(fieldOfView.transform.position, fieldOfView.Target.position);
		}
	}

	private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
	{
		angleInDegrees += eulerY;

		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}
}
