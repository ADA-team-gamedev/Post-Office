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

		private OptionData _optionData = new();

		private IDataService _dataService = new JsonDataService();
		private bool _isEncrypted = false;
		private string _path = "/Settings";

		#endregion

		private void Start()
		{
			LoadSettings();
		}

		private void LoadSettings()
		{
			if (_dataService.LoadData(out OptionData optionData, _path, _isEncrypted))
				_optionData = optionData;		

			ApplyChanges();
		}

		private void ApplyChanges()
		{
			Vector2Int resolution = _optionData.ScreenResolutions[_optionData.SelectedScreenResolutionIndex];

			Screen.SetResolution(resolution.x, resolution.y, _optionData.FullScreenMode);

			QualitySettings.vSyncCount = _optionData.VSyncCountEnable ? 1 : 0;

			Application.targetFrameRate = _optionData.FrameRates[_optionData.SelectedFrameRatesIndex];

			SaveSettings();
			
			UpdateTexts();
		}

		private void UpdateTexts()
		{
			Vector2Int resolution = _optionData.ScreenResolutions[_optionData.SelectedScreenResolutionIndex];

			_resolutionViewText.text = $"{resolution.x}x{resolution.y}";

			_vSyncToggle.isOn = _optionData.VSyncCountEnable;

			_screenModeText.text = $"{_optionData.FullScreenMode}";

			_frameRateText.text = $"{_optionData.FrameRates[_optionData.SelectedFrameRatesIndex]}";
		}

		private void SaveSettings()
		{
			_dataService.SaveData(_path, _optionData, _isEncrypted);
		}

		[ContextMenu("Change")]
		private void ChangeSetting()
		{

		}

		//#region Resolution

		//public void RightResolutionChangeButton()
		//{
		//	_selectedResolutionIndex++;

		//	if (_selectedResolutionIndex > _screenResolutions.Count - 1)
		//		_selectedResolutionIndex = 0;

		//	Vector2Int resolution = _screenResolutions[_selectedResolutionIndex];

		//	UpdateOptionText(_resolutionViewText, $"{resolution.x}x{resolution.y}");
		//}

		//public void LeftResolutionChangeButton()
		//{
		//	_selectedResolutionIndex--;

		//	if (_selectedResolutionIndex < 0)
		//		_selectedResolutionIndex = _screenResolutions.Count - 1;

		//	Vector2Int resolution = _screenResolutions[_selectedResolutionIndex];

		//	UpdateOptionText(_resolutionViewText, $"{resolution.x}x{resolution.y}");
		//}

		//#endregion

		//#region ScreenMode

		//public void RightScreenModeButton()
		//{
		//	_screenMode--;

		//	var fullScreenMode = Enum.GetValues(typeof(FullScreenMode));

		//	FullScreenMode maxEnumValue = (FullScreenMode)fullScreenMode.GetValue(fullScreenMode.Length - 1);
		//	FullScreenMode minEnumValue = (FullScreenMode)fullScreenMode.GetValue(0);

		//	if (_screenMode == FullScreenMode.MaximizedWindow)
		//		_screenMode--;

		//	if (_screenMode < minEnumValue)
		//		_screenMode = maxEnumValue;

		//	UpdateOptionText(_screenModeText, $"{_screenMode}");
		//}

		//public void LeftScreenModeButton()
		//{
		//	_screenMode++;

		//	if (_screenMode == FullScreenMode.MaximizedWindow)
		//		_screenMode++;

		//	var fullScreenMode = Enum.GetValues(typeof(FullScreenMode));

		//	FullScreenMode maxEnumValue = (FullScreenMode)fullScreenMode.GetValue(fullScreenMode.Length - 1);
		//	FullScreenMode minEnumValue = (FullScreenMode)fullScreenMode.GetValue(0);

		//	if (_screenMode > maxEnumValue)
		//		_screenMode = minEnumValue;

		//	UpdateOptionText(_screenModeText, $"{_screenMode}");
		//}

		//#endregion

		//#region FPS

		//public void RightFrameRateChangeButton()
		//{
		//	_selectedFrameRateIndex++;

		//	if (_selectedFrameRateIndex > _frameRates.Count - 1)
		//		_selectedFrameRateIndex = 0;

		//	UpdateOptionText(_frameRateText, $"{_frameRates[_selectedFrameRateIndex]}");
		//}

		//public void LeftFrameRateChangeButton()
		//{
		//	_selectedFrameRateIndex--;

		//	if (_selectedFrameRateIndex < 0)
		//		_selectedFrameRateIndex = _frameRates.Count - 1;

		//	UpdateOptionText(_frameRateText, $"{_frameRates[_selectedFrameRateIndex]}");
		//}

		//#endregion
	}
}
