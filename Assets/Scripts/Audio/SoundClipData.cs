using UnityEngine;

namespace Audio
{
	[CreateAssetMenu]
	public class SoundClipData : ScriptableObject
	{
		[field: SerializeField] public SoundClip SoundClip { get; private set; }
	}
}