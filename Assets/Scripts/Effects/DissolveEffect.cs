using UnityEngine;
using UnityEngine.UI;

namespace Effects
{
    public class DissolveEffect : MonoBehaviour
    {
		[SerializeField, Range(0, 1)] private float _dissolveEffectStrength = 0;

		[SerializeField] private Texture2D _dissolveTexture;

		private CanvasRenderer _canvasRenderer;
		private MaterialPropertyBlock _block;

		private const string DissolveEffectStrengthName = "_DissolveStrength";
		private const string DissolveEffectTexture = "_Texture2D";

		private Image _image;

		private void Awake()
		{
			_block = new();
			
			_canvasRenderer = GetComponent<CanvasRenderer>();

			_image = GetComponent<Image>();

			Material mat = Instantiate(_image.material);

			_image.material = mat;

			_image.material.SetTexture(DissolveEffectTexture, _dissolveTexture);

			_image.material.SetFloat(DissolveEffectStrengthName, 1);
		}

		private void OnValidate()
		{
			_block ??= new();

			_canvasRenderer ??= GetComponent<CanvasRenderer>();
		}

		public void ApplyDissolveEffectChanges(float dissolveStrength)
		{
			_image.material.SetFloat(DissolveEffectStrengthName, dissolveStrength);
		}
	}
}