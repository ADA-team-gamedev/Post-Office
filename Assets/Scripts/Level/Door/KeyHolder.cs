using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHolder : MonoBehaviour
{
	[SerializeField] private string _keyTag = "Key";

    static private List<DoorKeyTypes> _keyList;

	private void Awake()
	{
		_keyList = new();
	}

	static public void AddKey(DoorKeyTypes keyType)
	{
		Debug.Log($"Added Key: {keyType}");

		_keyList.Add(keyType);
	}

	static public void RemoveKey(DoorKeyTypes keyType)
		=> _keyList.Remove(keyType);

	static public bool ContainsKey(DoorKeyTypes keyType)
		=> _keyList.Contains(keyType);	

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(_keyTag))
		{
			if (other.TryGetComponent(out Key key))
			{
				AddKey(key.DoorKeyType);

				Destroy(key.gameObject); //create a pick up method
			}
		}		
	}
}
