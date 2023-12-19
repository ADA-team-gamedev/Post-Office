using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlatformSortEvent : UnityEvent<int>
{
    //none
}

public class PublisherSortPlatform : MonoBehaviour
{
    [SerializeField] private List<Transform> _requriedObjects;
    [SerializeField] private List<string> _requriedTags;
    [SerializeField] private Transform _unsortedObjects;
    private List<Transform> _collectedObjects;   
    public PlatformSortEvent PlatformSortEvent;
    public void BoxPlaced(Transform placedObj, string Tag)
    {
        if (_requriedObjects.Contains(placedObj))
        {
            placedObj.parent = _collectedObjects[_requriedTags.IndexOf(Tag)];
            foreach (Transform obj in _unsortedObjects) 
            {
                if (obj.tag == Tag)
                {
                    return;
                }
            }
            PlatformSortEvent.Invoke(_requriedTags.IndexOf(Tag));
        }

    }
}
