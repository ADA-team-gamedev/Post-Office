using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class SortEvent : UnityEvent<int>
{
    //none
}
public class PublisherSort : MonoBehaviour
{
    [SerializeField] private List<string> _requriedTags;
    private List<string> _collectedTags;
    public SortEvent _sortEvent;   
    private void RequriedPartCollected(string Tag)
    {
        _sortEvent.Invoke(_requriedTags.IndexOf(Tag));
        _collectedTags.Add(Tag);     
    }
    private void OnTriggerEnter(Collider col)
    {
        if (_requriedTags.Contains(col.tag) && !_collectedTags.Contains(col.tag))
        {
            RequriedPartCollected(col.tag);
        }
    }
}
