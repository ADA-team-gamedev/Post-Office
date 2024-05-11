using UnityEngine;

namespace Items.Keys
{
	public enum DoorKeyType
	{
		Hall,
		Storage,
		Toilet,
		Janitor,
		Entrance,
		Exit,
		Office,
		Boss,
		Workshop,
		Fuse,
		Kitchen,
	}

	[SelectionBase]
	[RequireComponent(typeof(BoxCollider))]
	public class Key : Item
	{
		[field: Header("Key")]
		[field: SerializeField] public DoorKeyType KeyType { get; private set; }

		[field: Header("Key Label")]

		public Color LabelColor { get; private set; }

		[SerializeField] private KeyLabelData _keyLabelData;

		[SerializeField] private Renderer _labelRenderer;

		public const string LabelBaseColorName = "_BaseColor";

		private void Awake()
		{
			PaintLabelToRightColor();
		}

		protected override void Start()
		{
			base.Start();	
		}

		private void PaintLabelToRightColor()
		{
			Color color = _keyLabelData.GetlColor(KeyType);

			MaterialPropertyBlock block = new();

			block.SetColor(LabelBaseColorName, color);
			
			_labelRenderer.SetPropertyBlock(block);

			LabelColor = color;
		}
	}
}
