using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashLightController : MonoBehaviour
{
    private bool _turning = false;
    private int _timesTurnig = 0;
    [SerializeField] private Light _flashlight;
    [SerializeField] private Slider _batteryEnergyBar;
    [SerializeField] private GameObject _batteryBar;
    [SerializeField] private KeyCode _flashLightInteractionKey;
    private ItemData[] Inventory;
    private void Start()
    {
        _batteryBar = GameObject.Find("BatteryEnergyBar");
        _batteryEnergyBar = _batteryBar.GetComponent<Slider>();
    }
    private void Update()
    {
        if (_turning)
            _batteryEnergyBar.value -= Time.deltaTime / 100;
        if (!_turning && Input.GetKeyDown(_flashLightInteractionKey))
        {
            Debug.Log(0);            
            _flashlight.enabled = true;
            _turning = true;
            _timesTurnig++;
            foreach (Transform Element in _batteryEnergyBar.transform)
                Element.gameObject.SetActive(true);                   
        } 
        else if(_timesTurnig != 0 && Input.GetKeyDown(_flashLightInteractionKey))
        {
            Debug.Log(1);
            _flashlight.enabled = false;
            _turning = false;
            _timesTurnig--;
            foreach (Transform Element in _batteryEnergyBar.transform)
                Element.gameObject.SetActive(false);

        }
    }   
}
