using System;
using System.Collections;
using UnityEngine;

public class Pill : Item, IUsable
{
    [Header("Pill settings")]
    [SerializeField][Range(1, 5)] private int _countOfUses = 1;

    [Header("Values")]
    [SerializeField] private float _sanityAddingNumber;
    [SerializeField] private float _sanityAddingDelay;

    [SerializeField] private PlayerSanity _sanity;

    private bool _isUsing = false;

	public void Use()
    {
        if (IsHaveCharge())
        {
            if (!_isUsing)
            {
				_countOfUses--;

                Debug.Log($"{gameObject.name}s are used");

                //Play using pill sound

				StartCoroutine(ResoteSanity());
			}             
        }
        else
        {
            Debug.Log($"These {gameObject.name} pills are empty");

            //Play empty sound
        }
    }  

    private bool IsHaveCharge()
        => _countOfUses > 0;

    private IEnumerator ResoteSanity()
    {
        _isUsing = true;            
        
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
