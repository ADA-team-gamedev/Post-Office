using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class PillEffect : MonoBehaviour
{
    [SerializeField][Range(1, 5)] private byte _countOfPiils = 0;
    [SerializeField] private float _countAddingSanity;
    [SerializeField] private PlayerSanity _sanity;
    private bool _isCanUsePills = true;  
    private void Update()
    {       
    }
    public void PickItem()
    {

    }
    public void DropItem()
    {

    }
    //private void SanityAdding()
    //{                      
    //    if (Input.GetKeyDown(_useKey) && _countOfPiils >= 1)
    //    {                
    //        if (_isCanUsePills)
    //            StartCoroutine(EffectCooldown());                                                          
    //    }
    //    else if (_countOfPiils < 1)
    //    {              
    //        Destroy(gameObject);
    //    }                
    //}
    IEnumerator EffectCooldown()
    {
        _isCanUsePills = false;
        _sanity.Sanity += _countAddingSanity;
        _countOfPiils--;
        yield return new WaitForSeconds(10f);
        _isCanUsePills = true;
    }
}
