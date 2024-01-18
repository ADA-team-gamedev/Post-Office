using System;
using System.Collections;
using UnityEngine;

public class Pill : MonoBehaviour, IPickable, IUsable
{
    [SerializeField][Range(1, 5)] private int _countOfUses = 1;

    [SerializeField] private float _sanityAddingNumber;
    [SerializeField] private float _sanityAddingDelay;

    [SerializeField] private PlayerSanity _sanity;

    private bool _isUsing = false;

	public Action OnPickUpItem { get; set; }
	public Action OnDropItem { get; set; }

    public void Use()
    {
        if (IsHaveCharge() && !_isUsing)
        {
            _countOfUses--;

            StartCoroutine(EffectCooldown());          
        }        
    }  

    private bool IsHaveCharge()
        => _countOfUses > 0;

    private IEnumerator EffectCooldown()
    {
        _isUsing = true;

        if (!IsHaveCharge())
            Destroy(gameObject, _sanityAddingDelay);            
        
        float timer = _sanityAddingDelay;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            _sanity.Sanity += _sanityAddingNumber / _sanityAddingDelay * Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        _isUsing = false;
    }
}
