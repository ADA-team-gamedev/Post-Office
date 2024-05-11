using System;

namespace UnityEngine.Modification
{
	public abstract class DestructiveBehaviour<T> : MonoBehaviour where T : DestructiveBehaviour<T>
	{
		public event Action<T> OnObjectDestroyed;

		protected virtual void OnDestroy()
		{
			OnObjectDestroyed?.Invoke((T)this);
		}
	}
}