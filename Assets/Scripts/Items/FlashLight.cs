using UnityEngine;

public class FlashLight : Item, IUsable
{
    [Header("Objects")]
    [SerializeField] private Light _flashlight;

    [SerializeField] private bool _disableOnStart = true;

    public bool IsWorking { get; private set; } = false;

	private void Awake()
	{
        IsWorking = !_disableOnStart;

        _flashlight.enabled = IsWorking;
	}

    public void Use()
    {
        if (!IsWorking)
            TurnOn();
        else
            TurnOff();
    }  

    private void TurnOff()
    {                    
        _flashlight.enabled = false;

        IsWorking = false;       
    }
    private void TurnOn()
    {
        _flashlight.enabled = true;

        IsWorking = true;          
    }
}
