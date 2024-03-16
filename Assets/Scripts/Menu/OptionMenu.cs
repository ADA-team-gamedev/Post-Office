using DataPersistance;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
	public class OptionMenu : MonoBehaviour
	{
		[SerializeField] private TMP_Text _resolutionViewText;

		[SerializeField] private TMP_Text _screenModeText;

		[SerializeField] private TMP_Text _frameRateText;

		[SerializeField] private Toggle _vSyncToggle;

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

		#region Buttons

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
	}
}
