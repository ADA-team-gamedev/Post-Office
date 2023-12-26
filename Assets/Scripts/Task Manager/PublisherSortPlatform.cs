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
    [SerializeField] private List<Transform> _collectedObjects;   
    public PlatformSortEvent SortEvent;
    public void BoxPlaced(Transform placedObj, string Tag)
    {
        if (_requriedObjects.Contains(placedObj))
        {
            _collectedObjects.Add(placedObj);       
            foreach (Transform obj in _unsortedObjects) 
            {
                if (obj.tag == Tag)
                {
                    return;
                }
            }
            SortEvent.Invoke(_requriedTags.IndexOf(Tag));
        }

    }
}
