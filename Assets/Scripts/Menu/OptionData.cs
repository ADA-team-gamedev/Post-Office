using System;
using UnityEngine;

namespace Menu
{
	public class OptionData
	{
		#region Video

		[field: SerializeField]
		public Vector2Int[] ScreenResolutions { get; private set; } = 
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

		public int SelectedScreenResolutionIndex
		{
			get
			{
				return _selectedScreenResolution;
			}
			set
			{
				if (value < 0)
					_selectedScreenResolution = ScreenResolutions.Length - 1;
				else if (value > ScreenResolutions.Length - 1)
					_selectedScreenResolution = 0;
				else
					_selectedScreenResolution = value;
			}
		}

		private int _selectedScreenResolution = 0;

		public FullScreenMode FullScreenMode 
		{
			get
			{
				return _selectedScreenMode;
			}
			set
			{
				int enumLength = Enum.GetValues(typeof(FullScreenMode)).Length;

				FullScreenMode lastElement = (FullScreenMode)(enumLength - 1);

				if (value < 0)
					_selectedScreenMode = lastElement;
				else if (value > lastElement)
					_selectedScreenMode = 0;
				else
					_selectedScreenMode = value;
			}
		} 
		private FullScreenMode _selectedScreenMode = FullScreenMode.FullScreenWindow;

		[field: SerializeField]
		public int[] FrameRates { get; private set; } = 
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

		public int SelectedFrameRatesIndex
		{
			get
			{
				return _selectedFrameRates;
			}
			set
			{
				if (value < 0)
					_selectedFrameRates = FrameRates.Length - 1;
				else if (value > FrameRates.Length - 1)
					_selectedFrameRates = 0;
				else
					_selectedFrameRates = value;
			}
		}

		private int _selectedFrameRates = 0;

		public bool VSyncCountEnable { get; set; } = false;

		public float ScreenBrightness { get; set; } = 1;

		#endregion

		#region Audio

		public float MasterVolume { get; set; } = 0;

		public float EffectVolume { get; set; } = 0;

		public float MusicVolume { get; set; } = 0;

		public float UIVolume { get; set; } = 0;

		#endregion
	}
}
