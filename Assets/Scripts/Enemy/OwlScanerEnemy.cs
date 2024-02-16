using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(FieldOfView))]
public class OwlScanerEnemy : MonoBehaviour
{
    private FieldOfView _fieldOfView;

	private void Start()
	{
		_fieldOfView ??= GetComponent<FieldOfView>();
	}

	private void Update()
	{
		_fieldOfView.VisionCheck();
	}
}
