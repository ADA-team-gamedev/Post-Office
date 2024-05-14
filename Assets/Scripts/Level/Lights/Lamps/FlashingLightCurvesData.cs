using System.Collections.Generic;
using UnityEngine;

namespace Level.Lights.Lamps
{
	[CreateAssetMenu]
	public class FlashingLightCurvesData : ScriptableObject
	{
		[field: SerializeField] public List<AnimationCurve> Curves { get; private set; }
	}
}
