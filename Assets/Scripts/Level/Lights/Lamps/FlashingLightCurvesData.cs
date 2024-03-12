using System.Collections.Generic;
using UnityEngine;

namespace Level.Lights.Lamp
{
	[CreateAssetMenu]
	public class FlashingLightCurvesData : ScriptableObject
	{
		[field: SerializeField] public List<AnimationCurve> Curves { get; private set; }
	}
}
