using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    [SerializeField] private ItemData _data;
    public ItemData Data
    {
        get { return _data; }
        set { _data = value; }
    }
}
