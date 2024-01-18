using System;
using UnityEngine;
using UnityEngine.UI;

public class FlashLight : MonoBehaviour, IPickable, IUsable
{
    [Header("Objects")]
    [SerializeField] private Light _flashlight;
    [SerializeField] private Slider _batteryEnergyBar;
    [SerializeField] private GameObject _energyBar;

    [Header("Values")]
    [SerializeField] private int _numberOfActivations = 0;
    [SerializeField][Range(1, 100)] private float _dischargeRate;

    private bool _isCanTurnOn = false;

    private bool _isWorking = false;

    private bool _charged = true;

    public Action OnPickUpItem { get; set; }

    public Action OnDropItem { get; set; }

	private void Awake()
	{
		OnPickUpItem += PickUpItem;

        OnDropItem += DropItem;
	}

	#region Pickable

	public void PickUpItem()
    {
        _isCanTurnOn = true;
        Debug.Log("canturn");
    }

    public void DropItem()
    {
        
    }

    #endregion

    public void Use()
    {
        Debug.Log(0);
        if (_isCanTurnOn && _charged && _numberOfActivations == 0)
            TurnOn();

        else if (_numberOfActivations != 0)
            TurnOff();
    }  

    void Update()
    {
        FlashLightWorking();
    }

    private void FlashLightWorking()
    {
        if (_batteryEnergyBar.value <= 0)
        {
            _flashlight.enabled = false;

            _charged = false;
        }

        if (_isWorking && _charged)        
            _batteryEnergyBar.value -= Time.deltaTime / _dischargeRate;        
    }

    private void TurnOff()
    {                    
        _numberOfActivations--;

        _flashlight.enabled = false;

        _isWorking = false;       

        _energyBar.SetActive(false);
        
    }
    private void TurnOn()
    {
        _numberOfActivations++;

        _flashlight.enabled = true;

        _isWorking = true;          

        _energyBar.SetActive(true);
    }
}
