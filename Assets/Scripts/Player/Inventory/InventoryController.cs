using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private KeyCode _dropKey;
    [SerializeField] private KeyCode _openDoorKey;

    //[SerializeField] private Image[] _inventoryIcons;
    [SerializeField] private Transform _itemPlace; 
    public static int _curSlotIndex;
    private static ItemData[] Inventory;
    [SerializeField][Range(0,3)] private int _inventorySlotsAmount = 3;
    //ookk
    void Start()
    {
        Inventory = new ItemData[_inventorySlotsAmount];
        ChangeSlot();       
    }
    private void Update()
    {
        Debug.Log(Inventory.Length);
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _curSlotIndex = 0;
            ChangeSlot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _curSlotIndex = 1;
            ChangeSlot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _curSlotIndex = 2;
            ChangeSlot();
        }
        if (Input.GetKeyDown(_dropKey))
        {            
             DropItem();
        }
    
        float scroll = Input.GetAxis("Mouse ScrollWheel");
         if (scroll == 0) return;
        else if (scroll > 0) _curSlotIndex++;
         else _curSlotIndex--;
        if (_curSlotIndex < 0) _curSlotIndex = Inventory.Length - 1;
         else if (_curSlotIndex > Inventory.Length - 1) _curSlotIndex = 0;
        ChangeSlot();              
    }
    public static bool TryGetInventoryItem(int index, out ItemData item)
    {
        item = Inventory[index];

        //return Inventory[index];

        if (item)
            return true;

        return false;
    }
    private void DropItem()
    {
        Instantiate(Inventory[_curSlotIndex].Object, transform.position, Quaternion.identity);
        Inventory[_curSlotIndex] = null;
       // _inventoryIcons[_curSlotIndex].sprite = null;
        if(_itemPlace.childCount > 0)
        {
            Destroy(_itemPlace.GetChild(0).gameObject);
        }
    }
    private void FillSlot(ItemData item)
    {
        for(int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] == null)
            {
                Inventory[i] = item;
              //  _inventoryIcons[i].sprite = item.Icon;
                return;
            }
        }
        //заміна обєкта і написати користувачу що немає вільних слотів
    }
    private void ChangeSlot()
    {                
        //for(int i = 0; i < _inventoryIcons.Length; i++)
        //{
        //    _inventoryIcons[i].color = new Color(1, 1, 1, 0.5f);
        //}
        //_inventoryIcons[_curSlotIndex].color = new Color(1, 1, 1, 1);
        if(_itemPlace.childCount > 0)
        {
            Destroy(_itemPlace.GetChild(0).gameObject);
        }
        if (Inventory[_curSlotIndex] != null)
        Instantiate(Inventory[_curSlotIndex].Model, _itemPlace);
    }
    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Item") && Input.GetKey(KeyCode.F))
        {
            
            FillSlot(col.GetComponent<ItemScript>().data);
            ChangeSlot();
            Destroy(col.gameObject);
        }
    }
}
