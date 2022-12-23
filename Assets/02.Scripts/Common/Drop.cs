using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DataInfo;


public class Drop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            Drag.draggingItem.transform.SetParent(this.transform);

            Item item = Drag.draggingItem.GetComponent<ItemInfo>().itemData;
            G_Manager.g_Manager.AddItem(item);
        }
    }
}
