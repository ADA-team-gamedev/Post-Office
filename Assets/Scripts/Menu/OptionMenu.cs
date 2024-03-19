using DataPersistance;
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

		#region Load System

		public OptionData _optionData = new();

		private IDataService _dataService = new JsonDataService();
		public const string SettingDataPath = "/Settings";

		#endregion

		private void Start()
		{
			LoadSettings();
		}

		[ContextMenu(nameof(LoadSettings))]
		private void LoadSettings()
		{
			if (_dataService.LoadData(out OptionData optionData, SettingDataPath, true))
				_optionData = optionData;
			
			ApplyChanges();
		}

		[ContextMenu(nameof(ResetSave))]
		private void ResetSave()
		{
			OptionData optionData = new();

			_dataService.SaveData(SettingDataPath, optionData, true);
		}

		[ContextMenu(nameof(SaveSettings))]
		private void SaveSettings()
		{
			_dataService.SaveData(SettingDataPath, _optionData, true);
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
		}

		#region Video Buttons

		public void RightResolutionButton()
		{
			_optionData.SelectedScreenResolutionIndex++;
		}

		public void LeftResolutionButton()
		{
			_optionData.SelectedScreenResolutionIndex--;
		}

		public void RightFrameRateButton()
		{
			_optionData.SelectedFrameRatesIndex++;
		}

		public void LeftFrameRateButton()
		{
			_optionData.SelectedFrameRatesIndex--;
		}

		public void RightFullScreenModeButton()
		{
			_optionData.FullScreenMode++;
		}

		public void LeftFullScreenModeButton()
		{
			_optionData.FullScreenMode--;
		}

		public void ChangeVSyncState()
		{
			_optionData.VSyncCountEnable = !_optionData.VSyncCountEnable;
		}

		#endregion

		#region Audio

		public void SetMasterVolume()
		{
			_masterMixerPercentText.text = $"{_masterSlider.value + 80}";

			_masterMixer.SetFloat("MasterVolume", _masterSlider.value);

			_optionData.MasterVolume = _masterSlider.value;
		}

		public void SetMusicVolume()
		{
			_musicMixerPercentText.text = $"{_musicSlider.value + 80}";

			_masterMixer.SetFloat("MusicVolume", _musicSlider.value);

			_optionData.MusicVolume = _musicSlider.value;
		}

		public void SetEffectVolume()
		{
			_effectMixerPercentText.text = $"{_effectSlider.value + 80}";

			_masterMixer.SetFloat("EffectVolume", _effectSlider.value);

			_optionData.EffectVolume = _effectSlider.value;
		}

		public void SetUIVolume()
		{
			_uiMixerPercentText.text = $"{_uiSlider.value + 80}";

			_masterMixer.SetFloat("UIVolume", _uiSlider.value);

			_optionData.UIVolume = _uiSlider.value;
		}

		#endregion
	}
}
