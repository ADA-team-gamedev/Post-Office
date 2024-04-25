using DataPersistance;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Menu
{
	public class OptionMenu : MonoBehaviour
	{
		[Header("Video")]

		[SerializeField] private TMP_Text _resolutionViewText;

		[SerializeField] private TMP_Text _screenModeText;

		[SerializeField] private TMP_Text _frameRateText;

		[SerializeField] private Toggle _vSyncToggle;

		#region Audio
		[Header("Audio")]

		[SerializeField] private AudioMixer _masterMixer;

		[SerializeField] private TMP_Text _masterMixerPercentText;
		[SerializeField] private TMP_Text _effectMixerPercentText;
		[SerializeField] private TMP_Text _musicMixerPercentText;
		[SerializeField] private TMP_Text _uiMixerPercentText;

		[SerializeField] private Slider _masterSlider;
		[SerializeField] private Slider _effectSlider;
		[SerializeField] private Slider _musicSlider;
		[SerializeField] private Slider _uiSlider;

		private const float _minLinearVolume = 0.0001f;

		#endregion

		#region Load System

		public OptionData _optionData = new();

		private IDataService _dataService = new JsonDataService();

		#endregion

		private void Start()
		{
			LoadSettings();
		}

		[ContextMenu(nameof(LoadSettings))]
		private void LoadSettings()
		{
			if (_dataService.TryLoadData(out OptionData optionData, JsonDataService.SettingDataPath, true))
				_optionData = optionData;
			
			ApplyChanges();
		}

		[ContextMenu(nameof(ResetSave))]
		private void ResetSave()
		{
			OptionData optionData = new();

			_dataService.SaveData(JsonDataService.SettingDataPath, optionData, true);
		}

		[ContextMenu(nameof(SaveSettings))]
		private void SaveSettings()
		{
			_dataService.SaveData(JsonDataService.SettingDataPath, _optionData, true);
		}

		[ContextMenu(nameof(ApplyChanges))]
		public void ApplyChanges()
		{
			Vector2Int resolution = _optionData.ScreenResolutions[_optionData.SelectedScreenResolutionIndex];

			Screen.SetResolution(resolution.x, resolution.y, _optionData.FullScreenMode);

			QualitySettings.vSyncCount = _optionData.VSyncCountEnable ? 1 : 0;

			Application.targetFrameRate = _optionData.FrameRates[_optionData.SelectedFrameRatesIndex];

			SaveSettings();
			
			UpdateTexts();
		}	

		public void UpdateTexts()
		{
			Vector2Int resolution = _optionData.ScreenResolutions[_optionData.SelectedScreenResolutionIndex];

			_resolutionViewText.text = $"{resolution.x}x{resolution.y}";

			_screenModeText.text = $"{_optionData.FullScreenMode}";

			_frameRateText.text = $"{_optionData.FrameRates[_optionData.SelectedFrameRatesIndex]}";

			_vSyncToggle.isOn = _optionData.VSyncCountEnable;

			_masterSlider.value = DecibelToLinear(_optionData.MasterVolume);
			_musicSlider.value = DecibelToLinear(_optionData.MusicVolume);
			_effectSlider.value = DecibelToLinear(_optionData.EffectVolume);
			_uiSlider.value = DecibelToLinear(_optionData.UIVolume);
		}

		#region Video Buttons

		public void ResolutionButton(int index)
		{
			index = Mathf.Clamp(index, -1, 1);

			_optionData.SelectedScreenResolutionIndex += index;
		}

		public void FrameRateButton(int index)
		{
			index = Mathf.Clamp(index, -1, 1);

			_optionData.SelectedFrameRatesIndex += index;
		}

		public void FullScreenModeButton(int index)
		{
			index = Mathf.Clamp(index, -1, 1);

			_optionData.FullScreenMode += index;
		}

		public void ChangeVSyncState()
		{
			_optionData.VSyncCountEnable = _vSyncToggle.isOn;
		}

		#endregion

		#region Audio

		public void SetMasterVolume()
		{
			float level = Mathf.Log10(_masterSlider.value) * 20f;

			_masterMixerPercentText.text = $"{Mathf.RoundToInt(DecibelToLinear(level) * 100)}";
			_masterMixer.SetFloat("MasterVolume", level);

			_optionData.MasterVolume = level;
		}

		public void SetMusicVolume()
		{
			float level = Mathf.Log10(_musicSlider.value) * 20f;

			_musicMixerPercentText.text = $"{Mathf.RoundToInt(DecibelToLinear(level) * 100)}";

			_masterMixer.SetFloat("MusicVolume", level);

			_optionData.MusicVolume = level;
		}

		public void SetEffectVolume()
		{
			float level = Mathf.Log10(_effectSlider.value) * 20f;

			_effectMixerPercentText.text = $"{Mathf.RoundToInt(DecibelToLinear(level) * 100)}";

			_masterMixer.SetFloat("EffectVolume", level);

			_optionData.EffectVolume = level;
		}

		public void SetUIVolume()
		{
			float level = Mathf.Log10(_uiSlider.value) * 20f;

			_uiMixerPercentText.text = $"{Mathf.RoundToInt(DecibelToLinear(level) * 100)}";

			_masterMixer.SetFloat("UIVolume", level);

			_optionData.UIVolume = level;
		}

		private float LinearToDecibel(float linear)
		{
			if (linear > 0)
				return Mathf.Log10(linear) * 20f;
			else
				return _minLinearVolume;
		}

		private float DecibelToLinear(float decibel)
		{
			return Mathf.Pow(10f, decibel / 20f);
		}	

		#endregion
	}
}
