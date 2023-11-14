using UnityEngine;

public class PlayerSanity : MonoBehaviour
{
	[SerializeField][Range(1, 100f)] private float _maxSanityValue = 100f;

	public float Sanity
	{
		get
		{
			return _sanity;
		}
		set
		{
			if (value >= 0 && value <= _maxSanityValue)
				_sanity = value;
		}
	}

	private float _sanity;
}
