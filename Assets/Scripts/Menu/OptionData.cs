using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
	public class OptionData
	{
		[field: SerializeField]
		public List<Vector2Int> ScreenResolutions { get; private set; } = new()
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
			private set
			{
				if (value < 0 || value >= ScreenResolutions.Count)
					return;

				_selectedScreenResolution = value;
			}
		}

		private int _selectedScreenResolution = 0;

		public FullScreenMode FullScreenMode { get; set; } = FullScreenMode.ExclusiveFullScreen;

		[field: SerializeField]
		public List<int> FrameRates { get; private set; } = new()
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
			private set
			{
				if (value < 0 || value >= FrameRates.Count)
					return;

				_selectedFrameRates = value;
			}
		}

		private int _selectedFrameRates = 0;

		public bool VSyncCountEnable { get; set; } = false;

		public float ScreenBrightness { get; set; } = 1; 
	}
}
