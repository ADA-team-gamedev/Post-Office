using UnityEngine;
using UnityEngine.UI;

namespace Effects
{
    public class DissolveEffect : MonoBehaviour
    {
		[SerializeField] private Texture2D _dissolveTexture;

		private CanvasRenderer _canvasRenderer;
		private MaterialPropertyBlock _block;

		private const string DissolveEffectStrengthName = "_DissolveStrength";
		private const string DissolveEffectTexture = "_Texture2D";

		private Image _image;

		public const float MaxDissolveEffectStrength = 1;
		public const float MinDissolveEffectStrength = 0;

		[Range(0f, 1f)] public float some = 0;

		private void Awake()
		{
			_block = new();
			
			_canvasRenderer = GetComponent<CanvasRenderer>();

			_image = GetComponent<Image>();

			Material mat = Instantiate(_image.material);

			_image.material = mat;

			_image.material.SetTexture(DissolveEffectTexture, _dissolveTexture);
			
			_image.material.SetFloat(DissolveEffectStrengthName, MinDissolveEffectStrength);
		}

		private void Update()
		{
			_image.material.SetFloat(DissolveEffectStrengthName, some);
		}

		private void OnValidate()
		{
			_block ??= new();

			_canvasRenderer ??= GetComponent<CanvasRenderer>();
		}

		public void ChangeEffectStrength(float dissolveStrength)
		{
			dissolveStrength = Mathf.Clamp(dissolveStrength, MinDissolveEffectStrength, MaxDissolveEffectStrength);
			
			_image.material.SetFloat(DissolveEffectStrengthName, dissolveStrength);
		}
	}
}