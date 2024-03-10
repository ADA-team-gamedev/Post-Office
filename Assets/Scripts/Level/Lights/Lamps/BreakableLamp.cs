using UnityEngine;

public class BreakableLamp : FlickeringLamp
{
	public bool IsLampDestroyed { get; private set; } = false;

	[Header("Destroying Lamp Event")]

	[SerializeField] private ParticleSystem _electronicalSparkVF;

	[Space(10)]
	[SerializeField] private float _minLampLifeDelayBeforeBreaking = 40;
	[SerializeField] private float _maxLampLifeDelayBeforeBreaking = 180;

	private float _lampDelayBeforeBreakingRemaining;

	private void Start()
	{
		_electronicalSparkVF.gameObject.SetActive(false);

		_lampDelayBeforeBreakingRemaining = _maxLampLifeDelayBeforeBreaking;

		InitializeFlickeringLamp();
	}	

	private void Update()
	{
		TryStartFlashingEvent();

		TryBreakLamp();
	}

	private void OnTriggerEnter(Collider other)
	{
		TryInvokeLamp(other);
	}

	private void TryBreakLamp()
	{
		if (!IsLampDestroyed)
		{
			_lampDelayBeforeBreakingRemaining -= Time.deltaTime;

			if (_lampDelayBeforeBreakingRemaining <= 0)
				BreakLamp();
		}
		else if (_lampDelayBeforeBreakingRemaining <= 0)
			_lampDelayBeforeBreakingRemaining = Random.Range(_minLampLifeDelayBeforeBreaking, _maxLampLifeDelayBeforeBreaking);
	}

	protected override void TryInvokeLamp(Collider other)
	{
		if (IsLampDestroyed)
			return;

		base.TryInvokeLamp(other);
	}

	protected override bool IsCanStartFlashingEvent()
	{
		if (IsLampDestroyed)
			return false;

		return base.IsCanStartFlashingEvent();
	}

	public void RepairLamp()
	{
		if (!IsLampDestroyed)
			return;

		LampRenderer.gameObject.SetActive(true);

		_electronicalSparkVF.gameObject.SetActive(false);

		SwitchLampState(true);

		StartFlashingEvent();

		IsLampDestroyed = false;
	}

	public void BreakLamp()
	{
		if (IsLampDestroyed)
			return;

		IsLampDestroyed = true;

		LampRenderer.gameObject.SetActive(false);

		SwitchLampState(false);

		_electronicalSparkVF.gameObject.SetActive(true);
	}

	private void OnValidate()
	{
		if (_minLampLifeDelayBeforeBreaking < 0)
			_minLampLifeDelayBeforeBreaking = 0;

		if (_maxLampLifeDelayBeforeBreaking < _minLampLifeDelayBeforeBreaking)
			_maxLampLifeDelayBeforeBreaking = _minLampLifeDelayBeforeBreaking;
	}
}
