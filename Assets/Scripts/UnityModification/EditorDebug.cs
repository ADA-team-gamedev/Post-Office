using UnityEngine;

namespace UnityModification
{
	public class EditorDebug
	{
		#region Logs

		public static void Log(object message)
		{
#if UNITY_EDITOR
			Debug.Log(message);
#endif
		}

		public static void Log(object message, Object context)
		{
#if UNITY_EDITOR
			Debug.Log(message, context);
#endif
		}

		public static void LogWarning(object message)
		{
#if UNITY_EDITOR
			Debug.LogWarning(message);
#endif
		}

		public static void LogWarning(object message, Object context)
		{
#if UNITY_EDITOR
			Debug.LogWarning(message, context);
#endif
		}

		public static void LogError(object message)
		{
#if UNITY_EDITOR
			Debug.LogError(message);
#endif
		}

		public static void LogError(object message, Object context)
		{
#if UNITY_EDITOR
			Debug.LogError(message, context);
#endif
		}

		#endregion

		#region Drawers

		public static void DrawRay(Vector3 start, Vector3 direction)
		{
#if UNITY_EDITOR
			Debug.DrawRay(start, direction);
#endif
		}

		public static void DrawRay(Vector3 start, Vector3 direction, Color color)
		{
#if UNITY_EDITOR
			Debug.DrawRay(start, direction, color);
#endif
		}

		public static void DrawRay(Vector3 start, Vector3 direction, Color color, float duration)
		{
#if UNITY_EDITOR
			Debug.DrawRay(start, direction, color, duration);
#endif
		}

		public static void DrawRay(Vector3 start, Vector3 direction, Color color, float duration, bool depthTest)
		{
#if UNITY_EDITOR
			Debug.DrawRay(start, direction, color, duration, depthTest);
#endif
		}

		public static void DrawLine(Vector3 start, Vector3 end)
		{
#if UNITY_EDITOR
			Debug.DrawLine(start, end);
#endif
		}

		public static void DrawLine(Vector3 start, Vector3 end, Color color)
		{
#if UNITY_EDITOR
			Debug.DrawLine(start, end, color);
#endif
		}

		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
		{
#if UNITY_EDITOR
			Debug.DrawLine(start, end, color, duration);
#endif
		}

		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
		{
#if UNITY_EDITOR
			Debug.DrawLine(start, end, color, duration, depthTest);
#endif
		}

		#endregion
	}
}