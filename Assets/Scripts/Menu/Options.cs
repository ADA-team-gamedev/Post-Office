using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
	[SerializeField] private Button _applyChangesButton;

	[Header("Resolution")]

	[SerializeField] private List<Vector2Int> _screenResolutions = new()
	{
		new(1920, 1080),
		new(1024, 768),
		new(1152, 864),
		new(1280, 720),
		new(1280, 768),
		new(1280, 800),
		new(1280, 960),
		new(1280, 1024),
		new(1360, 768),
		new(1366, 768),
		new(1440, 900),
		new(1440, 1080),
		new(1600, 900),
		new(1600, 1024),
		new(1680, 1050),
	};

	[SerializeField] private TMP_Text _resolutionViewText;

	[Header("Screen Mode")]
	[SerializeField] private TMP_Text _screenModeText;
	private FullScreenMode _screenMode;

	private int _selectedResolutionIndex = 0;

	[Header("Frame Rate")]
	[SerializeField] private TMP_Text _frameRateText;

	[SerializeField] private List<int> _frameRates = new()
	{
		0,
		15,
		30,
		45,
		60,
		75,
		90,
		105,
		120,
	};

	private int _selectedFrameRateIndex = 0;

	[SerializeField] private Toggle _vSync;

	private void Start()
	{
		LoadSettings();

		if (QualitySettings.vSyncCount == 0)
		{
			_vSync.isOn = false;
		}
		else
			_vSync.isOn = true;
	}

	public void SaveChanges()
    {
		Vector2Int resolution = _screenResolutions[_selectedResolutionIndex];
		
		Screen.SetResolution(resolution.x, resolution.y, _screenMode);

		QualitySettings.vSyncCount = _vSync.isOn ? 1 : 0;
    }

	private void LoadSettings()
	{

	}

	#region Resolution

	public void RightResolutionChangeButton()
    {
		_selectedResolutionIndex++;

		if (_selectedResolutionIndex > _screenResolutions.Count - 1)
			_selectedResolutionIndex = 0;

		Vector2Int resolution = _screenResolutions[_selectedResolutionIndex];

		UpdateOptionText(_resolutionViewText, $"{resolution.x}x{resolution.y}");
	}

	public void LeftResolutionChangeButton()
	{
        _selectedResolutionIndex--;

        if (_selectedResolutionIndex < 0)
            _selectedResolutionIndex = _screenResolutions.Count - 1;

		Vector2Int resolution = _screenResolutions[_selectedResolutionIndex];

		UpdateOptionText(_resolutionViewText, $"{resolution.x}x{resolution.y}");
	}

	#endregion

	#region ScreenMode

	public void RightScreenModeButton()
	{
		_screenMode--;

		var fullScreenMode = Enum.GetValues(typeof(FullScreenMode));

		FullScreenMode maxEnumValue = (FullScreenMode)fullScreenMode.GetValue(fullScreenMode.Length - 1);
		FullScreenMode minEnumValue = (FullScreenMode)fullScreenMode.GetValue(0);

		if (_screenMode == FullScreenMode.MaximizedWindow)
			_screenMode--;
		
		if (_screenMode < minEnumValue)
			_screenMode = maxEnumValue;

		UpdateOptionText(_screenModeText, $"{_screenMode}");
	}

	public void LeftScreenModeButton()
	{
		_screenMode++;

		if (_screenMode == FullScreenMode.MaximizedWindow)
			_screenMode++;

		var fullScreenMode = Enum.GetValues(typeof(FullScreenMode));

		FullScreenMode maxEnumValue = (FullScreenMode)fullScreenMode.GetValue(fullScreenMode.Length - 1);
		FullScreenMode minEnumValue = (FullScreenMode)fullScreenMode.GetValue(0);

		if (_screenMode > maxEnumValue)
			_screenMode = minEnumValue;

		UpdateOptionText(_screenModeText, $"{_screenMode}");
	}

	#endregion

	#region FPS

	public void RightFrameRateChangeButton()
	{
		_selectedFrameRateIndex++;

		if (_selectedFrameRateIndex > _frameRates.Count - 1)
			_selectedFrameRateIndex = 0;

		UpdateOptionText(_frameRateText, $"{_frameRates[_selectedFrameRateIndex]}");
	}

	public void LeftFrameRateChangeButton()
	{
		_selectedFrameRateIndex--;

		if (_selectedFrameRateIndex < 0)
			_selectedFrameRateIndex = _frameRates.Count - 1;

		UpdateOptionText(_frameRateText, $"{_frameRates[_selectedFrameRateIndex]}");
	}

	#endregion

	public void InvertVSyncMode()
	{
		_vSync.isOn = !_vSync.isOn;
	}

	private void UpdateOptionText(TMP_Text textObject, string text)
	{
		textObject.text = text;
	}
}
