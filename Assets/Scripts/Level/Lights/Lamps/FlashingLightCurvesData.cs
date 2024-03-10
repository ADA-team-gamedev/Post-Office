using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FlashingLightCurvesData : ScriptableObject
{
	[field: SerializeField] public List<AnimationCurve> Curves { get; private set; }
}
