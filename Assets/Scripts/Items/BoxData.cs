using UnityEngine;

public enum BoxContentType
{
	Empty,
	Glass,
	Metal,
	Toy,
	Book,
	Electronic,
}

public enum BoxTemperatureType
{
	Fiery,
	Hot,
	Normal,
	Cold,
	Frosty,
}

[CreateAssetMenu]
public class BoxData : ScriptableObject
{
	[field: SerializeField] public BoxContentType ContentType { get; private set; } = BoxContentType.Empty;

	[field: SerializeField] public BoxTemperatureType BoxTemperatureType { get; private set; } = BoxTemperatureType.Normal;
}
