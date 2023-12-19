using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashLightController : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private Light _flashlight;
    [SerializeField] private Transform _hand;
    [SerializeField] private Slider _batteryEnergyBar; 
    [SerializeField] private GameObject _energyBar; 
    

    [Header("Values")]
    [SerializeField] private int _timesTurnig;
    [SerializeField][Range(1, 100)] private float _dischargeRate;
    
    private bool _turning = false;
    [SerializeField] private bool _canTurn = false;
    private bool _charged = true;
    [SerializeField] private KeyCode _flashLightInteractionKey;      
    private void Start()
    {       
    }
    private void Update()
    {
        ChangeTurnOn();
        if (_batteryEnergyBar.value <= 0)
        {
            _flashlight.enabled = false;
            _charged = false;
        }
        if (_turning && _charged)
        {
            _batteryEnergyBar.value -= Time.deltaTime / _dischargeRate;
        }          
        if (!_turning && Input.GetKeyDown(_flashLightInteractionKey) && _canTurn && _charged)
        {                   
            _flashlight.enabled = true;
            _turning = true;
            _timesTurnig++;
            _energyBar.SetActive(true);
        } 
        else if(_timesTurnig != 0 && Input.GetKeyDown(_flashLightInteractionKey))
        {          
            _flashlight.enabled = false;
            _turning = false;
            _timesTurnig--;
            _energyBar.SetActive(false);
        }                      
    }   
    private void ChangeTurnOn()
    {            
        foreach(Transform Element in _hand)
        {
            if (Element.CompareTag("FlashLight"))
            {
                _canTurn = true;
            }          
        }       
        if (Input.GetKeyDown(KeyCode.G))
        {           
            _canTurn = false;
            _flashlight.enabled = false;
            _turning = false;
            _timesTurnig--;
            _energyBar.SetActive(false);
        }
    }
}
