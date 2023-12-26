using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private string _requriedTag;

    [SerializeField] private PublisherSortPlatform _platform;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag(_requriedTag))
        {
            _platform.BoxPlaced(col.transform, col.tag);
            col.transform.parent = transform;
        }
    }
}
