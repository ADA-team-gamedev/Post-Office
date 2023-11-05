using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubejctsSelector : MonoBehaviour
{
    [SerializeField] private float _distance;
    [SerializeField] private int _powerOfDrop;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _position;
    private Rigidbody _rb;
    private TakingOnce _fpcParam;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _fpcParam = GameObject.Find("First Person Controller").GetComponent<TakingOnce>();
    }
    private void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, _distance) && _fpcParam.Take == false)
        {
            _rb.isKinematic = true;
            _fpcParam.Take = true;
            this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    private void FixedUpdate()
    {
        if(_rb.isKinematic == true)
        {            
            this.gameObject.transform.SetParent(_player);
            this.gameObject.transform.position = _position.position;                                
            if (Input.GetKeyDown(KeyCode.F))
            {
                _fpcParam.Take = false;
                _rb.useGravity = true;
                _rb.isKinematic = false;
                _rb.AddForce(Camera.main.transform.forward * _powerOfDrop);
                this.gameObject.transform.SetParent(null);
            }
        }
    }
}
